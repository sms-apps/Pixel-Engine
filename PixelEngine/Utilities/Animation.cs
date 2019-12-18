using System.Threading;
using System.Threading.Tasks;

namespace PixelEngine.Utilities {
	/// <summary> Helper class for animating things. Updates an internal value over time using a <see cref="Task"/> </summary>
	/// <typeparam name="T"> Generic type parameter </typeparam>
	/// <remarks> This is kind of a poor way to animate things, since it will cause more work to be done than is really necessary. </remarks>
	public class Animation<T> {
		/// <summary> Current animation value at given moment </summary>
		public T Value { get; private set; }

		/// <summary> Is the animation running? </summary>
		public bool Running { get; private set; }
		/// <summary> Will the animation loop? </summary>
		public bool Loop { get; set; }

		/// <summary> Was this animation started with a duration? </summary>
		public bool Automatic { get; private set; }

		/// <summary> Values to iterate </summary>
		private T[] values;
		/// <summary> Current index </summary>
		private int index;

		/// <summary> ms interval to update on </summary>
		private int interval;

		public Animation(T[] values) {
			this.values = values;
			Automatic = false;
		}
		public Animation(T[] values, float duration) {
			this.values = values;
			interval = (int)(duration * 1000 / values.Length);
			Automatic = true;
		}

		/// <summary> Start this animation running in a separate task.  </summary>
		public void Start() {
			if (!Running) {
				index = 0;
				Value = values[0];
				Task.Run(Animate);
			}
			Running = true;
		}

		/// <summary> Update this animation manually. Stops running task, if present. </summary>
		public void Update() {
			if (!Running) { return; }

			index++;
			if (index == values.Length) {
				if (Loop) { index = 0; }
				else { Running = false; }
			}
			Value = values[index];
		}

		/// <summary> Stop the task running the animation. </summary>
		public void Stop() { Running = false; }

		/// <summary> Async update for this animation. Runs in its own 'thread' </summary>
		private async Task Animate() {
			while (true) {
				if (!Running) { break; }
				// Awaiting a delay will prevent starving the Task thread pool.
				await Task.Delay(interval);
				index++;
				if (index == values.Length) {
					if (Loop) { index = 0; }
					else { break; }
				}
				Value = values[index];
			}
			Running = false;
		}
	}
}
