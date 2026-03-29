# Decrypter and Dumper

The reason for running the mod under debug mode is to prevent the game data from being modified so that any dumped data
is unaltered.

Debug mode unloads every other mod and resets game's data if I understand correctly.

## Original Description
DO NOT KEEP THIS MOD CHECKED IN YOUR MODLIST!!!\
IT'S ONLY MEANT DO SOMETHING IN DEBUG MODE!

How to install from Steam Workshop:
1. Subscribe to the mod, THEN DISABLE it in your mod list!!
2. Copy-paste the Mod Source folder (ModProject) into your Mod creator's Work Root Directory. By default located in: "\Steam\steamapps\workshop\content\1468810\3225372871"
3. Go into ModAssets and copy the files "dumpClassList.txt" and "dumpClassListDLC.txt" into the game's main installation folder, e.g. "\Steam\steamapps\common\鬼谷八荒" or something else

How to Decrypt Json files from other mods:
1. Press 'Edit' to open it up, then press 'Go to game debugging'
2. Press the button "Decrypt Mod" that's been added at the right-hand side.
3. Select a (preferably) empty output folder to save the decrypted files.
4. If it's successful, the output folder will be opened.

How to re-Encrypt:
1. Same step as "How to Decrypt"
2. Press the button "Encrypt Mod" that's been added at the right-hand side.
The rest is identical to the decryption steps also...

Dumping the live-Vanilla game's Json files:
1. Same step as "How to Decrypt"
2. Press the button "Dump Json-data" that's been added at the right-hand side.
3. Wait for some time as the game will appear to be frozen, due to the sheer quantity of Json files.
The files will be located in the installation folder under "DataDump", e.g. something like "\Steam\steamapps\common\鬼谷八荒\DataDump"\
The DLC Json-file dump will be stored at "\Steam\steamapps\common\鬼谷八荒\DataDumpDLC"

The tool's behaviors:
- Non-encrypted Json/Cache/PNG files shouldn't be processed, e.g. won't be reverse-encrypted.
- The extension checker isn't case-sensitive, e.g. '.Json' and '.json' will both be read.
