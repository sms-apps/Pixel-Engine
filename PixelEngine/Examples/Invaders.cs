using PixelEngine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine.Examples {
	/// <summary> Example Space Invadorks game </summary>
	public class Invaders : Game {
		/// <summary> Entrypoint </summary>
		public static void Run(string[] args) {
			Invaders game = new Invaders();
			game.Construct(100, 100, 5, 5, 60);
			game.Start();
		}
		Pixel bgColor;
		List<Entity> entities;

		/// <summary> Simple entity class </summary>
		abstract class Entity {
			public Vector2 position;
			public ISprite sprite;
			public abstract void Update(Game game, float delta);
			public virtual void Draw(Game game) {
				if (sprite != null) {
					game.DrawSprite(position, sprite);
				}
			}

		}

		static int countBits(long data) {
			int cnt = 0;
			while (data != 0) {
				data = data & (data - 1);
				cnt++;
			}
			return cnt;
		}
		class Invader : Entity {
			int speed;
			int framesPerTick;
			int framesSinceTick;
			int seed;
			ISprite[] poses;
			int frame = 0;

			public Invader(int seed) {
				this.seed = seed;
				Randoms.Seed = seed;
				int numFrames = Randoms.RandomInt(2,7);
				poses = new ISprite[numFrames];
				Pixel color = Pixel.Random();

				long baseFrame = 0;
				int size = Randoms.RandomInt(5, 11);
				int numBits = size*size - (size * (size/2));
				long bitMask = (1L << (1+numBits)) - 1L;

				// Make sure at least some bits are on.
				Console.WriteLine($"Generating {seed} => {size}^2 ({numBits}) invader. Bitmask is 0x{bitMask:X16}.");
				while (countBits(baseFrame) < 6) {
					if (baseFrame != 0) {
						Console.WriteLine($"{baseFrame} has too few bits!.");
					}
					baseFrame = 0;
					baseFrame |= ((long)Randoms.RandomInt(int.MinValue, int.MaxValue));
					baseFrame |= ((long)Randoms.RandomInt(int.MinValue, int.MaxValue)) << 32;
					baseFrame &= bitMask;
				}
				for (int i = 0; i < numFrames; i++) {
					long bits = baseFrame;
					if (i != 0) {
						long mask = 0;
						int numFlips = Randoms.RandomInt(1,8);
						for (int k = 0; k < numFlips; k++) {
							int attempt = Randoms.RandomInt(0, numBits);
							bool alreadyDidIt = (mask & (1L << attempt)) != 0;
							if (!alreadyDidIt) { mask |= 1L << attempt; }
							else { k--; }
						}
						bits ^= mask;
					}

					poses[i] = CreateInvaderSprite(size, bits, color);
				}
				sprite = poses[0];
				speed = 1 + Randoms.RandomInt(0, 2);
				framesPerTick = 3 + Randoms.RandomInt(0, 6);
				framesSinceTick = 0;
				position = default;
			}
			public override void Update(Game game, float delta) {
				framesSinceTick++;
				if (framesSinceTick == framesPerTick) {
					framesSinceTick = 0;

					frame = (frame + 1) % poses.Length; 
					sprite = poses[frame];

					position.x += speed;
					if (speed > 0) {
						if (position.x >= game.ScreenWidth - sprite.Width) {
							position.x = game.ScreenWidth - sprite.Width;
							position.y += 1;
							speed *= -1;
						}
					} else {
						if (position.x <= 0) {
							position.x = 0;
							position.y += 1;
							speed *= -1;
						}
					}
					// For debug purposes.
					if (position.y >=  game.ScreenHeight / 2) {
						position.y = game.ScreenHeight /2;
					}
				}
			}
		}

		/// <inheritdoc />
		public Invaders() {
			entities = new List<Entity>();
		}
		/// <inheritdoc />
		public override void OnCreate() {
			PixelMode = Pixel.Mode.Alpha;
			Reset();
		}

		private void Reset() {
			entities.Clear();
			for (int i = 0; i < 14; i++) {
				Invader invader = new Invader(Randoms.RandomInt(int.MinValue, int.MaxValue));

				invader.position.x = Random(0, ScreenWidth);
				invader.position.y = Random(0, ScreenHeight / 2);
				entities.Add(invader);
			}
		}

		/// <inheritdoc />
		public override void OnUpdate(float delta) {
			Clear(bgColor);
			if (GetKey(Key.R).Pressed) {
				Reset();
			}

			foreach (var entity in entities) {
				entity.Update(this, delta);
			}

			entities.Sort((a, b) => {
				if (a.sprite == null) { return -1; }
				if (b.sprite == null) { return 1; }
				int ay = (int)(a.position.y + a.sprite.Height);
				int by = (int)(b.position.y + b.sprite.Height);
				return ay - by;
			});
			foreach (var entity in entities) {
				entity.Draw(this);
			}
		}

		static ISprite CreateInvaderSprite(int size, long bits, Pixel color) {
			Sprite spr = new Sprite(size, size);

			bool odd = size % 2 == 1;

			for (int yy = 0; yy < size; yy++) {
				for (int xx = 0; xx < (odd?1:0)+size/2; xx++) {
					int i = yy * size + xx;
					long mask = 1 << i;
					bool bit = (bits & mask) != 0;

					if (bit) {
						spr[xx,yy] = color;
						//Mirroring
						spr[size-1-xx,yy] = color;
					}

				}
			}

			return spr;
		}




	}
}
