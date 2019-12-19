using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine.Utilities {

	/// <summary> Backed by a single <see cref="int"/> to provide 32 <see cref="bool"/>s under one name. </summary>
	public struct IntFlags {
		/// <summary> Operator to convert <see cref="int"/> to <see cref="IntFlags"/> for easy assignment </summary>
		public static implicit operator IntFlags(int val) { IntFlags flag = new IntFlags(); flag.data = val; return flag; }
		/// <summary> Operator to convert <see cref="IntFlags"/> to <see cref="int"/> for easy assignment </summary>
		public static implicit operator int(IntFlags val) { return val.data; }
		/// <summary> Data backing this struct instance </summary>
		private int data;
		/// <summary> Size of a single int in bits </summary>
		private static readonly int SIZE = 8 * sizeof(int);
		/// <summary> Indexer into packed bits </summary>
		public bool this[int index] {
			get {
				if (index < 0 || index >= SIZE) { throw new IndexOutOfRangeException($"IntFlags may only be indexed from [0, {SIZE-1}]."); }
				return (data & (1 << index)) != 0;
			}
			set {
				if (index < 0 || index >= SIZE) { throw new IndexOutOfRangeException($"IntFlags may only be indexed from [0, {SIZE-1}]."); }
				int mask = 1 << index;
				data = value ? (data | mask) : (data & ~mask);
			}
		}
	}
	/// <summary> Backed by a single <see cref="long"/> to provide 64 <see cref="bool"/>s under one name. </summary>
	public struct LongFlags {
		/// <summary> Operator to convert <see cref="long"/> to <see cref="LongFlags"/> for easy assignment </summary>
		public static implicit operator LongFlags(long val) { LongFlags flag = new LongFlags(); flag.data = val; return flag; }
		/// <summary> Operator to convert <see cref="LongFlags"/> to <see cref="long"/> for easy assignment </summary>
		public static implicit operator long(LongFlags val) { return val.data; }
		/// <summary> Data backing this struct instance </summary>
		private long data;
		/// <summary> Size of a single long in bits </summary>
		private static readonly int SIZE = 8 * sizeof(long);
		/// <summary> Indexer into packed bits </summary>
		public bool this[int index] {
			get {
				if (index < 0 || index >= SIZE) { throw new IndexOutOfRangeException($"LongFlags may only be indexed from [0, {SIZE - 1}]."); }
				return (data & (1L << index)) != 0;
			}
			set {
				if (index < 0 || index >= SIZE) { throw new IndexOutOfRangeException($"LongFlags may only be indexed from [0, {SIZE - 1}]."); }
				long mask = 1L << index;
				data = value ? (data | mask) : (data & ~mask);
			}
		}
	}

	/// <summary> Highly packed bitflags for large numbers of flags. 8 Times more efficient packing than <see cref="bool[]"/>. </summary>
	public class Bitflags {

		/// <summary> Array containing flags </summary>
		private List<int> flags;
		/// <summary> Maximum amount of individual bitflags that can be indexed </summary>
		public int Capacity { get; private set; }
		/// <summary> Gets the number of int sized blocks in this bitflags </summary>
		public int Blocks { get { return Capacity / BLOCKSIZE; } }
		/// <summary> Size of a single int block </summary>
		public static readonly int BLOCKSIZE = 8 * sizeof(int);

		/// <summary> Creates a copy of the flags </summary>
		/// <returns> Array of ints containing the flag data. </returns>
		public int[] CopyFlags() {
			int[] copy = new int[flags.Count];
			for (int i = 0; i < copy.Length; i++) { copy[i] = flags[i]; }
			return copy;
		}

		/// <summary> Constructs a bitflags with at least <paramref name="numFlags"/> in capacity. </summary>
		/// <param name="numFlags"> Number of required flags </param>
		public Bitflags(int numFlags) {
			int size = numFlags / BLOCKSIZE + (numFlags % BLOCKSIZE == 0 ? 0 : 1);
			Capacity = size * BLOCKSIZE;
			flags = new List<int>(size);
			for (int i = 0; i < size; i++) { flags.Add(0); }
		}
		/// <summary> Constructs a bitflags with a copy of the given flags </summary>
		/// <param name="data"> Flags to copy </param>
		public Bitflags(int[] data) {
			Capacity = data.Length * BLOCKSIZE;
			flags = new List<int>(Capacity / BLOCKSIZE);
			for (int i = 0; i < data.Length; i++) { flags.Add(data[i]); }
		}

		/// <summary> Expand the Bitflags by one block </summary>
		public void Expand() {
			flags.Add(0);
			Capacity += BLOCKSIZE;
		}

		/// <summary> Might expand the bitflags. Expands if just one more block is needed to have <paramref name="desiredIndex"/> within <see cref="Capacity"/>. </summary>
		/// <param name="desiredIndex"> Index desired to access </param>
		/// <returns> True, if <paramref name="desiredIndex"/> is now valid, false otherwise. </returns>
		public bool MaybeExpand(int desiredIndex) {
			if (desiredIndex < Capacity) { return true; }
			if (desiredIndex < Capacity + BLOCKSIZE) {
				Expand();
				return true;
			}
			return false;
		}

		/// <summary> Gets the int block at the given index in the underlying array. </summary>
		/// <param name="index"> Index to get. Must be inside [0, <see cref="Blocks"/>-1] </param>
		/// <returns> int value at block </returns>
		public int InspectBlock(int index) { return flags[index]; }
		/// <summary> Sets the int block at the given index in the underlying array. </summary>
		/// <param name="index"> Index to set. Must be inside [0, <see cref="Blocks"/>-1] </param>
		/// <param name="value"> int value to set at block </param>
		public void UpdateBlock(int index, int value) { flags[index] = value; }

		/// <summary> Gets or updates a single flag  </summary>
		/// <param name="index"> Absolute index of bit to set or get </param>
		/// <returns> bit value at index </returns>
		public bool this[int index] {
			get {
				if (index < 0 || index >= Capacity) { throw new IndexOutOfRangeException(); }
				int pos = index / BLOCKSIZE;
				int bit = index % BLOCKSIZE;

				return (flags[pos] & (1 << bit)) != 0;
			}
			set {
				if (index < 0 || index >= Capacity) { throw new IndexOutOfRangeException(); }
				int pos = index / BLOCKSIZE;
				int bit = index % BLOCKSIZE;
				int mask = 1 << bit;

				int bits = flags[pos];
				if (value) {
					bits |= mask;
				} else {
					bits &= (~mask);
				}
				flags[pos] = bits;
			}
		}
	}
}
