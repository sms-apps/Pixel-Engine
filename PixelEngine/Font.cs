using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine {
	/// <summary> Class that holds information about a pixel font </summary>
	public class Font {
		/// <summary> Workaround for avoiding static constructor </summary>
		public static readonly bool Initialized = Init();
		private static bool Init() {
			ResxHelper.LoadFonts();
			retro = new Lazy<Font>(CreateRetro);
			modern = new Lazy<Font>(CreateModern);
			formal = new Lazy<Font>(CreateFormal);
			handwritten = new Lazy<Font>(CreateHandwritten);
			return true;
		}
		/// <summary> Lazy loader for <see cref="Presets.Retro"/></summary>
		private static Lazy<Font> retro;
		/// <summary> Lazy loader for <see cref="Presets.Modern"/></summary>
		private static Lazy<Font> modern;
		/// <summary> Lazy loader for <see cref="Presets.Formal"/></summary>
		private static Lazy<Font> formal;
		/// <summary> Lazy loader for <see cref="Presets.Handwritten"/></summary>
		private static Lazy<Font> handwritten;

		/// <summary> Enum for preset font </summary>
		public enum Presets { Retro, Modern, Formal, Handwritten }
		/// <summary> Helper conversion from <see cref="Presets"/> enum into <see cref="Font"/></summary>
		/// <param name="p"> Preset to convert </param>
		public static implicit operator Font(Presets p) {
			switch (p) {
				case Presets.Retro: return retro.Value;
				case Presets.Modern: return modern.Value;
				case Presets.Formal: return formal.Value;
				case Presets.Handwritten: return handwritten.Value;
			}
			return null;
		}
		
		/// <summary> Sprites representing each character in the font </summary>
		internal Dictionary<char, Sprite> Glyphs;
		/// <summary> Height of a line of text rendered with this font </summary>
		internal int CharHeight;

		private Font() { Glyphs = new Dictionary<char, Sprite>(); }

		internal Font(Dictionary<char, Sprite> glyphs) {
			Glyphs = glyphs;
			CharHeight = glyphs.Values.Max(g => g.Height);
		}

		/// <summary> Measures the width of a string of text when rendered with this font, if it was all on one line. </summary>
		/// <param name="text"> Text to measure  </param>
		/// <returns> Width, in pixels, of text </returns>
		public int TextWidth(string text) { return text.Sum(c => Glyphs[c].Width); }
		/// <summary> Measures the height of a string of text when rendered with this font. </summary>
		/// <param name="text"> Text to measure  </param>
		/// <returns> Height, in pixels, of text </returns>
		public int TextHeight(string text) { return (text.Count(c => c == '\n') + 1) * CharHeight; }

		#region Presets
		/// <summary> Load preset retro font </summary>
		/// <returns> Loaded font </returns>
		private static Font CreateRetro() {
			Font f = new Font();
			f.CharHeight = 8;

			Sprite spr = Sprite.Load(Windows.TempPath + "\\Retro.png");

			for (char cur = ' '; cur < 128; cur++) {
				Sprite fontChar = new Sprite(8, 8);

				int x = (cur - 32) % 16;
				int y = (cur - 32) / 16;

				for (int i = 0; i < 8; i++) {
					for (int j = 0; j < 8; j++) {
						fontChar[i, j] = spr[x * 8 + i, y * 8 + j];
					}
				}
				f.Glyphs.Add(cur, fontChar);
			}

			return f;
		}

		/// <summary> Load preset modern font </summary>
		/// <returns> Loaded font </returns>
		private static Font CreateModern() {
			Font f = new Font();
			f.CharHeight = 21;

			Sprite spr = Sprite.Load(Windows.TempPath + "\\Modern.png");

			using (FileStream stream = File.OpenRead(Windows.TempPath + "\\Modern.dat")) {
				using (BinaryReader reader = new BinaryReader(stream)) {
					reader.ReadBytes(16); // Offset 16 + 32
					reader.ReadBytes(33); // bytes into data

					byte[] widths = reader.ReadBytes(96); // widths of 96 ascii chars

					for (char cur = ' '; cur < 128; cur++) {
						byte w = widths[cur - 32];
						Sprite fontChar = new Sprite(w, 21);

						int x = (cur - 32) % 16;
						int y = (cur - 32) / 16;

						for (int i = 0; i < w; i++) { 
							for (int j = 0; j < 21; j++) {
								fontChar[i, j] = spr[x * 16 + i, y * 21 + j];
							}
						}

						f.Glyphs.Add(cur, fontChar);
					}
				}
			}

			return f;
		}

		/// <summary> Load preset formal font </summary>
		/// <returns> Loaded font </returns>
		private static Font CreateFormal() {
			Font f = new Font();
			f.CharHeight = 21;

			Sprite spr = Sprite.Load(Windows.TempPath + "\\Formal.png");

			using (FileStream stream = File.OpenRead(Windows.TempPath + "\\Formal.dat")) {
				using (BinaryReader reader = new BinaryReader(stream)) {
					reader.ReadBytes(16); // Offset 16 + 32
					reader.ReadBytes(33); // bytes into data

					byte[] widths = reader.ReadBytes(96); // widths of 96 ascii chars

					for (char cur = ' '; cur < 128; cur++) {
						byte w = widths[cur - 32];
						Sprite fontChar = new Sprite(w, 21);

						int x = (cur - 32) % 16;
						int y = (cur - 32) / 16;

						for (int i = 0; i < w; i++) {
							for (int j = 0; j < 21; j++) {
								fontChar[i, j] = spr[x * 16 + i, y * 21 + j];
							}
						}
						f.Glyphs.Add(cur, fontChar);
					}
				}
			}

			return f;
		}

		private static Font CreateHandwritten() {
			Font f = new Font();
			f.CharHeight = 21;

			Sprite spr = Sprite.Load(Windows.TempPath + "\\Handwritten.png");

			using (FileStream stream = File.OpenRead(Windows.TempPath + "\\Handwritten.dat")) {
				using (BinaryReader reader = new BinaryReader(stream)) {
					reader.ReadBytes(16); // Offset 16 + 32
					reader.ReadBytes(33); // bytes into data

					byte[] widths = reader.ReadBytes(96); // widths of 96 ascii chars

					for (char cur = ' '; cur < 128; cur++) {
						byte w = widths[cur - 32];
						Sprite fontChar = new Sprite(w, 21);

						int x = (cur - 32) % 16;
						int y = (cur - 32) / 16;

						for (int i = 0; i < w; i++) {
							for (int j = 0; j < 21; j++) {
								fontChar[i, j] = spr[x * 16 + i, y * 21 + j];
							}
						}

						f.Glyphs.Add(cur, fontChar);
					}
				}
			}

			return f;
		}

		#endregion
	}
}
