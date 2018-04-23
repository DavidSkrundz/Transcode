using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Transcode.Model {
	public class TranscodeSettings : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public string HandbrakePath {
			get => this.handbrakePath;
			set {
				if (value != null && !Path.IsPathRooted(value)) { throw new ApplicationException("set TranscodeSettings.HandbrakeLocation: value not a rooted path"); }
				this.SetField(ref this.handbrakePath, value);
			}
		}

		public string InputRootPath {
			get => this.inputRootPath;
			set {
				if (value != null && !Path.IsPathRooted(value)) { throw new ApplicationException("set TranscodeSettings.InputRootPath: value not a rooted path"); }
				this.SetField(ref this.inputRootPath, value);
			}
		}

		public string OutputRootPath {
			get => this.outputRootPath;
			set {
				if (value != null && !Path.IsPathRooted(value)) { throw new ApplicationException("set TranscodeSettings.OutputRootPath: value not a rooted path"); }
				this.SetField(ref this.outputRootPath, value);
			}
		}

		private string handbrakePath = "";
		private string inputRootPath = "";
		private string outputRootPath = "";

		// Boiler-plate for INotifyPropertyChanged
		private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
			if (EqualityComparer<T>.Default.Equals(field, value)) { return false; }
			field = value;
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
	}
}
