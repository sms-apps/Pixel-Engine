# Changelog

### 2019/12/20
I've been thinking of doing this for a while, but I'm building on top of `DevChrome`'s work to add some features.
Reading the source was pretty fun, and I think I've got a good handle on the PixelGameEngine's inner workings.

All changes since originally forked up to now:
- Code is now heavily doc-commented, and doc-comments are built alongside dll.
  - Still need to comment the massive `Vector.cs` file.
- All classes that used `static` constructors now avoid doing so.
  - This saves a slight performance penalty when rapidly accessing such classes.
- Core requirement for a drawable `Sprite` broken out into `ISprite` interface.
  - `PalettableSprite` is a `ISprite` which is indirected during drawing to use a different `Pixel[]` palette.
- Overhauls the `Json` utility class. Now it uses my `XtoJSON` library. 
  - Sorry if this is a breaking change, but the previous `Json` class was not usable, and would heavily conflict with the names in my code 
- Minor changes to `Clock`/`Timer` logic, some of it was being done inside `Game`
- Packs `Input` struct in a single `byte` to be less wasteful
- Packs snapshots of keyboard/mouse inside `Bitflags`/`IntFlags` types to be less wasteful
- Adds lots more `Key`s to be detected by default.
- Adds some operators to `Point`
- Adds ARGB/RGBA distinct helpers to `Pixel` 
- Renames some stuff inside of `Noise`
  - `Perlin` is now `Sample` to more accurately reflect what it does.
  - `Calculate` is now `FBM`, same reason.
  - Technically the entire `Noise` class is an implementation of `SimplexNoise`, but I digress.
- Added a Vector library similar to Unity3d's in Vector.cs
