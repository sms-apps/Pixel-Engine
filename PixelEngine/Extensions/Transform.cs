using System;

namespace PixelEngine.Extensions.Transforms {
	/// <summary> Example Extension for `Examples.Transformations`.</summary>
	public class Transform : Extension {
		/// <summary> Initialize the extension by creating 4 3x3 matricies. </summary>
		public Transform() {
			matrix = new float[4, 3, 3];
			Reset();
		}

		#region Operations
		/// <summary> Resets the internal state back to identity matricies (no transformation). </summary>
		public void Reset() {
			// Reset source/dest matrix indexes 
			targetMat = 0;
			sourceMat = 1;
			inverseDirty = true;

			matrix[0, 0, 0] = 1.0f; matrix[0, 1, 0] = 0.0f; matrix[0, 2, 0] = 0.0f;
			matrix[0, 0, 1] = 0.0f; matrix[0, 1, 1] = 1.0f; matrix[0, 2, 1] = 0.0f;
			matrix[0, 0, 2] = 0.0f; matrix[0, 1, 2] = 0.0f; matrix[0, 2, 2] = 1.0f;

			matrix[1, 0, 0] = 1.0f; matrix[1, 1, 0] = 0.0f; matrix[1, 2, 0] = 0.0f;
			matrix[1, 0, 1] = 0.0f; matrix[1, 1, 1] = 1.0f; matrix[1, 2, 1] = 0.0f;
			matrix[1, 0, 2] = 0.0f; matrix[1, 1, 2] = 0.0f; matrix[1, 2, 2] = 1.0f;
		}

		/// <summary> Applies a rotation to the current state  </summary>
		/// <param name="angle"> Angle, in radians, to rotate by. </param>
		public void Rotate(float angle) {
			matrix[TEMP_MATRIX, 0, 0] = (float)Math.Cos(angle); matrix[TEMP_MATRIX, 1, 0] = (float)Math.Sin(angle); matrix[TEMP_MATRIX, 2, 0] = 0.0f;
			matrix[TEMP_MATRIX, 0, 1] = -(float)Math.Sin(angle); matrix[TEMP_MATRIX, 1, 1] = (float)Math.Cos(angle); matrix[TEMP_MATRIX, 2, 1] = 0.0f;
			matrix[TEMP_MATRIX, 0, 2] = 0.0f; matrix[TEMP_MATRIX, 1, 2] = 0.0f; matrix[TEMP_MATRIX, 2, 2] = 1.0f;
			Multiply();
		}

		/// <summary> Applies a translation by the given x/y coords to the current state </summary>
		/// <param name="ox"> X to translate by </param>
		/// <param name="oy"> Y to translate by </param>
		public void Translate(float ox, float oy) {
			matrix[TEMP_MATRIX, 0, 0] = 1.0f; matrix[TEMP_MATRIX, 1, 0] = 0.0f; matrix[TEMP_MATRIX, 2, 0] = ox;
			matrix[TEMP_MATRIX, 0, 1] = 0.0f; matrix[TEMP_MATRIX, 1, 1] = 1.0f; matrix[TEMP_MATRIX, 2, 1] = oy;
			matrix[TEMP_MATRIX, 0, 2] = 0.0f; matrix[TEMP_MATRIX, 1, 2] = 0.0f; matrix[TEMP_MATRIX, 2, 2] = 1.0f;
			Multiply();
		}

		/// <summary> Applies a scale by the given x/y coords, to the current state</summary>
		/// <param name="sx"> X to scale by </param>
		/// <param name="sy"> Y to scale by </param>
		public void Scale(float sx, float sy) {
			matrix[TEMP_MATRIX, 0, 0] = sx; matrix[TEMP_MATRIX, 1, 0] = 0.0f; matrix[TEMP_MATRIX, 2, 0] = 0.0f;
			matrix[TEMP_MATRIX, 0, 1] = 0.0f; matrix[TEMP_MATRIX, 1, 1] = sy; matrix[TEMP_MATRIX, 2, 1] = 0.0f;
			matrix[TEMP_MATRIX, 0, 2] = 0.0f; matrix[TEMP_MATRIX, 1, 2] = 0.0f; matrix[TEMP_MATRIX, 2, 2] = 1.0f;
			Multiply();
		}

