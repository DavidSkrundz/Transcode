using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Transcode.Extensions {
	public static class ProcessExtension {
		[Flags]
		private enum ThreadAccess : int {
			SuspendResume = 2,
		}

		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenThread(ThreadAccess desiredAccess, bool inheritHandle, uint threadID);
		[DllImport("kernel32.dll")]
		private static extern uint SuspendThread(IntPtr thread);
		[DllImport("kernel32.dll")]
		private static extern int ResumeThread(IntPtr thread);
		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool CloseHandle(IntPtr handle);

		// Exceptions:
		//   System.ArgumentException:
		//     process is null
		public static void Suspend(this Process process) {
			if (process == null) { throw new ArgumentException("Process is null"); }
			foreach (var thread in process.Threads.OfType<ProcessThread>()) {
				var threadHandle = OpenThread(ThreadAccess.SuspendResume, false, (uint)thread.Id);
				if (threadHandle == IntPtr.Zero) { continue; }
				SuspendThread(threadHandle);
				CloseHandle(threadHandle);
			}
		}

		// Exceptions:
		//   System.ArgumentException:
		//     process is null
		public static void Resume(this Process process) {
			if (process == null) { throw new ArgumentException("Process is null"); }
			foreach (var thread in process.Threads.OfType<ProcessThread>()) {
				var threadHandle = OpenThread(ThreadAccess.SuspendResume, false, (uint)thread.Id);
				if (threadHandle == IntPtr.Zero) { continue; }
				var count = 0;
				do { count = ResumeThread(threadHandle); } while (count > 0);
				CloseHandle(threadHandle);
			}
		}
	}
}
