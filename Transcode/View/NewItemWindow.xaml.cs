using System;
using System.Text.RegularExpressions;
using System.Windows;
using Transcode.Extensions;
using Transcode.Model;

namespace Transcode.View {
	/// <summary>
	/// Interaction logic for NewItemWindow.xaml
	/// </summary>
	public partial class NewItemWindow : Window {
		public NewItemRequest Request { get; set; }
		public NewItemResponse Response { get; } = new NewItemResponse();

		private static Regex DigitRegex = new Regex("^[0-9]+$");

		public NewItemWindow() {
			InitializeComponent();

			this.Loaded += delegate {
				if (this.Request == null) { throw new ApplicationException("A Request must be set."); }
				this.InputPathLabel.Content = this.Request.InputPath;
				this.InputNameLabel.Content = this.Request.InputName;
				this.OutputPathField.Text = this.Request.OutputPath;
				this.OutputNumberField.Text = this.Request.OutputNumber.ToString();
			};
		}

		private void ActionComboBoxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs eventArgs) {
			if (sender != this.ActionComboBox) { throw new ApplicationException("Invalid ComboBox"); }
			if (this.ActionComboBox.SelectedItem == this.AlternateActionItem) {
				// Do nothing
			} else if (this.ActionComboBox.SelectedItem == this.SkipFileItem) {
				this.Response.SkipFile = true;
				this.Close();
			} else if (this.ActionComboBox.SelectedItem == this.SkipFolderItem) {
				this.Response.SkipFolder = true;
				this.Close();
			} else if (this.ActionComboBox.SelectedItem == this.SkipAllItem) {
				this.Response.SkipAll = true;
				this.Close();
			} else { throw new ApplicationException("Invalid ComboBox Item"); }
		}

		private void ButtonClicked(object sender, RoutedEventArgs eventArgs) {
			if (sender != this.AddButton) { throw new ApplicationException("Invalid Button"); }
			if (this.OutputPathField.Text == "") {
				// TODO: Show something about path being empty
			} else if (!PathExtension.IsValidPathName(this.OutputPathField.Text)) {
				// TODO: Show something about path being invalid
			} else if (this.OutputNameField.Text == "") {
				// TODO: Show something about name being empty
			} else if (!PathExtension.IsValidFileName(this.OutputNameField.Text)) {
				// TODO: Show something about name being invalid
			} else if (this.OutputNumberField.Text.Length != 0 && !DigitRegex.IsMatch(this.OutputNumberField.Text)) {
				// TODO: Show something about number being invalid
			} else {
				this.Response.Path = this.OutputPathField.Text;
				if (this.OutputNumberField.Text.Length > 0) {
					this.Response.Number = int.Parse(this.OutputNumberField.Text);
				}
				this.Response.Name = this.OutputNameField.Text;
				this.Close();
			}
		}
	}
}
