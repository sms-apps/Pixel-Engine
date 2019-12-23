using PixelEngine.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine.Examples {
	/// <summary> Example class showing how to work with save files. </summary>
	public class SaveFileExample : Game {
		/// <summary> Entry point </summary>
		public static void Run(string[] args) {
			SaveFileExample game = new SaveFileExample();
			game.Construct(200, 200, 5, 5, 30);
			game.Start();

			
		}

		/// <summary> Class holding game data. This will get serialized out to the save file </summary>
		class GameData {
			/// <summary> Current money in pocket </summary>
			public double money;
			/// <summary> Total clicks since game start </summary>
			public long clicks;
			/// <summary> Things found by the player </summary>
			public List<Thing> things;
			/// <summary> Powerups bought by the player </summary>
			public int powerups;
			public GameData() {
				money = 0;
				powerups = 0;
				clicks = 0;
				things = new List<Thing>();
			}
		}

		/// <summary> Some Thing the player found. Structs can be saved/loaded as well. </summary>
		struct Thing {
			// <summary> Name of the Thing </summary>
			public string name;
			// <summary> Power of the Thing </summary>
			public float power;
			public Thing(string n, float p) { name = n; power = p; }
			public override string ToString() { return $"{name} - Power: {power:F2}"; }
		}

		/// <summary> Little UI system </summary>
		List<Button> buttons;

		/// <summary> Current game data </summary>
		GameData gameData;
		/// <summary> Current best thing </summary>
		Thing? best;
		/// <summary> Total power of all owned Things </summary>
		float totalThingPower;
		/// <summary> Base click money earning ability </summary>
		float income;
		/// <summary> Drop chance per click </summary>
		float dropChance;
		/// <summary> Cost of next powerup </summary>
		int nextCost;

		/// <inheritdoc />
		public SaveFileExample() {
			buttons = new List<Button>();
			gameData = new GameData();

		}
		/// <summary> Place to save the file... </summary>
		string saveFile { get { return WindowsInfo.UserDataPath + "/save.json"; } }
		/// <inheritdoc />
		// Called when the game starts, we can check for the save file here.
		public override void OnCreate() {
			// Be safe about it 
			try {
				// See if the file exists 
				if (File.Exists(saveFile)) {
					// Try to load the file's text 
					string saveFileText = File.ReadAllText(saveFile);
					// And convert it to an object
					gameData = Json.To<GameData>(saveFileText);
				}
			} catch (Exception e) {
				Console.WriteLine("Error loading save: " + e);
			}
			
			// Add UI buttons 
			buttons.Add(new Button(new Vector2(ScreenWidth / 2, ScreenHeight / 2), "CLICK ME", Clicked, Pixel.Presets.Mint));
			buttons.Add(new Button(new Vector2(ScreenWidth / 2, ScreenHeight / 2 + Font.CharHeight * 2), "BUY POWERUP", TryBuyPowerup, Pixel.Presets.Red));

			// Recalculate some stats from loaded game data. 
			Recalc();
		}
		/// <inheritdoc />
		// Called when the game ends, we can save here.
		public override void OnDestroy() {
			// Be safe about it 
			try {
				// Convert save data to json text
				string saveFileText = Json.ToJson(gameData);
				// Write text to file !
				File.WriteAllText(saveFile, saveFileText);

			} catch (Exception e) {
				Console.WriteLine("Error saving game: " + e);
			}
		}


		/// <inheritdoc />
		public override void OnUpdate(float delta) {
			Clear(Pixel.Presets.Black);
			
			Vector2 mouse = new Vector2(MouseX, MouseY);
			Input left = GetMouse(Mouse.Left);

			foreach (var b in buttons) {
				// Get size of rendered text
				Point size = CenterText(b.center, b.text, b.color);
				
				// calc half size to get area of screen button was on 
				Vector2 half = new Vector2(size.X + 4, size.Y + 4) / 2f;
				Vector2 max = b.center + half;
				Vector2 min = b.center - half;
				Rect area = new Rect();
				area.min = min; area.max = max;
				
				Pixel color = b.color;

				// See if the button was clicked
				if (area.Contains(mouse) && left.Pressed) { 
					b.onClick(); 
					color = Pixel.Presets.Pink;
				}

				// Draw rect around button region 
				DrawRect(min.FloorToInt(), max.FloorToInt(), color);
			}

			// Draw all of this UI garbage.
			CenterText(new Vector2(ScreenWidth/2, Font.CharHeight), $"Money: ${gameData.money:F0}", Pixel.Presets.Yellow);
			CenterText(new Vector2(ScreenWidth/2, Font.CharHeight*2), $"Clicks: {gameData.clicks}", Pixel.Presets.Pink);

			CenterText(new Vector2(ScreenWidth/2, Font.CharHeight*3), $"Drop Chance: {(100.0f*dropChance):F2}%.", Pixel.Presets.Purple);
			if (best != null) {
				CenterText(new Vector2(ScreenWidth/2, Font.CharHeight*4), $"You have {gameData.things.Count} things.", Pixel.Presets.Orange);
				CenterText(new Vector2(ScreenWidth/2, Font.CharHeight*5.5f), $"Your Best Thing Is", Pixel.Presets.Beige);
				CenterText(new Vector2(ScreenWidth/2, Font.CharHeight*6.5f), $"{best}", Pixel.Presets.Apricot);
			} else {
				CenterText(new Vector2(ScreenWidth/2, Font.CharHeight*4), $"You have no Things :(", Pixel.Presets.Orange);
			}
			
			CenterText(new Vector2(ScreenWidth/2, ScreenHeight-Font.CharHeight*5.5f), $"Powerups: {gameData.powerups}", Pixel.Presets.Red);
			CenterText(new Vector2(ScreenWidth/2, ScreenHeight-Font.CharHeight*4.5f), $"Next costs: ${nextCost}", Pixel.Presets.Yellow);
			CenterText(new Vector2(ScreenWidth/2, ScreenHeight-Font.CharHeight*3), $"Thing power: {totalThingPower:F2}", Pixel.Presets.Purple);
			CenterText(new Vector2(ScreenWidth/2, ScreenHeight-Font.CharHeight*2), $"Base income: ${income:F2}", Pixel.Presets.Yellow);

		}

		/// <summary> Function to call when the main button is clicked. </summary>
		void Clicked() {
			gameData.clicks += 1;
			gameData.money += income;

			if (Random(1.0f) < dropChance) {
				DropThing();
			}
		}

		/// <summary> Called to give the player a new Thing </summary>
		void DropThing() {
			string name = string.Concat(MakeArray(Random(3, 8), i => (char)Random((int)'A', (int)'Z')));
			gameData.things.Add(new Thing(name, Random(1.0f, 3.0f + Mathf.Sqrt(gameData.things.Count))));
			Recalc();
		}

		/// <summary> Called when the player clicks the buy powerup button </summary>
		void TryBuyPowerup() {
			int cost = PowerUpCost(gameData.powerups);
			if (gameData.money >= cost) {
				gameData.money -= cost;
				gameData.powerups++;
				Recalc();
			}
		}

		/// <summary> Recalculates information derived from game state. </summary>
		void Recalc() {
			dropChance = DropChance(gameData.things.Count, gameData.powerups);
			nextCost = PowerUpCost(gameData.powerups);
			best = BestThing(gameData.things);
			totalThingPower = 1 + TotalThingPower(gameData.things);

			income = BaseIncome(totalThingPower, gameData.powerups);
			if (best != null) { 
				income += best.Value.power; 
			}
			
		}

		/// <summary> Combine ratios so they never reach 100% </summary>
		// Balance: Want to let players progress slowly towards endgame?
		// This is a great way, lets % approach, but not reach 100%
		float CombineRatio(float a, float b) { return (1.0f- ((1.0f-a) *(1.0f-b))); }

		/// <summary> Calculate current thing chance </summary>
		float DropChance(int ownedThings, int powerups) {
			float chance = .01f;

			// Give new games a boost to drop chance until they get some Things
			for (int i = 10 - ownedThings; i >= 0; i--) {
				chance = CombineRatio(chance, .0040f);
			}
			// Add a small drop chance per powerup 
			for (int i = 0; i < powerups; i++) {
				chance = CombineRatio(chance, .0033f);
			}

			return chance;
		}

		/// <summary> Find the user's best Thing, or null if they have none. </summary>
		Thing? BestThing(List<Thing> things) {
			Thing? best = null;
			foreach (var thing in things) {
				if (best == null) { best = thing; }
				else if (thing.power > best.Value.power) { best = thing; }
			}
			return best;
		}

		/// <summary> SUM ALL THE THINGS power. </summary>
		float TotalThingPower(List<Thing> things) { return gameData.things.Sum(it => it.power); }

		/// <summary> Calculate baseIncome from thing power. </summary>
		float BaseIncome(float total, int powerups) {
			// Square root grows fairly slowly as users amass lots of THING POWER
			float p = total <= 0 ? 1 : Mathf.Sqrt(total);
			// Log grows even slower.
			p += powerups * Mathf.Log10(total);

			return p;
		}

		/// <summary> Get the cost of the next Power Up </summary>
		int PowerUpCost(int owned) {
			// Exponential cost increase helps keep the game from being too 'easy' 
			return (int)(1000 * Mathf.Pow(1.15f, (owned)));
		}


		/// <summary> Draw some text centered on a position </summary>
		Point CenterText(Vector2 center, string text, Pixel color) {
			Point size = Font.Measure(text);
			Vector2 offset = new Vector2(size.X, size.Y) / 2;
			DrawText(center - offset, text, color);
			return size;
		}

		/// <summary> UI Helper class </summary>
		class Button {
			public Vector2 center;
			public string text;
			public Action onClick;
			public Pixel color;
			public Button(Vector2 center, string text, Action onClick, Pixel? color = null) {
				this.center = center; this.text = text; this.onClick = onClick;
				this.color = color ?? Pixel.Presets.White;
			}
		}
	}
}
