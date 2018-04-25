using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using Transcode.Extensions;
using Transcode.Model;

namespace Transcode.View {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public bool IsRunning {
			get => this.isRunning;
			set => this.SetField(ref this.isRunning, value);
		}
		public ObservableCollection<Item> Items {
			get => this.items;
			set => this.SetField(ref this.items, value);
		}
		public ObservableCollection<string> Presets {
			get => this.presets;
			set => this.SetField(ref this.presets, value);
		}
		public string Progress {
			get => this.progress;
			set => this.SetField(ref this.progress, value);
		}

		private Process process = null;
		private bool isRunning = false;
		private ObservableCollection<Item> items = new ObservableCollection<Item>();
		private ObservableCollection<string> presets = new ObservableCollection<string>();
		private string progress = "";

		private TranscodeSettings settings = new TranscodeSettings();

		public MainWindow() {
			InitializeComponent();
		}
		
		private void SettingsButtonClicked(object sender, RoutedEventArgs eventArgs) {
			var settingsWindow = new SettingsWindow(this.settings) { Owner = this };
			settingsWindow.ShowDialog();
		}

		private void ReloadPresetsClicked(object sender, RoutedEventArgs eventArgs) {
			if (sender != this.ReloadPresetsButton) { throw new ApplicationException("Invalid Button"); }
			this.Presets.Clear();
			if (this.settings.HandbrakePath.Length == 0) { return; }
			var startInfo = new ProcessStartInfo() {
				FileName = this.settings.HandbrakePath,
				Arguments = "--preset-import-gui --preset-list",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true,
			};
			using (var handbrake = Process.Start(startInfo)) {
				var shouldAdd = false;
				while (!handbrake.StandardError.EndOfStream) {
					string line = null;
					try {
						line = handbrake.StandardError.ReadLine();
					} catch { return; }
					if (line.Length == 0) { continue; }
					if (line[0] == ' ' && shouldAdd) {
						this.Presets.Add(line.Trim(' '));
					} else {
						shouldAdd = line == "User Presets/";
					}
				}
			}
		}

		private void OpenButtonClicked(object sender, RoutedEventArgs eventArgs) {
			if (sender != this.OpenButton) { throw new ApplicationException("Invalid Button"); }
			if (this.PresetComboBox.SelectedIndex < 0) {
				// TODO: Show something about no preset selected
				return;
			}
			if (this.settings.InputRootPath == "") {
				// TODO: Show something about editing settings
				return;
			}
			if (this.settings.OutputRootPath == "") {
				// TODO: Show something about editing settings
				return;
			}
			using (var dialog = new CommonOpenFileDialog()) {
				dialog.IsFolderPicker = true;
				dialog.RestoreDirectory = false;
				dialog.InitialDirectory = this.settings.InputRootPath;
				dialog.EnsurePathExists = true;

				var result = dialog.ShowDialog();
				if (result == CommonFileDialogResult.Ok) {
					string path;
					try {
						path = PathExtension.GetRelativePath(this.settings.InputRootPath, dialog.FileName);
					} catch (ApplicationException e) {
						MessageBox.Show(e.Message);
						return;
					}
					this.ProcessFolder(path, this.settings.InputRootPath, this.settings.OutputRootPath);
				}
			}
		}

		private void DeleteButtonClicked(object sender, RoutedEventArgs eventArgs) {
			var button = sender as Button;
			var item = button.DataContext as Item;
			this.Items.Remove(item);

			if (item.Status == ItemStatus.Running) {
				this.IsRunning = false;
				this.process.Kill();
				this.process = null;
				this.progress = "";
				this.StartHandbrake();
			}
		}

		private void RetryButtonClicked(object sender, RoutedEventArgs eventArgs) {
			var button = sender as Button;
			var item = button.DataContext as Item;
			item.Status = ItemStatus.Pending;
		}
		
