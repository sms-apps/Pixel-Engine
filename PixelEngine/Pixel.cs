using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelEngine {
	public struct Pixel {
		/// <summary> Workaround to avoid static constructor. </summary>
		public static readonly bool Initialized = Init();
		/// <summary> Workaround to avoid static constructor. </summary>
		private static bool Init() {
			Pixel ToPixel(Presets p) {
				string hex = p.ToString("X");

				byte r = (byte)Convert.ToUInt32(hex.Substring(2, 2), 16);
				byte g = (byte)Convert.ToUInt32(hex.Substring(4, 2), 16);
				byte b = (byte)Convert.ToUInt32(hex.Substring(6, 2), 16);

				return new Pixel(r, g, b);
			}

			Presets[] presets = (Presets[])Enum.GetValues(typeof(Presets));
			presetPixels = presets.ToDictionary(p => p, p => ToPixel(p)); 
			PresetPixels = presetPixels.Values.ToArray();

			return true;
		}

		/// <summary> Red channel </summary>
		public byte R { get; private set; }
		/// <summary> Green channel </summary>
		public byte G { get; private set; }
		/// <summary> Blue channel </summary>
		public byte B { get; private set; }
		/// <summary> Alpha (typically transparency) channel </summary>
		public byte A { get; private set; }

		/// <summary> Create a <see cref="Pixel"/> with the given color information </summary>
		/// <param name="red"> Red channel (0-255) </param>
		/// <param name="green"> Green channel (0-255) </param>
		/// <param name="blue"> Blue channel (0-255) </param>
		/// <param name="alpha"> Alpha (transparency) channel (0-255), Default = 255 </param>
		public Pixel(byte red, byte green, byte blue, byte alpha = 255) {
			R = red; G = green; B = blue; A = alpha;
		}

		/// <summary> Pixel blending modes </summary>
		public enum Mode { Normal, Mask, Alpha, Custom }

		/// <summary> Create a randomly colored, opaque pixel. </summary>
		public static Pixel Random() {
			byte[] vals = Randoms.RandomBytes(3);
			return new Pixel(vals[0], vals[1], vals[2]);
		}
		/// <summary> Create a randomly colored, randomly transparent pixel. </summary>
		public static Pixel RandomAlpha() {
			byte[] vals = Randoms.RandomBytes(4);
			return new Pixel(vals[0], vals[1], vals[2], vals[3]);
		}

		#region Presets
		/// <summary> Color presets. </summary>
		public enum Presets : uint {
			White = 0xffffff,
			Grey = 0xa9a9a9,
			Red = 0xff0000,
			Yellow = 0xffff00,
			Green = 0x00ff00,
			Cyan = 0x00ffff,
			Blue = 0x0000ff,
			Magenta = 0xff00ff,
			Brown = 0x9a6324,
			Orange = 0xf58231,
			Purple = 0x911eb4,
			Lime = 0xbfef45,
			Pink = 0xfabebe,
			Snow = 0xFFFAFA,
			Teal = 0x469990,
			Lavender = 0xe6beff,
			Beige = 0xfffac8,
			Maroon = 0x800000,
			Mint = 0xaaffc3,
			Olive = 0x808000,
			Apricot = 0xffd8b1,
			Navy = 0x000075,
			Black = 0x000000,
			DarkGrey = 0x8B8B8B,
			DarkRed = 0x8B0000,
			DarkYellow = 0x8B8B00,
			DarkGreen = 0x008B00,
			DarkCyan = 0x008B8B,
			DarkBlue = 0x00008B,
			DarkMagenta = 0x8B008B
		}

		/// <summary> Clear pixel (is same as default(Pixel)) </summary>
		public static readonly Pixel Empty = new Pixel(0, 0, 0, 0);

		/// <summary> Dictionary for preset pixels </summary>
		private static Dictionary<Presets, Pixel> presetPixels;
		/// <summary> Array for preset pixels </summary>
		public static Pixel[] PresetPixels { get; private set; }
		#endregion

		/// <summary> Create a color from an integer. </summary>
		/// <param name="rgba">uint value treated as 0xRRGGBBAA </param>
		/// <returns> Pixel representing value of given RGBA </returns>
		public static Pixel FromRgb(uint rgba) {
			byte a = (byte)(rgba & 0xFF);
			byte b = (byte)((rgba >> 8) & 0xFF);
			byte g = (byte)((rgba >> 16) & 0xFF);
			byte r = (byte)((rgba >> 24) & 0xFF);

			return new Pixel(r, g, b, a);
		}
		/// <summary> Create a color from HSV space </summary>
		/// <param name="h"> Hue (Angle of color, [0, 1]) </param>
		/// <param name="s"> Saturation (Grayscale or Vibrant [0, 1]) </param>
		/// <param name="v"> Value (Dark or Bright [0, 1]) </param>
		/// <returns> Pixel representing value of given HSV </returns>
		public static Pixel FromHsv(float h, float s, float v) {
			float c = v * s;
			float nh = (h / 60) % 6;
			float x = c * (1 - Math.Abs(nh % 2 - 1));
			float m = v - c;

			float r, g, b;

			if (0 <= nh && nh < 1) {
				r = c; g = x; b = 0;
			} else if (1 <= nh && nh < 2) {
				r = x; g = c; b = 0;
			} else if (2 <= nh && nh < 3) {
				r = 0; g = c; b = x;
			} else if (3 <= nh && nh < 4) {
				r = 0; g = x; b = c;
			} else if (4 <= nh && nh < 5) {
				r = x; g = 0; b = c;
			} else if (5 <= nh && nh < 6) {
				r = c; g = 0; b = x;
			} else {
				r = 0; g = 0; b = 0;
			}

			r += m; g += m; b += m;

			return new Pixel((byte)Math.Floor(r * 255), (byte)Math.Floor(g * 255), (byte)Math.Floor(b * 255));
		}
		/// <summary> Compare two pixels by all color channels </summary> <returns> True if the two are completely equal, false otherwise </returns>
		public static bool operator ==(Pixel a, Pixel b) {
			return (a.R == b.R) && (a.G == b.G) && (a.B == b.B) && (a.A == b.A);
		}
		/// <summary> Compare two pixels by all color channels </summary> <returns> False if the two are completely equal, true otherwise </returns>
		public static bool operator !=(Pixel a, Pixel b) { return !(a == b); }

		/// <summary> Implicit conversion from a Preset enum value to a Pixel color value. </summary>
		/// <param name="p"> Preset value </param>
		/// <returns> Converted color, or <see cref="Pixel.Empty"/> if invalid. </returns>
		public static implicit operator Pixel(Presets p) {
			if (presetPixels.TryGetValue(p, out Pixel pix)) { return pix; }
			return Empty;
		}

		/// <inheritdoc />
		public override bool Equals(object obj) {
			if (obj is Pixel p) { return this == p; }
			return false;
		}
		/// <inheritdoc />
		public override int GetHashCode() {
			int hashCode = 196078;
			hashCode = hashCode * -152113 + R.GetHashCode();
			hashCode = hashCode * -152113 + G.GetHashCode();
			hashCode = hashCode * -152113 + B.GetHashCode();
			hashCode = hashCode * -152113 + A.GetHashCode();
			return hashCode;
		}
	}
}
