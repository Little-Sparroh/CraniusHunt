# CraniusHunt

A BepInEx mod for MycoPunk that adds a standalone hunt mission for the secret boss Cranius.

## Description

This mod enables players to hunt down the elusive boss Cranius by adding a dedicated mission to the game's replay menu. Rather than hunting for Cranius during regular play, this mod clones the amalgamation mission system to create a separate, repeatable challenge focused solely on encountering and defeating Cranius.

The mod uses Harmony patches to integrate seamlessly with the mission selection system, creating a custom CraniusMission class that inherits from AmalgamationMission. This ensures the Cranius hunting experience works identically to other Amalgamation missions but guarantees Cranius encounters every time.

## Getting Started

### Dependencies

* MycoPunk (base game)
* [BepInEx](https://github.com/BepInEx/BepInEx) - Version 5.4.2403 or compatible
* .NET Framework 4.8

### Building/Compiling

1. Clone this repository
2. Open the solution file in Visual Studio, Rider, or your preferred C# IDE
3. Build the project in Release mode

Alternatively, use dotnet CLI:
```bash
dotnet build --configuration Release
```

### Installing

**Option 1: Via Thunderstore (Recommended)**
1. Download and install using the Thunderstore Mod Manager
2. Search for "CraniusHunt" under MycoPunk community
3. Install and enable the mod

**Option 2: Manual Installation**
1. Ensure BepInEx is installed for MycoPunk
2. Copy `CraniusHunt.dll` from the build folder
3. Place it in `<MycoPunk Game Directory>/BepInEx/plugins/`
4. Launch the game

### Executing program

Once the mod is loaded and the game is started, the Cranius hunt mission will be available in the replay menu:

1. **Access the Replay Menu:**
   - Press the replay button or access the replay missions section from the main menu

2. **Select Cranius Mission:**
   - Look for the magenta-colored mission button with "cranius_hunt" name
   - The mission will have a larger appearance (1.5x scale) and distinctive coloration
   - Description: "Hunt down the secret boss Cranius!"

3. **Play the Mission:**
   - Click the mission button to start
   - The mission will generate a random scene with Cranius as the target
   - Proceed to hunt and defeat Cranius using normal game mechanics

The mission behaves identically to Amalgamation missions but guarantees Cranius encounters. Multiple regeneration cycles may be needed to reach Cranius depending on spawning mechanics.

## Help

* **Cranius mission not showing?** Make sure the mod loads successfully and check BepInEx console for errors
* **Mission appears but Cranius doesn't spawn?** This mod only adds the mission selection; Cranius spawning depends on game mechanics and may require multiple cycles
* **Multiple mission buttons?** The mod includes duplicate prevention logic, but rare timing issues might create multiples - restart the menu
* **Conflicts with other mods?** If you have other mission-related modifications that change mission lists or spawners, incompatibilities may occur
* **Performance issues?** This mod only patches mission selection and doesn't run during active gameplay
* **Can't complete mission?** Cranius missions may have specific victory conditions - consult game wiki for detailed strategies

## Authors

* Sparroh
* funlennysub (original mod template)
* [@DomPizzie](https://twitter.com/dompizzie) (README template)

## License

* This project is licensed under the MIT License - see the LICENSE.md file for details
