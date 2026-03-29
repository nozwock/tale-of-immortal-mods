# Portrait Favorites/Database

An updated version of the now completely removed(?) English translated Portrait Database mod (and the original).

Before I list the changes made I'll like to mention that don't use this to either save the portraits of special NPCs
(like the 5 blooms) and transformed Imps, or to change their portrait with an already favorited portrait, it'll not
work, and you will just mess up your savegame a little (if you make a save after doing that obviously).

Now, for the relevant list of changes in brief (from last-to-first):
- Don't allow applying portrait of different gender to player and NPC
- Adjust 'Apply' button's confirmation message based on context
- Disable 'Apply' when Portraits UI is opened from UIModDress
- Use circular X button for btnClose instead of text button in UIModDress
- Remove custom UI from ModDress UI in Mod Creator context (since it doesn't work)
- Let users open Portraits UI from UIModDress spawned from another Portraits UI
- Fix users being able to spawn multiple UIModDress (broken) via 'Edit'
- Fix 'Apply' portrait not working for UIModDress
- Adjust confirmation for starting portrait edits and saving edits
- Don't show the Portraits UI if there are no portraits available
- Fix current page not being updated on deleting portraits
- Update selectIndex on deleting a favorite portrait, that is always have a valid portrait selected
- Center page InputField's text and placeholder component
- Restrict InputField to valid page numbers
- Localize the mod 'Portrait Portraits/Database' properly

## Original Description
捏脸数据库（捏脸收藏及易容） (Last updated: 11 Mar 2024 @ 9:28am) \
https://steamcommunity.com/sharedfiles/filedetails/?id=3043110804

You can collect your favorite and face pinching data in the corner creation interface, player attribute interface, NPC
information interface and module face pinching interface. After collection, it will automatically be included in the
face pinching database. You can freely select face pinching data from the database later.

Special Note: \
The database of this module can be used across archives. The saved data file is in the game root directory. The file
name is: ModelFile.json. If you have good face-making data, you are welcome to join the group to share it. \
QQ group: 830967745

This module comes with the ability to disguise players and NPCs.

It is recommended to use it with another module of mine: charm value display, which can make it easier to find facial
pinching data with good-looking vertical drawing and high charm value.