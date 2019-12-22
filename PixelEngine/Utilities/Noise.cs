using System;

namespace PixelEngine.Utilities {
	/// <summary> Helper class for noise functions </summary>
	public static class Noise {

		/// <summary> Lookup table for permutations. </summary>
		private static readonly byte[] ORIGINAL_PERM = {
			151,160,137,91,90,15,
			131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
			190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
			88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
			77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
			102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
			135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
			5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
			223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
			129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
			251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
			49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
			138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,151
		};
		/// <summary> Lookup table for permutations. </summary>
		private static byte[] perm;

		// Avoiding static constructor. 
		// There is a slight perf penalty when using the class if you actually use a static constructor.
		/// <summary> Workaround for static constructor  </summary>
		public static readonly bool Initialized = Init();
		/// <summary> Workaround for static constructor  </summary> <returns>true</returns>
		public static bool Init() { 
			Seed(); 
			return true; 
		}

		/// <summary> Current offsets </summary>
		private static float xOff, yOff, zOff;
		
		/// <summary> Randomizer </summary>
		private static Random rnd;

		/// <summary> Fractal octaves to apply </summary>
		public static int Octaves { get; set; } = 1;
		/// <summary> Persistance of fractal octaves </summary>
		public static float Persistence { get; set; } = 1;

		/// <summary> Reset the noise </summary>
		public static void Seed() { 
			Seed(Randoms.RandomInt(int.MinValue, int.MaxValue)); 
		}
		/// <summary> Reset the noise with a given seed </summary>
		public static void Seed(int seed) {
			rnd = new Random(seed);
			perm = new byte[ORIGINAL_PERM.Length];
			// Reset perm before swapping
			Array.Copy(ORIGINAL_PERM, perm, perm.Length);

			for (int i = 0; i < perm.Length; i++) {
				int r = rnd.Next(i, perm.Length);

				int temp = perm[i];
				perm[i] = perm[r];
				perm[r] = (byte)temp;
			}

			CreateOffset();
		}
		/// <summary> 1d noise sample </summary>
		/// <param name="x"> Coordinate of sample </param>
		/// <returns> Output of Perlin sample </returns>
		private static float Sample(float x) {
			int X = (int)Math.Floor(x) & 0xff;
			x -= (float)Math.Floor(x);
			float u = Fade(x);
			return Lerp(u, Grad(perm[X], x), Grad(perm[X + 1], x - 1)) * 2;
		}
		/// <summary> 2d noise sample </summary>
		/// <param name="x"> x coordinate of sample </param>
		/// <param name="y"> y coordinate of sample </param>
		/// <returns> Output of Perlin sample </returns>
		private static float Sample(float x, float y) {
			int X = (int)Math.Floor(x) & 0xff;
			int Y = (int)Math.Floor(y) & 0xff;
			x -= (float)Math.Floor(x);
			y -= (float)Math.Floor(y);
			float u = Fade(x);
			float v = Fade(y);
			int A = (perm[X] + Y) & 0xff;
			int B = (perm[X + 1] + Y) & 0xff;
			return Lerp(v, Lerp(u, Grad(perm[A], x, y), Grad(perm[B], x - 1, y)),
						   Lerp(u, Grad(perm[A + 1], x, y - 1), Grad(perm[B + 1], x - 1, y - 1)));
		}
		/// <summary> 3d noise sample </summary>
		/// <param name="x"> x coordinate of sample </param>
		/// <param name="y"> y coordinate of sample </param>
		/// <param name="z"> z coordinate of sample </param>
		private static float Sample(float x, float y, float z) {
			int X = (int)Math.Floor(x) & 0xff;
			int Y = (int)Math.Floor(y) & 0xff;
			int Z = (int)Math.Floor(z) & 0xff;
			x -= (float)Math.Floor(x);
			y -= (float)Math.Floor(y);
			z -= (float)Math.Floor(z);
			float u = Fade(x);
			float v = Fade(y);
			float w = Fade(z);
			int A = (perm[X] + Y) & 0xff;
			int B = (perm[X + 1] + Y) & 0xff;
			int AA = (perm[A] + Z) & 0xff;
			int BA = (perm[B] + Z) & 0xff;
			int AB = (perm[A + 1] + Z) & 0xff;
			int BB = (perm[B + 1] + Z) & 0xff;
			return Lerp(w, Lerp(v, Lerp(u, Grad(perm[AA], x, y, z), Grad(perm[BA], x - 1, y, z)),
								   Lerp(u, Grad(perm[AB], x, y - 1, z), Grad(perm[BB], x - 1, y - 1, z))),
						   Lerp(v, Lerp(u, Grad(perm[AA + 1], x, y, z - 1), Grad(perm[BA + 1], x - 1, y, z - 1)),
								   Lerp(u, Grad(perm[AB + 1], x, y - 1, z - 1), Grad(perm[BB + 1], x - 1, y - 1, z - 1))));
		}
		/// <summary> 1d Fractal Brownian Motion noise </summary>
		/// <param name="x"> coordinate of sample </param>
		/// <returns> Output of fractal noise sample </returns>
		public static float FBM(float x) {
			x += xOff;

			float total = 0.0f;
			float amplitude = 1;
			float totalAmp = 0;
			int frequency = 1;

			for (int i = 0; i < Octaves; i++) {
				total += amplitude * Sample(x * frequency);
				frequency = 1 << i;
				totalAmp += amplitude;
				amplitude *= Persistence;
			}

			return total / totalAmp;
		}
		/// <summary> 2d Fractal Brownian Motion noise </summary>
		/// <param name="x"> x coordinate of sample </param>
		/// <param name="y"> y coordinate of sample </param>
		/// <returns> Output of fractal noise sample </returns>
		public static float FBM(float x, float y) {
			x += xOff;
			y += yOff;

			float total = 0.0f;
			float amplitude = 1;
			float totalAmp = 0;
			int frequency = 1;

			for (int i = 0; i < Octaves; i++) {
				total += amplitude * Sample(x * frequency, y * frequency);
				frequency = 1 << i;
				totalAmp += amplitude;
				amplitude *= Persistence;
			}

			return total / totalAmp;
		}
		/// <summary> 3d Fractal Brownian Motion noise </summary>
		/// <param name="x"> x coordinate of sample </param>
		/// <param name="y"> y coordinate of sample </param>
		/// <param name="z"> z coordinate of sample </param>
		/// <returns> Output of fractal noise sample </returns>
		public static float FBM(float x, float y, float z) {
			x += xOff;
			y += yOff;
			z += zOff;

			float total = 0.0f;
			float amplitude = 1;
			float totalAmp = 0;
			int frequency = 1;

			for (int i = 0; i < Octaves; i++) {
				total += amplitude * Sample(x * frequency, y * frequency, z * frequency);
				frequency = 1 << i;
				totalAmp += amplitude;
				amplitude *= Persistence;
			}

			return total / totalAmp;
		}

