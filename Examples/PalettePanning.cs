using PixelEngine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine.Examples {
	/// <summary> Example class containing Palette panning example </summary>
	public class PalettePanning : Game {
		/// <summary> Entry point. </summary>
		public static void Run(string[] args) {
			var game = new PalettePanning();
			game.Construct(400, 400, 2, 2, 60);
			
			game.Start();
		}
		
		static Pixel[] Rainbow(int num) {
			Pixel[] pixels = new Pixel[num];
			float dh = 1.0f / num;
			for (int i = 0; i < num; i++) {
				pixels[i] = Pixel.FromHsv(dh*i, 1, 1);
			}
			return pixels;
		}
		static Pixel[] Reds(int num) {
			Pixel[] pixels = new Pixel[num];
			float df = 1.0f / num;
			for (int i = 0; i < num; i++) {
				pixels[i] = Pixel.FromHsv(0, 1, df * i);
			}
			return pixels;
		}
		static Pixel[] Greens(int num) {
			Pixel[] pixels = new Pixel[num];
			float df = 1.0f / num;
			for (int i = 0; i < num; i++) {
				pixels[i] = Pixel.FromHsv(.33f, 1, df * i);
			}
			return pixels;
		}
		static Pixel[] Blues(int num) {
			Pixel[] pixels = new Pixel[num];
			float df = 1.0f / num;
			for (int i = 0; i < num; i++) {
				pixels[i] = Pixel.FromHsv(.66f, 1, df * i);
			}
			return pixels;
		}
		static void PanRight(Pixel[] colors) {
			Pixel temp = colors[colors.Length-1];
			for (int i = 0; i < colors.Length; i++) {
				Pixel temp2 = colors[i];
				colors[i] = temp;
				temp = temp2;
			}
		}
		static void PanLeft(Pixel[] colors) {
			Pixel temp = colors[0];
			for (int i = colors.Length-1; i >= 0; i--) {
				Pixel temp2 = colors[i];
				colors[i] = temp;
				temp = temp2;
			}
		}
		PalettedSprite cursor;
		Pixel[] palette;
		Func<int, Pixel[]>[] paletteGens = { Rainbow, Reds, Blues, Greens };

		int size = 63;
		int nColors = 32;

		int paletteId = 0;

		/// <inheritdoc />
		public PalettePanning() {
			Rebuild();


		}

		void Rebuild() {
			palette = paletteGens[paletteId](nColors);

			// If we change number of colors in palette, we need to rebuild the sprite as well...
			// 0 is fixed to be transparent, so with 32 colors we can only use 31 of them. 
			// a new PalettedSprite is initialized to be filled with transparent pixels.
			cursor = new PalettedSprite(size, size, palette);

			// Draw two lines into the paletted sprite
			for (int i = 0; i < size; i++) {
				// Create two color indexes into the palette
				// Don't set any zeros into the sprite (zeros are transparent!)
				int ci1 = 1 + (i % (nColors - 1));
				int ci2 = 1 + ((i + nColors / 2) % (nColors - 1));
				cursor.SetIndex(size / 2, i, ci1);
				cursor.SetIndex(i, size / 2, ci2);
			}

			// Remove some of the center colors by setting them to be transparent.
			cursor.SetIndex(size / 2, size / 2, 0);
			cursor.SetIndex(size / 2 - 1, size / 2, 0);
			cursor.SetIndex(size / 2 - 2, size / 2, 0);
			cursor.SetIndex(size / 2 + 1, size / 2, 0);
			cursor.SetIndex(size / 2 + 2, size / 2, 0);
			cursor.SetIndex(size / 2, size / 2 - 1, 0);
			cursor.SetIndex(size / 2, size / 2 - 2, 0);
			cursor.SetIndex(size / 2, size / 2 + 1, 0);
			cursor.SetIndex(size / 2, size / 2 + 2, 0);
		}

		void UpdatePalette() {
			// We don't need to change the sprite, just update the palette for it.
			palette = paletteGens[paletteId](nColors);
			// Create a new cursor and discard the old one
			cursor = new PalettedSprite(cursor, palette);
		}


		void RainbowifyText(Vector2Int textPt, string text) {
			Vector2Int offset = Vector2Int.zero;
			int height = Font.CharHeight;

			for (int i = 0; i < text.Length; i++) {
				char c = text[i];
				int ci = 1 + i % (nColors - 1);
				string str = "" + c;
				if (c == '\n') {
					offset.x = 0;
					offset.y += height;
				} else {
					DrawText(textPt + offset, str, palette[ci]);
					offset.x += Font.TextWidth(str);
				}
			}


		}



		/// <inheritdoc />
		public override void OnUpdate(float delta) {
			Clear(Pixel.Presets.Black);
			
			if (GetKey(Key.Shift).Down) { PanLeft(palette); } else { PanRight(palette); }
			if (GetKey(Key.K1).Pressed) { paletteId = 0; UpdatePalette(); }
			if (GetKey(Key.K2).Pressed) { paletteId = 1; UpdatePalette(); }
			if (GetKey(Key.K3).Pressed) { paletteId = 2; UpdatePalette(); }
			if (GetKey(Key.K4).Pressed) { paletteId = 3; UpdatePalette(); }
			if (GetKey(Key.Q).Pressed) { nColors = 16; Rebuild(); }
			if (GetKey(Key.W).Pressed) { nColors = 32; Rebuild(); }
			if (GetKey(Key.E).Pressed) { nColors = 64; Rebuild(); }
			if (GetKey(Key.R).Pressed) { nColors = 128; Rebuild(); }
			


			Point mp = new Point(MouseX, MouseY) - cursor.Size() / 2;
			DrawSprite(mp, cursor);


			Font = Font.Presets.Retro;
			RainbowifyText(Vector2Int.zero, "Cursor drawn at mouse.\nPress shift to reverse palette panning direction");
			RainbowifyText(new Vector2Int(0, ScreenHeight-Font.CharHeight*2), "Press 1-4 to swap palettes\nPress Q-W-E-R to change palette size.");
		}

	}
}
