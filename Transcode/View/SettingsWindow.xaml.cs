using System;
using System.Windows;
using Transcode.Model;

namespace Transcode.View {
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : Window {
		private TranscodeSettings settings;

		public SettingsWindow(TranscodeSettings settings) {
			InitializeComponent();

			this.settings = settings ?? throw new ApplicationException("Settings is null");
		}
	}
}
