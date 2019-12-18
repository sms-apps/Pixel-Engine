namespace PixelEngine {
	/// <summary> Holds an x/y pair. </summary>
	public struct Point {
		public Point(int x, int y) : this() {
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
