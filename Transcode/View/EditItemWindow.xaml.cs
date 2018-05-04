using System;
using System.Windows;
using Transcode.ViewModel;

namespace Transcode.View {
	/// <summary>
	/// Interaction logic for EditItemWindow.xaml
	/// </summary>
	public partial class EditItemWindow : Window {
		private ItemViewModel itemViewModel;

		public EditItemWindow(ItemViewModel itemViewModel) {
			InitializeComponent();

			this.itemViewModel = itemViewModel ?? throw new ApplicationException("new EditItemWindow(): itemViewModel is null");
			this.DataContext = this.itemViewModel;
		}

		private void DoneButtonClicked(object sender, RoutedEventArgs eventArgs) {
			if (this.itemViewModel.Item.PresetName.Length == 0) { return; }
			this.Close();
		}
	}
}
