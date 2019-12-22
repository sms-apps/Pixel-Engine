using PixelEngine;

namespace PixelEngine.Examples {

	/// <summary> Draw a bunch of random colors to the screen every frame </summary>
	public class RandomPixels : Game {

		/// <summary> Entry point, formerly Main. </summary>
		public static void Run(string[] args) {
			// Create an instance
			RandomPixels rp = new RandomPixels();
			rp.Construct(100, 100, 5, 5, 24); // Construct the game
			rp.Start(); // Start and show a window
		}

		/// <inheritdoc />
		public override void OnUpdate(float delta) {
			// Loop through all the pixels
			for (int i = 0; i < ScreenWidth; i++) {
				for (int j = 0; j < ScreenHeight; j++) {
					Draw(i, j, Pixel.Random()); // Draw a random pixel
				}
			}
		}
	}
}
