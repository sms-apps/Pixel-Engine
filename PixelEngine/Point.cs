using PixelEngine.Utilities;

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

		/// <summary> Add two <see cref="Point"/>s component-wise </summary>
		public static Point operator +(Point a, Point b) { return new Point(a.X + b.X, a.Y + b.Y); }
		/// <summary> Subtract two <see cref="Point"/>s component-wise </summary>
		public static Point operator -(Point a, Point b) { return new Point(a.X - b.X, a.Y - b.Y); }
		/// <summary> Multiply two <see cref="Point"/>s component-wise </summary>
		public static Point operator *(Point a, Point b) { return new Point(a.X * b.X, a.Y * b.Y); }
		/// <summary> Divide two <see cref="Point"/>s component-wise </summary>
		public static Point operator /(Point a, Point b) { return new Point(a.X / b.X, a.Y / b.Y); }
		/// <summary> Multiply a <see cref="Point"/> by an <see cref="int"/> </summary>
		public static Point operator *(Point a, int b) { return new Point(a.X * b, a.Y * b); }
		/// <summary> Multiply a <see cref="Point"/> by an <see cref="int"/> </summary>
		public static Point operator *(int b, Point a) { return new Point(a.X * b, a.Y * b); }
		/// <summary> Divide a <see cref="Point"/> by an <see cref="int"/> </summary>
		public static Point operator /(Point a, int b) { return new Point(a.X / b, a.Y / b); }

		/// <summary> Implicitly coerce a <see cref="Vector2"/> into a <see cref="Point"/> by casting its x,y components into <see cref="int"/>s. </summary>
		public static implicit operator Point(Vector2 v) { return new Point((int)v.x, (int)v.y); }
		/// <summary> Implicitly coerce a <see cref="Vector2Int"/> into a <see cref="Point"/> by capturing its x,y components. </summary>
		public static implicit operator Point(Vector2Int v) { return new Point(v.x, v.y); }
		/// <summary> Implicitly coerce a <see cref="Vector3"/> into a <see cref="Point"/> by casting its x,y components into <see cref="int"/>s. </summary>
		public static implicit operator Point(Vector3 v) { return new Point((int)v.x, (int)v.y); }
		/// <summary> Implicitly coerce a <see cref="Vector3Int"/> into a <see cref="Point"/> by capturing its x,y components. </summary>
		public static implicit operator Point(Vector3Int v) { return new Point(v.x, v.y); }
		/// <summary> Implicitly coerce a <see cref="Vector4"/> into a <see cref="Point"/> by casting its x,y components into <see cref="int"/>s. </summary>
		public static implicit operator Point(Vector4 v) { return new Point((int)v.x, (int)v.y); }

	}
}
