using System;

namespace PixelEngine.Utilities {
	public class Clock {
		internal Clock() { Start = DateTime.Now; }

		public DateTime Start { get; internal set; }
		public TimeSpan Total { get { return DateTime.Now - Start; } }
		public TimeSpan Elapsed { get; internal set; }
	}
}
