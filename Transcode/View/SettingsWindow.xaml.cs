using System;
using System.Windows;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using Transcode.Model;

namespace Transcode.View {
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window {
		private TranscodeSettings settings;

		public SettingsWindow(TranscodeSettings settings) {
			InitializeComponent();

			this.settings = settings ?? throw new ApplicationException("new SettingsWindow(): settings is null");
			this.DataContext = this.settings;
		}

		private void LocateHandbrakeCLI(object sender, RoutedEventArgs eventArgs) {
			var dialog = new OpenFileDialog() {
				Filter = "Executable|*.exe"
			};
			if (dialog.ShowDialog() == true) {
				this.settings.HandbrakePath = dialog.FileName;
			}
		}

		private void LocateInputRootPath(object sender, RoutedEventArgs eventArgs) {
			using (var dialog = new CommonOpenFileDialog()) {
				dialog.IsFolderPicker = true;
				dialog.EnsurePathExists = true;

				var result = dialog.ShowDialog();
				if (result == CommonFileDialogResult.Ok) {
					this.settings.InputRootPath = dialog.FileName;
				}
			}
		}

		private void LocateOutputRootPath(object sender, RoutedEventArgs eventArgs) {
			using (var dialog = new CommonOpenFileDialog()) {
				dialog.IsFolderPicker = true;
				dialog.EnsurePathExists = true;

				var result = dialog.ShowDialog();
				if (result == CommonFileDialogResult.Ok) {
					this.settings.OutputRootPath = dialog.FileName;
				}
			}
		}
	}
}
