namespace Transcode.Model {
	public class NewItemRequest {
		public string InputPath { get; set; } = null;
		public string InputName { get; set; } = null;

		public string OutputPath { get; set; } = null;
		public int OutputNumber { get; set; } = -1;
		public string OutputName { get; set; } = null;
	}
}