		/// <summary> Applies a shear by the given x/y coords to the current state </summary>
		/// <param name="sx"> X to shear by </param>
		/// <param name="sy"> Y to shear by </param>
		public void Shear(float sx, float sy) {
			matrix[TEMP_MATRIX, 0, 0] = 1.0f; matrix[TEMP_MATRIX, 1, 0] = sx; matrix[TEMP_MATRIX, 2, 0] = 0.0f;
			matrix[TEMP_MATRIX, 0, 1] = sy; matrix[TEMP_MATRIX, 1, 1] = 1.0f; matrix[TEMP_MATRIX, 2, 1] = 0.0f;
			matrix[TEMP_MATRIX, 0, 2] = 0.0f; matrix[TEMP_MATRIX, 1, 2] = 0.0f; matrix[TEMP_MATRIX, 2, 2] = 1.0f;
			Multiply();
		}

		/// <summary> Draws a sprite, using the current transform state of the given <see cref="Transform"/> </summary>
		/// <param name="spr"> <see cref="ISprite"/> to draw </param>
		/// <param name="transform"> Transformation to apply </param>
		public static void DrawSprite(ISprite spr, Transform transform) {
			if (spr == null) { return; }

			float ex = 0, ey = 0;
			float px, py;
			float sx, sy;

			transform.Forward(0.0f, 0.0f, out sx, out sy);
			px = ex = sx; py = ey = sy;

			transform.Forward(spr.Width, spr.Height, out px, out py);
			sx = Math.Min(sx, px);
			sy = Math.Min(sy, py);
			ex = Math.Max(ex, px);
			ey = Math.Max(ey, py);

			transform.Forward(0.0f, spr.Height, out px, out py);
			sx = Math.Min(sx, px);
			sy = Math.Min(sy, py);
			ex = Math.Max(ex, px);
			ey = Math.Max(ey, py);

			transform.Forward(spr.Width, 0.0f, out px, out py);
			sx = Math.Min(sx, px);
			sy = Math.Min(sy, py);
			ex = Math.Max(ex, px);
			ey = Math.Max(ey, py);

			if (transform.inverseDirty) {
				transform.Invert();
				transform.inverseDirty = false;
			}

			if (ex < sx) {
				float t = sx;
				sx = ex;
				ex = t;
			}

			if (ey < sy) {
				float t = sy;
				sy = ey;
				ey = t;
			}

			for (float i = sx; i < ex; i++) {
				for (float j = sy; j < ey; j++) {
					float ox, oy;
					transform.Backward(i, j, out ox, out oy);
					Game.Draw((int)i, (int)j, spr[(int)(ox + 0.5f), (int)(oy + 0.5f)]);
				}
			}
		}
		#endregion

