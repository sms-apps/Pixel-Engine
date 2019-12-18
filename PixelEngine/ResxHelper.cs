using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace PixelEngine {
	/// <summary> Helper class to extract builtin resource files </summary>
	internal static class ResxHelper {
		/// <summary> DLL files are stored in </summary>
		private const string DllName = "PixGL.dll";

		/// <summary> Native pointer to loaded dll </summary>
		private static IntPtr dllHandle;

		/// <summary> Call to initialize dll </summary>
		public static void LoadDll() { dllHandle = Windows.LoadLibrary(LoadFile(DllName)); }

		/// <summary> Extract all font files into temp folder . </summary>
		public static void LoadFonts() {
			LoadFile("Retro.png");
			LoadFile("Modern.png");
			LoadFile("Formal.png");
			LoadFile("Handwritten.png");

			LoadFile("Modern.dat");
			LoadFile("Formal.dat");
			LoadFile("Handwritten.dat");
		}

		/// <summary> Extract a file into the temp folder </summary>
		/// <param name="file"> File name to extract </param>
		/// <returns> Path where the file was loaded </returns>
		private static string LoadFile(string file) {
			Assembly assembly = Assembly.GetExecutingAssembly();

			string path = Path.Combine(Windows.TempPath, file);

			using (Stream stream = assembly.GetManifestResourceStream($"{nameof(PixelEngine)}.Properties.{file}")) {
				try {
					using (Stream outFile = File.Create(path)) {
						const int Size = 4096;

						byte[] buffer = new byte[Size];

						while (true) {
							int nRead = stream.Read(buffer, 0, Size);

							if (nRead < 1) { break; }

							outFile.Write(buffer, 0, nRead);
						}
					}
				} catch { }
			}

			return path;
		}

		/// <summary> Call to unload dll </summary>
		public static void DestroyDll() { Windows.FreeLibrary(dllHandle); }
	}
}
