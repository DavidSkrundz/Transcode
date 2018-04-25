using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Transcode.Model;

namespace Transcode.ViewModel {
	public class ItemViewModel : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public Item Item {
			get => this.item;
			set => this.SetField(ref this.item, value);
		}

		public TranscodeSettings Settings {
			get => this.settings;
			set => this.SetField(ref this.settings, value);
		}

		private Item item = null;
		private TranscodeSettings settings = null;

		// Boiler-plate for INotifyPropertyChanged
		private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
			if (EqualityComparer<T>.Default.Equals(field, value)) { return false; }
			field = value;
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
	}
}
