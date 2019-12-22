using System;
using PixelEngine;

namespace PixelEngine.Examples {
	/// <summary> Example that uses the audio system to create a beepin tone. </summary>
	public class Beeping : Game {
		/// <summary> Entry point, formerly Main. </summary>
		public static void Run(string[] args) {
			Beeping t = new Beeping(); // Create an instance
			t.Construct(); // Construct the app
			t.Start(); // Start the app
		}

		/// <inheritdoc />
		// Enable the sound system
		public override void OnCreate() { Enable(Subsystem.Audio); }
		
		/// <inheritdoc />
		// This is generating a sin wave
		public override float OnSoundCreate(int channels, float globalTime, float timeStep) { return Sin(globalTime * 440 * PI * 2) / 2; }
		
		/// <inheritdoc />
		// This is zeroing sound at periodic times to make it sound like beeping
		public override float OnSoundFilter(int channels, float globalTime, float sample) { return sample * Sin(globalTime * PI * 2); }
	}
}
