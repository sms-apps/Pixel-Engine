namespace PixelEngine {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	/// <summary> Enum for possible keyboard keys </summary>
	public enum Key {
		A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
		K0, K1, K2, K3, K4, K5, K6, K7, K8, K9,
		F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
		F13, F14, F15, F16, F17, F18, F19, F20, F21, F22, F23, F24,

		Up, Down, Left, Right,

		Back, Space, 
		Escape, Enter,
		Tab, Capslock,
		
		Home, End, 
		PageUp, PageDown,
		Insert, Delete, 
		
		LWin, RWin, Apps,
		Shift, LShift, RShift,
		Control, LControl, RControl,
		Alt, LAlt, RAlt,

		Pause, Scroll, PrintScreen,

		Colon,
		Slash, 
		Backtick, 
		OpenBracket, 
		Backslash, 
		CloseBracket,
		Quote, 
		Minus, 
		Comma, 
		Plus, 
		Period, 
		NumLock, Add, 
		Divide, Multiply, Sub,
		Num1, Num2, Num3, 
		Num4, Num5, Num6, 
		Num7, Num8, Num9,
		Num0, Separator,

		Any, None
	}
	/// <summary> Enum for possible mouse buttons </summary>
	public enum Mouse { Left, Middle, Right, Any, None }
	/// <summary> Enum for possible scroll directions </summary>
	public enum Scroll { Up = -1, None = 0, Down = 1 }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
