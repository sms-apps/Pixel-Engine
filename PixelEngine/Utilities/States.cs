using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine.Utilities {
	/// <summary> Helper state machine over an Enum </summary>
	/// <typeparam name="T"> Generic Enum type </typeparam>
	public class StateMachine<T> where T : Enum {
		/// <summary> All possible states. Workaround to avoid static constructor penalty. </summary>
		public static readonly T[] ALL_STATES = Initialize();
		/// <summary> Gets all possible states. Workaround to avoid static constructor penalty. </summary>
		public static T[] Initialize() { return (T[])Enum.GetValues(typeof(T)); }
		/// <summary> Current state </summary>
		public T CurrentState { get; private set; }

		/// <summary> Allowed states </summary>
		private T[] states;
		/// <summary> Transition function </summary>
		private Action<T> transition;

		/// <summary> Create a new state machine that can be in any state of the given Enum </summary>
		public StateMachine() {
			states = ALL_STATES;
			CurrentState = default(T); 
		}
		
		/// <summary> Create a new state machine with a subset of states </summary>
		/// <param name="states"> Subset of states to use </param>
		public StateMachine(params T[] states) {
			if (states.Length > 0) {
				this.states = new T[states.Length];
				states.CopyTo(this.states, 0);
				Switch(states[0]);
			} else {
				states = ALL_STATES;
				CurrentState = default(T);
			}
		}

		/// <summary> Add a transition callback </summary>
		/// <param name="transition"> Callback to add to transition function </param>
		public void OnTransition(Action<T> transition) { this.transition += transition; }

		/// <summary> Switch into a new state, if possible, and invokes transition function </summary>
		/// <param name="newState"> New state to switch into. </param>
		public void Switch(T newState) {
			if (states.Contains(newState)) {
				CurrentState = newState;
				transition?.Invoke(CurrentState);
			}
		}
	}
}
