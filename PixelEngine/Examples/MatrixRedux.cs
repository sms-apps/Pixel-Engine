using System.Collections.Generic;

using PixelEngine;
using PixelEngine.Utilities;

namespace PixelEngine.Examples {
	/// <summary> Example that lets you see the code, circa 1999. </summary>
	public class MatrixRedux : Game {
		/// <summary> Entry point, formerly Main. </summary>
		public static void Run(string[] args) {
			MatrixRedux game = new MatrixRedux();
			game.Construct(500, 500, 1, 1, 60);
			game.Start();
		}

		private const int MaxStreamers = 300;

		private List<Streamer> streamers;
		private readonly Pixel[][] palettes = new Pixel[][] {
			Greens(17), Greens(23), Greens(29), Greens(31), Greens(37), 
			Greens(41), Greens(43), Greens(47), Greens(53), Greens(59), 
			Greens(61), Greens(67), Greens(71), Greens(73), Greens(79), 
			Greens(83), Greens(89), Greens(97), Greens(101), Greens(103), 
			Greens(107), Greens(109), Greens(113), Greens(127), Greens(131), 
			Greens(137), Greens(139), Greens(149), Greens(151), Greens(163)
		};
		static void PanRight(Pixel[] colors) {
			Pixel temp = colors[colors.Length - 1];
			for (int i = 0; i < colors.Length; i++) {
				Pixel temp2 = colors[i];
				colors[i] = temp;
				temp = temp2;
			}
		}
		static Pixel[] Greens(int num) {
			Pixel[] pixels = new Pixel[num];
			float df = 1.0f / num;
			for (int i = 0; i < num; i++) {
				pixels[i] = Pixel.FromHsv(.33f, 1, df * i);
			}
			return pixels;
		}

		/// <inheritdoc />
		public override void OnCreate() {
			streamers = new List<Streamer>(MaxStreamers);
			for (int n = 0; n < MaxStreamers; n++) {
				Streamer s = new Streamer();
				PrepareStreamer(ref s, Random(0, ScreenHeight * 8 / 6));
				streamers.Add(s);
			}

			foreach (var palette in palettes) {
				// Set last element in each palette to white
				palette[palette.Length-1] = Pixel.Presets.White;
				// prewarm palettes so they don't all harmonize with eachother immediately.
				int shift = Random(0, palette.Length);
				for (int i = 0; i < shift; i++) { PanRight(palette); }
			}
		}

		int panFrames = 8;
		int panFrame = 0;
		/// <inheritdoc />
		public override void OnUpdate(float delta) {
			Clear(Pixel.Presets.Black);
			panFrame++;
			if (panFrame % panFrames == 0) {
				foreach (var palette in palettes) {
					PanRight(palette);
				}
			}

			for (int k = 0; k < streamers.Count; k++) {
				Streamer s = streamers[k];

				s.Position += delta * s.Speed * 10;

				DrawMatrixTextPalette(new Vector2Int(s.Column * 8, (int)s.Position), s.Text, s.Palette);

				for (int i = 0; i < s.Text.Length; i++) {
					// Pixel col = s.Speed > 10 ? Pixel.Presets.Green : Pixel.Presets.DarkGreen;
					// int index = (i - (int)s.Position) % s.Text.Length;
					// DrawText(new Point(s.Column * 8, (int)s.Position - i * 8), s.Text[i].ToString(), col);
					if (Random(1000) < 5) {
						s.Text = s.Text.Remove(i, 1).Insert(i, RandomChar().ToString());
					}

				}

				if (s.Position - s.Text.Length * 8 >= ScreenHeight) {
					PrepareStreamer(ref s, 0);
				}


				streamers[k] = s;
			}
		}

		private char RandomChar() { return (char)Random(32, 128); }

		private void PrepareStreamer(ref Streamer s, int pos) {
			s.Column = Random(ScreenWidth / 8);
			s.Speed = Random(10, 25);
			s.Text = string.Concat(MakeArray(Random(10, 20), i => RandomChar()));
			s.Palette = palettes[Random(0, palettes.Length)];
			s.Position = pos - s.Text.Length * 8 - 10;
		}

		private struct Streamer {
			public int Column { get; set; }
			public float Position { get; set; }
			public float Speed { get; set; }
			public string Text { get; set; }
			public Pixel[] Palette { get; set; }
		}

		void DrawMatrixTextPalette(Vector2Int textPt, string text, Pixel[] palette) {
			int nColors = palette.Length;

			Vector2Int offset = Vector2Int.zero;
			int height = Font.CharHeight;

			for (int i = 0; i < text.Length; i++) {
				char c = text[i];
				int ci = 1 + i % (nColors - 1);
				string str = "" + c;
				offset.y += height;
				offset.x = 0;
				
				DrawText(textPt + offset, str, palette[ci]);
				
			}


		}

	}
}
