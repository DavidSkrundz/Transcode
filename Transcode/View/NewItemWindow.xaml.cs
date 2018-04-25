using System;
using System.Windows;
using System.Windows.Controls;
using Transcode.Model;
using Transcode.ViewModel;

namespace Transcode.View {
	public enum NewItemWindowResult {
		Ok,
		SkipFile,
		SkipFolder,
		SkipAll,
	}

	/// <summary>
	/// Interaction logic for NewItemWindow.xaml
	/// </summary>
	public partial class NewItemWindow : Window {
		private ItemViewModel itemViewModel;
		private NewItemWindowResult result = NewItemWindowResult.SkipFile;

		public NewItemWindow(ItemViewModel itemViewModel) {
			InitializeComponent();

			this.itemViewModel = itemViewModel ?? throw new ApplicationException("new NewItemWindow(): itemViewModel is null");
			this.DataContext = this.itemViewModel;
		}

		public new NewItemWindowResult ShowDialog() {
			base.ShowDialog();
			return this.result;
		}

		private void ActionComboBoxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs eventArgs) {
			var comboBox = (ComboBox)sender;
			if (comboBox.SelectedItem == this.AlternateActionItem) {
				// Do nothing
			} else if (comboBox.SelectedItem == this.SkipFileItem) {
				this.result = NewItemWindowResult.SkipFile;
				this.Close();
			} else if (comboBox.SelectedItem == this.SkipFolderItem) {
				this.result = NewItemWindowResult.SkipFolder;
				this.Close();
			} else if (comboBox.SelectedItem == this.SkipAllItem) {
				this.result = NewItemWindowResult.SkipAll;
				this.Close();
			} else { throw new ApplicationException("Invalid ComboBox Item"); }
		}

		private void AddButtonClicked(object sender, RoutedEventArgs eventArgs) {
			if (this.itemViewModel.Item.PresetName.Length == 0) { return; }
			this.result = NewItemWindowResult.Ok;
			this.Close();
		}
	}
}
