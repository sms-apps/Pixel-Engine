using PixelEngine;
using PixelEngine.Extensions.Transforms;

namespace PixelEngine.Examples {
	/// <summary> Example showing matrix transformations. Make sure there is a file 'car.png' in the working directory of the program.
	/// Call <see cref="System.IO.Directory.GetCurrentDirectory"/> and <see cref="System.IO.Directory.SetCurrentDirectory(string)"/>
	/// There is a <see cref="PixelEngine.WindowsInfo.SourceFileDirectory"/> Helper method. </summary>
	public class Transformations : Game {
		private Sprite car;
		private float angle;

		/// <summary> Entry point, formerly Main. </summary>
		public static void Run(string[] args) {
			Transformations t = new Transformations();
			t.Construct(250, 250, 1, 1, 60);
			t.Start();
		}

		/// <inheritdoc />
		public override void OnCreate() {
			car = Sprite.Load("Car.png");
			PixelMode = Pixel.Mode.Alpha;
		}

		/// <inheritdoc />
		public override void OnKeyDown(Key k) {
			switch (k) {
				case Key.Left:
					angle += Clock.Delta;
					break;
				case Key.Right:
					angle -= Clock.Delta;
					break;
			}
		}

		/// <inheritdoc />
		public override void OnUpdate(float elapsed) {
			Clear(Pixel.Presets.Cyan);

			Transform transform = new Transform();
			transform.Translate(-car.Width / 2, -car.Height / 2);
			transform.Rotate(angle);
			transform.Translate(ScreenWidth / 2, ScreenHeight / 2);

			Transform.DrawSprite(car, transform);
		}
	}
}
