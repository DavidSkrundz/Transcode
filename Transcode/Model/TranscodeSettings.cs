using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Transcode.Exceptions;

namespace Transcode.Model {
	public class TranscodeSettings : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public string HandbrakePath {
			get => this.handbrakePath;
			set {
				try {
					if (value == null || !Path.IsPathRooted(value)) { throw new BindingException("Path is not absolute"); }
					this.SetField(ref this.handbrakePath, value);
				} catch (ArgumentException e) { throw new BindingException(e.Message); }
			}
		}

		public string InputRootPath {
			get => this.inputRootPath;
			set {
				try {
					if (value == null || !Path.IsPathRooted(value)) { throw new BindingException("Path is not absolute"); }
					this.SetField(ref this.inputRootPath, value);
				} catch (ArgumentException e) { throw new BindingException(e.Message); }
			}
		}

		public string OutputRootPath {
			get => this.outputRootPath;
			set {
				try {
					if (value == null || !Path.IsPathRooted(value)) { throw new BindingException("Path is not absolute"); }
					this.SetField(ref this.outputRootPath, value);
				} catch (ArgumentException e) { throw new BindingException(e.Message); }
			}
		}

		public ObservableCollection<string> Presets => this.presets;

		private string handbrakePath = "";
		private string inputRootPath = "";
		private string outputRootPath = "";

		private ObservableCollection<string> presets = new ObservableCollection<string>();

		public TranscodeSettings() {
			this.LoadSettings();
		}

		public void ReloadPresets() {
			this.Presets.Clear();
			var startInfo = new ProcessStartInfo() {
				FileName = this.HandbrakePath,
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

		private string configFileName = "transcode.config";
		private string handbrakeName = "HandbrakePath";
		private string inputRootName = "InputRootPath";
		private string outputRootName = "OutputRootPath";
		public void SaveSettings() {
			var writer = new StringWriter();
			writer.Write(this.handbrakeName);
			writer.Write("=");
			writer.WriteLine(this.HandbrakePath);
			writer.Write(this.inputRootName);
			writer.Write("=");
			writer.WriteLine(this.InputRootPath);
			writer.Write(this.outputRootName);
			writer.Write("=");
			writer.WriteLine(this.OutputRootPath);
			File.WriteAllText(this.configFileName, writer.ToString());
		}

		private void LoadSettings() {
			try {
				using (var file = new StreamReader(this.configFileName)) {
					string line;
					char[] separator = { '=' };
					while ((line = file.ReadLine()) != null) {
						var parts = line.Split(separator, 2);
						if (parts.Length != 2) { MessageBox.Show("Bad config file"); }
						if (parts[0] == this.handbrakeName) {
							this.handbrakePath = parts[1];
							this.ReloadPresets();
						} else if (parts[0] == this.inputRootName) {
							this.inputRootPath = parts[1];
						} else if (parts[0] == this.outputRootName) {
							this.outputRootPath = parts[1];
						}
					}
				}
			} catch { }
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
