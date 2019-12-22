using System;

namespace PixelEngine.Utilities {
	/// <summary> Clock helper for timing things. </summary>
	public class Clock {

		/// <summary> Create a new Clock </summary>
		public Clock() { Last = Start = DateTime.UtcNow; }

		/// <summary> Get starting time of clock. </summary>
		public DateTime Start { get; internal set; }
		/// <summary> Last time this clock was sampled. </summary>
		public DateTime Last { get; internal set; }

		/// <summary> Tick the clock. </summary>
		public void Tick() { Last = DateTime.UtcNow; }

		/// <summary> Get the time elapsed since clock was created. </summary>
		public TimeSpan ElapsedSpan { get { return DateTime.UtcNow - Start; } }
		/// <summary> Get the delta since the <see cref="Last"/> <see cref="Tick"/>. </summary>
		public TimeSpan DeltaSpan { get { return DateTime.UtcNow - Last; } }

		/// <summary> Get the total time in seconds sinc the <see cref="Start"/> of this clock. </summary>
		public float Elapsed { get { return (float) ElapsedSpan.TotalSeconds; } }
		/// <summary> Get the delta time in seconds sinc the <see cref="Last"/> <see cref="Tick"/></summary>
		public float Delta { get { return (float) DeltaSpan.TotalSeconds; } }

	}
}
