# Changelog

## 1.1.0 (2026-01-02)

### Major Changes
* **Authentic Hidden Mission**: Converted the mod to use the game's genuine hidden Cranius mission mechanics instead of a custom Amalgamation-based implementation
* **Hidden Scene Loading**: Mission now loads the authentic "Tundra1Flat" hidden scene with all original effects
* **Proper Boss Mechanics**: Uses `FlatTundraMission` and `FlatTundraObjective` for the complete hidden boss encounter experience

### Added
* Inheritance from `FlatTundraMission` for authentic hidden mission behavior
* Scene override to load "Tundra1Flat" scene
* Team revive override (set to 0)
* Reset date time override to prevent replay menu errors
* Debug logging for region and scene information

### Removed
* Custom `AmalgamationObjective` patching
* Boss class setup code
* Replay menu functionality (mission now appears in random regions)
* Region override (allows random region placement)

### Technical
* Fixed mission container issues for proper mission initialization
* Updated mission appearance to random regions on the map
* Maintained compatibility with existing save data

## 1.0.2 (2025-10-07)

### Added
* Standalone Cranius hunt mission available in the replay menu
* Cloning system for Amalgamation missions with custom name and description
* Integration with mission selection window for proper mission registration
* Automatic player flag setting for mission accessibility
* Proper mission icon and color (magenta) for Cranius mission
* Duplicate mission button prevention in replay menu

## 1.0.0 (2025-08-19)

### Features
* Add MinVer
* Add thunderstore.toml for [tcli](https://github.com/thunderstore-io/thunderstore-cli)
* Add LICENSE and CHANGELOG.md