		#region Helpers
		/// <summary> Initialize offsets to (0, 100) </summary>
		private static void CreateOffset() {
			xOff = (float)rnd.NextDouble() * 100;
			yOff = (float)rnd.NextDouble() * 100;
			zOff = (float)rnd.NextDouble() * 100;
		}

		/// <summary> Gradiant fading for a given sample </summary>
		/// <param name="t"> Sample to fade </param>
		/// <returns> Amount to blend between neighboring samples </returns>
		private static float Fade(float t) { return t * t * t * (t * (t * 6 - 15) + 10); }
		/// <summary> Linear Interpolation </summary>
		/// <param name="t"> Proportion to interpolate between left/right values [0..1]</param>
		/// <param name="a"> "Left" value </param>
		/// <param name="b"> "Right" value </param>
		/// <returns> Interpolated value on line between <paramref name="a"/> and <paramref name="b"/> </returns>
		private static float Lerp(float t, float a, float b) { return a + t * (b - a); }

		/// <summary> 1d gradiant sample </summary>
		/// <param name="hash"> 'random' hash </param>
		/// <param name="x"> x position </param>
		/// <returns> Gradient sample for 1d noise </returns>
		private static float Grad(int hash, float x) { return (hash & 1) == 0 ? x : -x; }
		/// <summary> 2d gradiant sample </summary>
		/// <param name="hash"> 'random' hash </param>
		/// <param name="x"> x position </param>
		/// <param name="y"> y position </param>
		/// <returns> Gradient sample for 2d noise </returns>
		private static float Grad(int hash, float x, float y) { return ((hash & 1) == 0 ? x : -x) + ((hash & 2) == 0 ? y : -y); }
		/// <summary> 3d gradiant sample </summary>
		/// <param name="hash"> 'random' hash </param>
		/// <param name="x"> x position </param>
		/// <param name="y"> y position </param>
		/// <param name="z"> z position </param>
		/// <returns> Gradient sample for 3d noise </returns>
		private static float Grad(int hash, float x, float y, float z) {
			int h = hash & 15;
			float u = h < 8 ? x : y;
			float v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
			return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
		}


		#endregion
	}
}
