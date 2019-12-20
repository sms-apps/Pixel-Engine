namespace PixelEngine {
	/// <summary> Helper class for wrapping a <see cref="ShaderFunc"/>. </summary>
	public class Shader {
		/// <summary> Wrapped <see cref="ShaderFunc"/> </summary>
		public ShaderFunc Calculate { get; private set; }
		/// <summary> Wrap a <see cref="ShaderFunc"/> </summary>
		public Shader(ShaderFunc calculate) { Calculate = calculate; }
	}
	/// <summary> Delegate class for applying a shader-function per-pixel. </summary>
	/// <param name="x"> x Coord of pixel </param>
	/// <param name="y"> y Coord of pixel </param>
	/// <param name="prev"> Last pixel value </param>
	/// <param name="cur"> Input pixel value </param>
	/// <returns> Color of pixel with regards to location/previous </returns>
	public delegate Pixel ShaderFunc(int x, int y, Pixel prev, Pixel cur);
}
