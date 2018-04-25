using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
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

		public ObservableCollection<string> Presets =>
				// TODO: Load presets on  background thread at some point
				this.presets;

		private string handbrakePath = "";
		private string inputRootPath = "";
		private string outputRootPath = "";

		private ObservableCollection<string> presets = new ObservableCollection<string>();

		// Boiler-plate for INotifyPropertyChanged
		private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
			if (EqualityComparer<T>.Default.Equals(field, value)) { return false; }
			field = value;
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
	}
}
