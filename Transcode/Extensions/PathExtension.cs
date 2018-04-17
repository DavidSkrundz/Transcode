using System;
using System.IO;
using System.Linq;

namespace Transcode.Extensions {
	public static class PathExtension {
		public static bool IsValidPathName(string path) {
			var invalidPathCharacters = Path.GetInvalidPathChars();
			foreach (var character in path) {
				if (invalidPathCharacters.Contains(character)) {
					return false;
				}
			}
			return true;
		}

		public static bool IsValidFileName(string name) {
			var invalidFileCharacters = Path.GetInvalidFileNameChars();
			foreach (var character in name) {
				if (invalidFileCharacters.Contains(character)) {
					return false;
				}
			}
			return true;
		}

		public static string GetRelativePath(string source, string target) {
			if (!Path.IsPathRooted(source)) { throw new ArgumentException("Source path cannot be relative"); }
			if (!Path.IsPathRooted(target)) { throw new ArgumentException("Target path cannot be relative"); }

			var sourceComponents = source.Split(Path.DirectorySeparatorChar);
			var targetComponents = target.Split(Path.DirectorySeparatorChar);

			if (sourceComponents.Length > targetComponents.Length) {
				throw new ApplicationException("Target must be contained within Source");
			}

			for (var i = 0; i < sourceComponents.Length; ++i) {
				if (!string.Equals(sourceComponents[i], targetComponents[i], StringComparison.CurrentCultureIgnoreCase)) {
					throw new ApplicationException("Target must be contained within Source");
				}
			}

			return string.Join(Path.DirectorySeparatorChar.ToString(), targetComponents.Skip(sourceComponents.Length).ToArray());
		}
	}
}
