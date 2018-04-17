namespace Transcode.Model {
	public class NewItemResponse {
		public bool SkipFile { get; set; } = false;
		public bool SkipFolder { get; set; } = false;
		public bool SkipAll { get; set; } = false;

		public string Path { get; set; } = null;
		public int Number { get; set; } = -1;
		public string Name { get; set; } = null;

		public bool IsValid =>
			(this.Path != null && this.Name != null) ||
			(this.SkipFile || this.SkipFolder || this.SkipAll);
	}
}
