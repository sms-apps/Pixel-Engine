using System;
using System.Collections.Generic;

namespace PixelEngine {
	internal static class Randoms {
		/// <summary> Workaround to avoid static constructor penalty </summary>
		public static bool Initialized = Init();
		/// <summary> Workaround to avoid static constructor penalty </summary>
		private static bool Init() {
			Seed = Environment.TickCount;
			return true;
		}
		/// <summary> Current random state. </summary>
		private static Random rnd = new Random();
		/// <summary> Last random seed used to initialize with </summary>
		private static int seed;

		/// <summary> Change the random seed. </summary>
		public static int Seed { get { return seed; } set { seed = value; rnd = new Random(value); } }

		/// <summary> Get a single random value between [0, 255] </summary>
		/// <returns> Single number between 0 and 255 </returns>
		public static byte RandomByte() { return (byte)rnd.Next(255); }

		/// <summary> Get some number of random byte values </summary>
		/// <param name="count"> Number of bytes to get </param>
		/// <returns> Array of <paramref name="count"/> random bytes </returns>
		public static byte[] RandomBytes(int count) {
			byte[] b = new byte[count];
			rnd.NextBytes(b);
			return b;
		}

		/// <summary> Get a single random int between [<paramref name="min" />, <paramref name="max"/>) </summary>
		/// <param name="min"> Min int value </param>
		/// <param name="max"> Max int value </param>
		/// <returns> Value at least equal to <paramref name="min"/>, up to but not including <paramref name="max"/>. </returns>
		public static int RandomInt(int min, int max) { return rnd.Next(min, max); }

		/// <summary> Get a single random float between [<paramref name="min" />, <paramref name="max"/>) </summary>
		/// <param name="min"> Min float value </param>
		/// <param name="max"> Max float value </param>
		/// <returns> Value at least equal to <paramref name="min"/>, up to but not including <paramref name="max"/>. </returns>
		public static float RandomFloat(float min = 0, float max = 1) { return (float)rnd.NextDouble() * (max - min) + min; }
	}
}
