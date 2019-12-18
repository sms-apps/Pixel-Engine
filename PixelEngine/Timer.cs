using System;

namespace PixelEngine {
	/// <summary> Helper class for calculating ticks over time at different intervals </summary>
	internal class Timer {
		/// <summary> Create a timer that ticks at the given interval </summary>
		/// <param name="interval"> Milliseconds per tick </param>
		public Timer(float interval) { Interval = interval; }

		/// <summary> Milliseconds per tick </summary>
		public float Interval { get; private set; }

		/// <summary> Timestamp of last tick </summary>
		private DateTime last;

		/// <summary> Update timer and see if a tick-point was passed. </summary>
		/// <returns> True if a tick was passed, false otherwise </returns>
		public bool Tick() {
			if ((DateTime.UtcNow - last).TotalMilliseconds >= Interval) {
				last = DateTime.UtcNow;
				return true;
			}
			return false;
		}

		/// <summary> Initialize this timer to the current time </summary>
		public void Init() { last = DateTime.UtcNow; }
		/// <summary> Initialize this timer to the given time </summary>
		public void Init(DateTime time) { last = time; }
	}
}
