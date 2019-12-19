using System;
using System.Runtime.InteropServices;
using static PixelEngine.Windows;

namespace PixelEngine {
	/// <summary> PixelEngine base class holding logic for interacting with the display window. </summary>
	public class Display {
		/// <summary> Callback for windows to send the window messages </summary>
		private WindowProcess proc;

		/// <summary> Has the window been initialized yet? </summary>
		private bool init;
		/// <summary> Window title text </summary>
		private string text;

		public Display() {
			proc = WndProc;
		}
		
		/// <summary> Pixels from left to right of screen </summary>
		public int ScreenWidth { get; private protected set; }
		/// <summary> Pixels from top to bottom of screen </summary>
		public int ScreenHeight { get; private protected set; }
		/// <summary> Width per pixel </summary>
		public int PixWidth { get; private protected set; }
		/// <summary> Height per pixel </summary>
		public int PixHeight { get; private protected set; }

		/// <summary> Width of window </summary>
		internal int windowWidth;
		/// <summary> Height of window </summary>
		internal int windowHeight;

		/// <summary> Window title </summary>
		public virtual string AppName {
			get { return text; }
			protected set {
				text = value;
				if (init) {
					SetWindowText(Handle, text);
				}
			}
		}

		/// <summary> Name of overriding class, used as the default window title text. </summary>
		private protected string ClassName { get { return  GetType().FullName; } }

		/// <summary> Area of client window </summary>
		internal WRect ClientRect { get; set; }

		/// <summary> Native pointer to window handle </summary>
		internal IntPtr Handle { get; set; }

		/// <summary> Constructs a window. </summary>
		/// <param name="width"> Width, in pixels. Default = 100 </param>
		/// <param name="height"> Height, in pixels. Default = 100</param>
		/// <param name="pixWidth"> Width in screen pixels of a single game pixel. Default = 5 </param>
		/// <param name="pixHeight"> Height in screen pixels of a single game pixel. Default = 5 </param>
		internal void Construct(int width = 100, int height = 100, int pixWidth = 5, int pixHeight = 5) {
			ScreenWidth = width;
			ScreenHeight = height;
			PixWidth = pixWidth;
			PixHeight = pixHeight;

			windowWidth = ScreenWidth * PixWidth;
			windowHeight = ScreenHeight * PixHeight;
		}

		/// <summary> Start the windows message pump </summary>
		private protected void MessagePump() {
			while (GetMessage(out Message msg, IntPtr.Zero, 0, 0) > 0) {
				TranslateMessage(ref msg);
				DispatchMessage(ref msg);
			}
		}

		/// <summary> Create the window using the winapi </summary>
		private protected virtual void CreateWindow() {
			if (string.IsNullOrWhiteSpace(AppName)) {
				AppName = GetType().Name;
			}
			
			Handle = CreateWindowEx(0, ClassName, AppName, (uint)(WindowStyles.OverlappedWindow | WindowStyles.Visible),
					0, 0, windowWidth, windowHeight, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

			GetClientRect(Handle, out WRect r);
			ClientRect = r;

			init = true;
		}

		/// <summary> Register the class with the winapi </summary>
		private protected virtual void RegisterClass() {
			WindowClassEx wcex = new WindowClassEx() {
				Style = DoubleClicks | VRedraw | HRedraw,
				WindowsProc = proc,
				ClsExtra = 0,
				WndExtra = 0,
				Icon = LoadIcon(IntPtr.Zero, (IntPtr)ApplicationIcon),
				Cursor = LoadCursorA(IntPtr.Zero, (IntPtr)ArrowCursor),
				IconSm = IntPtr.Zero,
				Background = (IntPtr)(ColorWindow + 1),
				MenuName = null,
				ClassName = ClassName
			};
			wcex.Size = (uint)Marshal.SizeOf(wcex);

			RegisterClassEx(ref wcex);
		}

		/// <summary> Windows message processor </summary>
		private protected virtual IntPtr WndProc(IntPtr handle, uint msg, int wParam, int lParam) {
			switch (msg) {
				case (uint)WM.CLOSE:
					DestroyWindow(handle);
					break;
				case (uint)WM.DESTROY:
					PostQuitMessage(0);
					break;
				default:
					return DefWindowProc(handle, msg, wParam, lParam);
			}
			return IntPtr.Zero;
		}
	}
}
