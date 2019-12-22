using System;

namespace PixelEngine.Utilities {
	/// <summary> Clock helper for timing things. </summary>
	public class Clock {

		internal Clock() { 
			Last = Start = DateTime.UtcNow; 
		}

		/// <summary> Get starting time of clock. </summary>
		public DateTime Start { get; internal set; }
		/// <summary> Last time this clock was sampled. </summary>
		public DateTime Last { get; internal set; }

		/// <summary> Tick the clock. </summary>
		public void Tick() {
			Last = DateTime.UtcNow;
		}

		/// <summary> Get the time elapsed since clock was created. </summary>
		public TimeSpan Elapsed { get { return DateTime.UtcNow - Start; } }
		/// <summary> Get the delta since the <see cref="Last"/> tick. </summary>
		public TimeSpan Delta { get { return DateTime.UtcNow - Last; } }
	}
}
