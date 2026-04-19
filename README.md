# Tale of Immortal Modding
Mods for [鬼谷八荒 Tale of Immortal][steam-toi].

> [!TIP]
> There are modding related [notes](./docs/NOTES.md) in `docs/`.

## Installation

Extract the contents of the mod archives into `<Game Folder>/ModExportData/`.

The mods should then be visible in the mod browser.

## Build
- Get [just] for the build scripts, and [TaleOfImmortalTool] 0.4.0+ (add it to the `PATH` as `toi`) for packaging mods.
- Create a `Local.props` file under src/ next to `Directory.Build.props` with the Game's root path:
```xml
<Project>
  <PropertyGroup>
    <GameDir>...</GameDir>
  </PropertyGroup>
</Project>
```
- Create an `.env` file in the repo's root with `$MOD_PATH` defined:
```sh
MOD_PATH='/path/to/ModExportData'
```
- Run `just` from a mod's directory or subdirectory to build the mod, and `just pack` to build and export to `$MOD_PATH`.

## Other
Some other repositories for TOI mods:
- https://github.com/creater0822/TOI_Mods
- https://github.com/LynBean/Tale-of-Immortal


[just]: https://github.com/casey/just/
[TaleOfImmortalTool]: https://github.com/nozwock/tale-of-immortal-tool/
[steam-toi]: https://store.steampowered.com/app/1468810/_Tale_of_Immortal/

