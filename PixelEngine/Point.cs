namespace PixelEngine {
	/// <summary> Holds an x/y pair. </summary>
	public struct Point {
		/// <summary> Create a <see cref="Point"/> with the given x/y pair </summary>
		/// <param name="x"> x coord </param>
		/// <param name="y"> y coord </param>
		public Point(int x, int y) {
			this.X = x;
			this.Y = y;
		}

		/// <summary> x Coordinate </summary>
		public int X { get; private set; }
		/// <summary> y Coordinate </summary>
		public int Y { get; private set; }

		/// <summary> Always (0, 0) </summary>
		public static Point Origin { get { return new Point(); } }
	}
}
