namespace PixelEngine {
	/// <summary> Struct representing a snapshot of the state of an input key. </summary>
	public struct Input {
		/// <summary> Data byte... Still a bit wasteful, but less than before. </summary>
		/// <remarks> With 3 bool fields, an Input was 12 bytes before, now it is just one. </remarks>
		private byte data;
		/// <summary> Mask for pressed bit </summary>
		private const byte PRESSED = 0x01;
		/// <summary> Mask for released bit </summary>
		private const byte RELEASED = 0x02;
		/// <summary> Mask for down bit </summary>
		private const byte DOWN = 0x04;
		
		/// <summary> Is the key pressed this frame? </summary>
		public bool Pressed { get { return (data & PRESSED) > 0; } internal set { data = (byte) (value ? (data | PRESSED) : (data & ~PRESSED)); } }
		/// <summary> Is the key released this frame? </summary>
		public bool Released { get { return (data & RELEASED) > 0; } internal set { data = (byte) (value ? (data | RELEASED) : (data & ~RELEASED)); } }
		/// <summary> Is the key held down? </summary>
		public bool Down { get { return (data & DOWN) > 0; } internal set { data = (byte)(value ? (data | DOWN) : (data & ~DOWN)); } }
		/// <summary> Is the key not held down? </summary>
		public bool Up { get { return !Down; } }
	}
}
