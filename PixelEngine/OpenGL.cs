using System;
using System.Runtime.InteropServices;

using static PixelEngine.Windows;

namespace PixelEngine {
	/// <summary> Class holding rendering hooks and information </summary>
	internal class OpenGL {
		/// <summary> Bound Game instance </summary>
		private Game game;

		/// <summary> Pointer to device's context </summary>
		private IntPtr deviceContext;
		/// <summary> Pointer to rendering context </summary>
		private IntPtr renderContext;

		/// <summary> Pixel size </summary>
		private float pw, ph;
		/// <summary> Inverse float size </summary>
		private float ww, wh;

		/// <summary> Setup in regards to a Game instance </summary>
		/// <param name="game"> Game instance to setup </param>
		public void Create(Game game) {
			this.game = game;

			deviceContext = GetDC(game.Handle);

			PixelFormatDesc pfd = new PixelFormatDesc(1,
				(uint)PFD.DrawToWindow | (uint)PFD.SupportOpenGL | (uint)PFD.DoubleBuffer,
				(byte)PFD.TypeRGBA, 32, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				(sbyte)PFD.MainPlane, 0, 0, 0, 0);

			int pf = ChoosePixelFormat(deviceContext, ref pfd);

			if (pf == 0) { return; }

			SetPixelFormat(deviceContext, pf, ref pfd);

			renderContext = WglCreateContext(deviceContext);
			if (renderContext == IntPtr.Zero) { return; }


			WglMakeCurrent(deviceContext, renderContext);

			uint glBuffer = 0;

			GlEnable((uint)GL.Texture2D);
			GlGenTextures(1, new uint[] { glBuffer });
			GlBindTexture((uint)GL.Texture2D, glBuffer);
			GlTexParameteri((uint)GL.Texture2D, (uint)GL.TextureMagFilter, (int)GL.Nearest);
			GlTexParameteri((uint)GL.Texture2D, (uint)GL.TextureMinFilter, (int)GL.Nearest);
			GlTexEnvf((uint)GL.TextureEnv, (uint)GL.TextureEnvMode, (float)GL.Decal);
		}

		/// <summary> Initialize with sprites to render to </summary>
		/// <param name="drawTarget"> Sprite to render plain draw calls to </param>
		/// <param name="textTarget"> Sprite to render text to </param>
		public unsafe void Initialize(Sprite drawTarget, Sprite textTarget) {
			GlTexImage2D((uint)GL.Texture2D, 0, (uint)GL.RGBA,
				game.ScreenWidth, game.ScreenHeight,
				0, (uint)GL.RGBA, (uint)GL.UnsignedByte, null);

			ww = 1f / game.windowWidth;
			wh = 1f / game.windowHeight;

			pw = game.PixWidth * ww;
			ph = game.PixHeight * wh;

			SetValues(pw, ph, ww, wh);
			CreateCoords(game.PixWidth, game.PixHeight, game.ScreenWidth, game.ScreenHeight);

			IntPtr vsyncPtr = WglGetProcAddress("wglSwapIntervalEXT");
			if (vsyncPtr != IntPtr.Zero) {
				SwapInterval swap = Marshal.GetDelegateForFunctionPointer<SwapInterval>(vsyncPtr);
				swap(0);
			}

			ReleaseDC(game.Handle, deviceContext);
			deviceContext = GetDC(game.Handle);
		}

		/// <summary> Draw given sprites to the screen </summary>
		/// <param name="drawTarget"> Sprite layer </param>
		/// <param name="textTarget"> Text layer </param>
		public unsafe void Draw(Sprite drawTarget, Sprite textTarget) {
			fixed (Pixel* ptr = drawTarget.GetPixels()) {
				if (game.PixWidth == 1 && game.PixHeight == 1) { 
					RenderUnitPixels(drawTarget.Width, drawTarget.Height, ptr);
				} else { RenderPixels(drawTarget.Width, drawTarget.Height, ptr); }
			}

			if (textTarget != null) {
				fixed (Pixel* ptr = textTarget.GetPixels()) {
					RenderText(game.windowWidth, game.windowHeight, textTarget.Width, textTarget.Height, ptr);
				}
			}
			SwapBuffers(deviceContext);
		}

		/// <summary> Releases resources when finished. </summary>
		public void Destroy() {
			ReleaseDC(game.Handle, deviceContext);
			WglDeleteContext(renderContext);
			DestroyCoords();
		}
	}
}
