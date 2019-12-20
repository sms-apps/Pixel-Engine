using System;

namespace PixelEngine.Utilities {
	/// <summary> Clock helper for timing things. </summary>
	public class Clock {

		internal Clock() { Start = DateTime.UtcNow; }

		/// <summary> Get starting time of clock. </summary>
		public DateTime Start { get; internal set; }
		/// <summary> Get the time elapsed since clock was created. </summary>
		public TimeSpan Elapsed { get { return DateTime.UtcNow - Start; } }
	}
}
