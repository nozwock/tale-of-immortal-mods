# Tale of Immortal Modding

## Build
- Get [TOITool] - used for packaging mods, and [just] for the build scripts.
- Create an `.env` file in the repo's root with `$MOD_PATH` defined:
```sh
MOD_PATH='/path/to/ModExportData'
```
- Create a `Local.props` file in the repo's root:
```xml
<Project>
  <PropertyGroup>
    <GameDir>...</GameDir>
  </PropertyGroup>
</Project>
```
- Run `just` from a mod's directory or subdirectory to build and export the mod to `$MOD_PATH`.


[TOITool]: https://github.com/nozwock/tale-of-immortal-tool/
[just]: https://github.com/casey/just/
