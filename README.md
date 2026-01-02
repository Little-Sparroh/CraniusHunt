# CraniusHunt

Provides direct access to the hidden Cranius boss fight in MycoPunk.

## Description

This mod converts the standalone Cranius hunt mission to use the game's authentic hidden mission mechanics. Instead of a custom implementation, it now triggers the genuine hidden Cranius encounter that normally requires finding a secret button in the Moldy Tundra region.

The mission appears randomly in different regions on the mission select map and loads the hidden "Tundra1Flat" scene with all the original special effects: fungal mass absorption, upgrade pulls, boss music, and the Scrap Boss fight.

## Getting Started

### Dependencies

* MycoPunk (base game)
* [BepInEx](https://github.com/BepInEx/BepInEx) - Version 5.4.2403 or compatible
* .NET Framework 4.8
* [HarmonyLib](https://github.com/pardeike/Harmony) (included via NuGet)

### Building/Compiling

1. Clone this repository
2. Open the solution file in Visual Studio, Rider, or your preferred C# IDE
3. Build the project in Release mode to generate the .dll file

Alternatively, use dotnet CLI:
```bash
dotnet build --configuration Release
```

### Installing

**Option 1: Via Thunderstore (Recommended)**
1. Download and install via Thunderstore Mod Manager

**Option 2: Manual Installation**
1. Download the latest release
2. Place the .dll file in your `<MycoPunk Directory>/BepInEx/plugins/` folder

### Usage

Once installed, the mod will automatically load through BepInEx when the game starts. Check the BepInEx console for loading confirmation messages. The Cranius Hunt mission will appear randomly in different regions on the mission select map.

Select the "Cranius Hunt" mission to start the authentic hidden boss fight with:
- Transportation to the hidden "Tundra1Flat" scene
- Fungal mass absorption sequence
- Upgrade pulls from the environment
- Cranius boss music and effects
- Scrap Boss encounter with special mechanics

### Project Structure

- **Plugin.cs:** Contains the mod's main logic, including Harmony patches to add the Cranius mission to the game
- **thunderstore.toml:** Configuration file for publishing to Thunderstore
- **CSPROJECT.csproj:** Project file with build settings and dependencies
- **icon.png:** Mod icon for Thunderstore
- **README.md:** This documentation
- **CHANGELOG.md:** Version history and changes
- **LICENSE:** MIT license file

## Help

* **First time modding?** Check BepInEx documentation and MycoPunk modding resources
* **Harmony patches failing?** Ensure method signatures match the game's IL
* **Dependency issues?** Update NuGet packages and verify .NET runtime version
* **Thunderstore publishing?** Update all metadata in thunderstore.toml before publishing
* **Plugin not loading?** Check BepInEx logs for errors and verify GUID uniqueness

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for version history and changes.

## Authors

- Sparroh
- funlennysub (BepInEx template)
- [@DomPizzie](https://twitter.com/dompizzie) (README template)

## License

* This project is licensed under the MIT License - see the LICENSE.md file for details
