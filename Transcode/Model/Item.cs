using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Transcode.Exceptions;
using Transcode.Extensions;

namespace Transcode.Model {
	public class Item : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public string InputPath => Path.Combine(this.inputBasePath, this.inputRelativePath, Path.ChangeExtension(this.inputFileName, this.inputFileExtension));
		public string OutputPath => Path.Combine(this.outputBasePath, this.outputRelativePath, this.OutputFile);
		private string OutputFile => Path.ChangeExtension(this.OutputFilePrefix + this.outputFileName, this.outputFileExtension);
		private string OutputFilePrefix => this.outputFileNumber < 0 ? "" : string.Format("{0:00} - ", this.outputFileNumber);

		public string InputBasePath {
			get => this.inputBasePath;
			set {
				try {
					if (value == null || !Path.IsPathRooted(value)) { throw new BindingException("Path is not absolute"); }
					this.SetField(ref this.inputBasePath, value);
				} catch (ArgumentException e) { throw new BindingException(e.Message); }
			}
		}
		public string InputRelativePath {
			get => this.inputRelativePath;
			set {
				try {
					if (value == null || Path.IsPathRooted(value)) { throw new BindingException("Relative path cannot be absolute"); }
					this.SetField(ref this.inputRelativePath, value);
				} catch (ArgumentException e) { throw new BindingException(e.Message); }
			}
		}
		public string InputFileName {
			get => this.inputFileName;
			set {
				try {
					if (value == null || !PathExtension.IsValidFileName(value)) { throw new BindingException("Illegal characters in name."); }
					this.SetField(ref this.inputFileName, value);
				} catch (ArgumentException e) { throw new BindingException(e.Message); }
			}
		}
		public string InputFileExtension {
			get => this.inputFileExtension;
			set {
				try {
					if (value == null || !PathExtension.IsValidFileName(value)) { throw new BindingException("Illegal characters in name."); }
					this.SetField(ref this.inputFileExtension, value);
				} catch (ArgumentException e) { throw new BindingException(e.Message); }
			}
		}

		public string OutputBasePath {
			get => this.outputBasePath;
			set {
				try {
					if (value == null || !Path.IsPathRooted(value)) { throw new BindingException("Path is not absolute"); }
					this.SetField(ref this.outputBasePath, value);
				} catch (ArgumentException e) { throw new BindingException(e.Message); }
			}
		}
		public string OutputRelativePath {
			get => this.outputRelativePath;
			set {
				try {
					if (value == null || Path.IsPathRooted(value)) { throw new BindingException("Relative path cannot be absolute"); }
					this.SetField(ref this.outputRelativePath, value);
				} catch (ArgumentException e) { throw new BindingException(e.Message); }
			}
		}
		public string OutputFileNumber {
			get => this.outputFileNumber < 0 ? "" : this.outputFileNumber.ToString();
			set {
				try {
					if (value == null) { throw new BindingException("Number cannot be null"); }
					if (NotNumberRegex.IsMatch(value)) { throw new BindingException("Only digits are allowed"); }
					this.SetField(ref this.outputFileNumber, value.Length == 0 ? -1 : int.Parse(value));
				} catch (OverflowException e) { throw new BindingException(e.Message); }
			}
		}
		public string OutputFileName {
			get => this.outputFileName;
			set {
				try {
					if (value == null || !PathExtension.IsValidFileName(value)) { throw new BindingException("Illegal characters in name."); }
					this.SetField(ref this.outputFileName, value);
				} catch (ArgumentException e) { throw new BindingException(e.Message); }
			}
		}

		public ItemStatus Status {
			get => this.status;
			set {
				this.SetField(ref this.status, value);
				this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanRetry"));
			}
		}
		public bool CanRetry => this.Status == ItemStatus.Done || this.status == ItemStatus.Error;

		private string inputBasePath = "";
		private string inputRelativePath = "";
		private string inputFileName = "";
		private string inputFileExtension = "";
		private string outputBasePath = "";
		private string outputRelativePath = "";
		private int outputFileNumber = -1;
		private string outputFileName = "";
		private string outputFileExtension = ".mp4";
		private ItemStatus status;

		private static Regex NotNumberRegex = new Regex("[^0-9]+");

		// Boiler-plate for INotifyPropertyChanged
		private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
			if (EqualityComparer<T>.Default.Equals(field, value)) { return false; }
			field = value;
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
	}
}
