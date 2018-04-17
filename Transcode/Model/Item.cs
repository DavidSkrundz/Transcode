using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Transcode.Model {
	public class Item : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public string InputPath { get; }
		public string OutputPath { get; }

		public ItemStatus Status {
			get => this.status;
			set {
				this.SetField(ref this.status, value);
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanRetry"));
			}
		}
		public bool CanRetry => this.Status == ItemStatus.Done || this.status == ItemStatus.Error;
		
		private ItemStatus status;
		
		public Item(string inputBasePath, string inputRelativePath, string inputFileName, string outputBasePath, string outputRelativePath, int outputNumber, string outputFileName) {
			this.InputPath = Path.Combine(inputBasePath, inputRelativePath, inputFileName);
			this.OutputPath = Path.Combine(outputBasePath, outputRelativePath, (outputNumber < 0 ? "" : string.Format("{0:00} - ", outputNumber)) + outputFileName + ".mp4");
		}

		// Boiler-plate for INotifyPropertyChanged
		private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
			if (EqualityComparer<T>.Default.Equals(field, value)) {
				return false;
			}
			field = value;
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
	}
}
