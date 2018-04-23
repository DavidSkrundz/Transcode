using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transcode.Exceptions {
	// Visual Studio should not break on this exception
	public class BindingException : Exception {
		public BindingException(string message) : base(message) {}
	}
}
