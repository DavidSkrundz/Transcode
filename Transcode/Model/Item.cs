using System;
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

		// Exceptions:
		//   System.ArgumentException:
		//     process is null
		//     any of the inputs contain invalid characters
		public Item(string inputBasePath, string inputRelativePath, string inputFileName, string outputBasePath, string outputRelativePath, int outputNumber, string outputFileName) {
			if (inputBasePath == null) { throw new ArgumentException("InputBasePath is null"); }
			if (inputRelativePath == null) { throw new ArgumentException("InputRelativePath is null"); }
			if (inputFileName == null) { throw new ArgumentException("InputFileName is null"); }
			if (outputBasePath == null) { throw new ArgumentException("OutputBasePath is null"); }
			if (outputRelativePath == null) { throw new ArgumentException("OutputRelativePath is null"); }
			if (outputFileName == null) { throw new ArgumentException("OutputFileName is null"); }
			this.InputPath = Path.Combine(inputBasePath, inputRelativePath, inputFileName);
			this.OutputPath = Path.Combine(outputBasePath, outputRelativePath, (outputNumber < 0 ? "" : string.Format("{0:00} - ", outputNumber)) + outputFileName + ".mp4");
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
