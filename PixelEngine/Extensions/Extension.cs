namespace PixelEngine.Extensions {
	/// <summary> PixelEngine Extension framework base class. </summary>
	public abstract class Extension {
		/// <summary> Statically initialize this extension </summary>
		/// <param name="game"> Game to bind extension to </param>
		internal static void Init(Game game) { Game = game; }

		/// <summary> Bound Game object</summary>
		protected static Game Game { get; private set; }
	}
}
