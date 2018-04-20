using System;
using System.IO;
using System.Linq;

namespace Transcode.Extensions {
	public static class PathExtension {
		// Returns: `false` if the path contains invalid characters
		//
		// Exceptions:
		//   System.ArgumentException:
		//     path is null
		public static bool IsValidPathName(string path) {
			if (path == null) { throw new ArgumentException("Path is null"); }
			var invalidPathCharacters = Path.GetInvalidPathChars();
			foreach (var character in path) {
				if (invalidPathCharacters.Contains(character)) {
					return false;
				}
			}
			return true;
		}

		// Returns: `false` if the file name contains invalid characters
		//
		// Exceptions:
		//   System.ArgumentException:
		//     name is null
		public static bool IsValidFileName(string name) {
			if (name == null) { throw new ArgumentException("Name is null"); }
			var invalidFileCharacters = Path.GetInvalidFileNameChars();
			foreach (var character in name) {
				if (invalidFileCharacters.Contains(character)) {
					return false;
				}
			}
			return true;
		}

		// Returns: The relative path from the source path to the target path
		//
		// Exceptions:
		//   System.ArgumentException:
		//     source is null
		//     target is null
		//     source is not an absolute path
		//     target is not an absolute path
		//     target is not contained within source
		public static string GetRelativePath(string source, string target) {
			if (source == null) { throw new ArgumentException("Source path is null"); }
			if (target == null) { throw new ArgumentException("Target path is null"); }
			if (!Path.IsPathRooted(source)) { throw new ArgumentException("Source path cannot be relative"); }
			if (!Path.IsPathRooted(target)) { throw new ArgumentException("Target path cannot be relative"); }

			var sourceComponents = source.Split(Path.DirectorySeparatorChar);
			var targetComponents = target.Split(Path.DirectorySeparatorChar);

			if (sourceComponents.Length > targetComponents.Length) {
				throw new ArgumentException("Target must be contained within Source");
			}

			for (var i = 0; i < sourceComponents.Length; ++i) {
				if (!string.Equals(sourceComponents[i], targetComponents[i], StringComparison.CurrentCultureIgnoreCase)) {
					throw new ArgumentException("Target must be contained within Source");
				}
			}

			return string.Join(Path.DirectorySeparatorChar.ToString(), targetComponents.Skip(sourceComponents.Length).ToArray());
		}
	}
}