		private bool ProcessFolder(string inputRelativePath, string inputBasePath, string outputBasePath) {
			var outputRelativePath = inputRelativePath;
			var outputNumber = 0;
			IEnumerable<string> files = null;
			try {
				files = Directory.EnumerateFileSystemEntries(Path.Combine(inputBasePath, inputRelativePath)).OrderBy(filename => filename);
			} catch { return true; }
			foreach (var file in files) {
				if (Directory.Exists(file)) {
					var filePath = PathExtension.GetRelativePath(inputBasePath, file);
					if (!ProcessFolder(filePath, inputBasePath, outputBasePath)) { return false; }
				} else if (File.Exists(file)) {
					var inputFileName = Path.GetFileName(file);
					var newItem = new Item() {
						InputBasePath = inputBasePath,
						InputRelativePath = inputRelativePath,
						InputFileName = Path.GetFileName(inputFileName),
						InputFileExtension = Path.GetExtension(inputFileName),
						OutputBasePath = outputBasePath,
						OutputRelativePath = outputRelativePath,
						OutputFileNumber = (outputNumber + 1).ToString()
					};
					var newItemWindow = new NewItemWindow(newItem) { Owner = this };
					var response = newItemWindow.ShowDialog();
					if (response == NewItemWindowResult.SkipFile) { continue; }
					if (response == NewItemWindowResult.SkipFolder) { break; }
					if (response == NewItemWindowResult.SkipAll) { return false; }
					this.Items.Add(newItem);
					outputRelativePath = newItem.OutputRelativePath;
					outputNumber = int.Parse(newItem.OutputFileNumber);
				}
			}
			return true;
		}

		private void StartStopButtonClicked(object sender, RoutedEventArgs eventArgs) {
			if (sender != this.StartButton) { throw new ApplicationException("Invalid Button"); }
			if (this.PresetComboBox.SelectedIndex < 0) {
				// TODO: Show something about no preset selected
				return;
			}
			if (this.IsRunning) {
				this.PauseHandbrake();
			} else {
				this.StartHandbrake();
			}
		}

		private void StartHandbrake() {
			if (this.IsRunning) { throw new ApplicationException("Should not be running yet"); }
			if (this.process != null) { this.ResumeHandbrake(); return; }
			Item item = null;
			foreach (var i in this.Items) {
				if (i.Status == ItemStatus.Pending) {
					item = i;
					break;
				}
			}
			if (item == null) { return; }
			this.ProcessItem(item);
		}

		private void ResumeHandbrake() {
			if (this.process == null) { throw new ApplicationException("Process is null"); }
			if (this.IsRunning) { throw new ApplicationException("Should not be running"); }
			this.process.Resume();
			this.IsRunning = true;
		}

		private void PauseHandbrake() {
			if (this.process == null) { throw new ApplicationException("Process is null"); }
			if (!this.IsRunning) { throw new ApplicationException("Should be running"); }
			this.process.Suspend();
			this.IsRunning = false;
		}

		private void ProcessItem(Item item) {
			if (this.PresetComboBox.SelectedIndex < 0) { throw new ApplicationException("No preset selected"); }
			if (this.process != null) { throw new ApplicationException("Process not null"); }
			if (item == null) { throw new ApplicationException("Item is null"); }
			if (item.Status != ItemStatus.Pending) { throw new ApplicationException("Item should be pending"); }

			try {
				Directory.CreateDirectory(Path.GetDirectoryName(item.OutputPath));
			} catch {
				item.Status = ItemStatus.Error;
				return;
			}

			this.process = new Process {
				StartInfo = new ProcessStartInfo() {
					FileName = this.settings.HandbrakePath,
					Arguments = string.Format("--preset-import-gui --preset \"{0}\" --input \"{1}\" --output \"{2}\"", this.Presets[this.PresetComboBox.SelectedIndex], item.InputPath, item.OutputPath),
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true,
				}
			};

			this.process.ErrorDataReceived += (s, e) => { /* Do nothing; this needs to be read */ };
			this.process.OutputDataReceived += (s, e) => {
				if (e.Data == null || e.Data.Length == 0) { return; }
				this.Progress = e.Data;
			};
			this.process.Exited += (s, e) => {
				item.Status = this.process.ExitCode == 0 ? ItemStatus.Done : ItemStatus.Error;
				this.process.Dispose();
				this.process = null;
				var shouldStartNext = this.IsRunning;
				this.IsRunning = false;
				this.Progress = "";
				if (shouldStartNext) { this.Dispatcher.Invoke(this.StartHandbrake); }
			};
			this.process.EnableRaisingEvents = true;

			this.process.Start();
			this.process.PriorityClass = ProcessPriorityClass.Idle;
			this.process.BeginOutputReadLine();
			this.process.BeginErrorReadLine();

			item.Status = ItemStatus.Running;
			this.IsRunning = true;
		}

		// Boiler-plate for INotifyPropertyChanged
		private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
			if (EqualityComparer<T>.Default.Equals(field, value)) { return false; }
			field = value;
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
	}
}
