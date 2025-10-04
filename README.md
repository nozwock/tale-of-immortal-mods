# Tale of Immortal Modding
There are [notes](./docs/NOTES.md) in `docs/`.

## Build
- Get [just] for the build scripts, and [TOITool] (and it to the `PATH` as `toi`) for packaging mods.
- Create a `Local.props` file in the repo's root:
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


[TOITool]: https://github.com/nozwock/tale-of-immortal-tool/
[just]: https://github.com/casey/just/