		#region Helpers
		/// <summary> Sets the 4th matrix to be the inverse of the source matrix. </summary>
		private void Invert() {
			float det = matrix[sourceMat, 0, 0] * (matrix[sourceMat, 1, 1] * matrix[sourceMat, 2, 2] - matrix[sourceMat, 1, 2] * matrix[sourceMat, 2, 1]) -
						matrix[sourceMat, 1, 0] * (matrix[sourceMat, 0, 1] * matrix[sourceMat, 2, 2] - matrix[sourceMat, 2, 1] * matrix[sourceMat, 0, 2]) +
						matrix[sourceMat, 2, 0] * (matrix[sourceMat, 0, 1] * matrix[sourceMat, 1, 2] - matrix[sourceMat, 1, 1] * matrix[sourceMat, 0, 2]);

			float idet = 1 / det;

			matrix[INVERSE_MATRIX, 0, 0] = (matrix[sourceMat, 1, 1] * matrix[sourceMat, 2, 2] - matrix[sourceMat, 1, 2] * matrix[sourceMat, 2, 1]) * idet;
			matrix[INVERSE_MATRIX, 1, 0] = (matrix[sourceMat, 2, 0] * matrix[sourceMat, 1, 2] - matrix[sourceMat, 1, 0] * matrix[sourceMat, 2, 2]) * idet;
			matrix[INVERSE_MATRIX, 2, 0] = (matrix[sourceMat, 1, 0] * matrix[sourceMat, 2, 1] - matrix[sourceMat, 2, 0] * matrix[sourceMat, 1, 1]) * idet;
			matrix[INVERSE_MATRIX, 0, 1] = (matrix[sourceMat, 2, 1] * matrix[sourceMat, 0, 2] - matrix[sourceMat, 0, 1] * matrix[sourceMat, 2, 2]) * idet;
			matrix[INVERSE_MATRIX, 1, 1] = (matrix[sourceMat, 0, 0] * matrix[sourceMat, 2, 2] - matrix[sourceMat, 2, 0] * matrix[sourceMat, 0, 2]) * idet;
			matrix[INVERSE_MATRIX, 2, 1] = (matrix[sourceMat, 0, 1] * matrix[sourceMat, 2, 0] - matrix[sourceMat, 0, 0] * matrix[sourceMat, 2, 1]) * idet;
			matrix[INVERSE_MATRIX, 0, 2] = (matrix[sourceMat, 0, 1] * matrix[sourceMat, 1, 2] - matrix[sourceMat, 0, 2] * matrix[sourceMat, 1, 1]) * idet;
			matrix[INVERSE_MATRIX, 1, 2] = (matrix[sourceMat, 0, 2] * matrix[sourceMat, 1, 0] - matrix[sourceMat, 0, 0] * matrix[sourceMat, 1, 2]) * idet;
			matrix[INVERSE_MATRIX, 2, 2] = (matrix[sourceMat, 0, 0] * matrix[sourceMat, 1, 1] - matrix[sourceMat, 0, 1] * matrix[sourceMat, 1, 0]) * idet;
		}
		/// <summary> Applies multiplication between source and temp matricies </summary>
		private void Multiply() {
			for (int c = 0; c < 3; c++) {
				for (int r = 0; r < 3; r++) {
					matrix[targetMat, c, r] = matrix[TEMP_MATRIX, 0, r] * matrix[sourceMat, c, 0] +
												 matrix[TEMP_MATRIX, 1, r] * matrix[sourceMat, c, 1] +
												 matrix[TEMP_MATRIX, 2, r] * matrix[sourceMat, c, 2];
				}
			}

			// Swap source and target matricies to cut down on unecessary copying
			int t = targetMat;
			targetMat = sourceMat;
			sourceMat = t;

			inverseDirty = true;
		}

		/// <summary> Applies matrix times input x/y coordinates </summary>
		/// <param name="ix"> Input x </param> <param name="iy"> Input y </param>
		/// <param name="ox"> Output x </param> <param name="oy"> Output y </param>
		private void Forward(float ix, float iy, out float ox, out float oy) {
			ox = ix * matrix[sourceMat, 0, 0] + iy * matrix[sourceMat, 1, 0] + matrix[sourceMat, 2, 0];
			oy = ix * matrix[sourceMat, 0, 1] + iy * matrix[sourceMat, 1, 1] + matrix[sourceMat, 2, 1];
		}

		/// <summary> Applies inverse matrix times input x/y coordinates </summary>
		/// <param name="ix"> Input x </param> <param name="iy"> Input y </param>
		/// <param name="ox"> Output x </param> <param name="oy"> Output y </param>
		private void Backward(float ix, float iy, out float ox, out float oy) {
			ox = ix * matrix[INVERSE_MATRIX, 0, 0] + iy * matrix[INVERSE_MATRIX, 1, 0] + matrix[INVERSE_MATRIX, 2, 0];
			oy = ix * matrix[INVERSE_MATRIX, 0, 1] + iy * matrix[INVERSE_MATRIX, 1, 1] + matrix[INVERSE_MATRIX, 2, 1];
		}

		/// <summary> 3d array of floats representing multiple matricies. </summary>
		private float[,,] matrix;
		/// <summary> Current target matrix </summary>
		private int targetMat;
		/// <summary> Current source matrix </summary>
		private int sourceMat;
		/// <summary> Constant temp matrix index </summary>
		private const int TEMP_MATRIX = 2;
		/// <summary> Constant inverse matrix index </summary>
		private const int INVERSE_MATRIX = 3;
		/// <summary> Dirty flag for inverse matrix </summary>
		private bool inverseDirty;
		#endregion
	}
}
