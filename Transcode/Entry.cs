using System;
using System.IO;

namespace Transcode {
	public class Entry {
		[STAThread]
		public static void Main(string[] args) {
			using (var file = File.CreateText("transcode.log")) {
				try {
					var app = new App();
					app.InitializeComponent();
					app.Run();
				} catch (Exception e) {
					file.WriteLine(e.ToString());
					file.Flush();
				}
			}
		}
	}
}
