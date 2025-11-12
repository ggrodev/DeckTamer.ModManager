ModManager for MelonLoader

ModManager adds an in-game interface for managing MelonLoader mods.
It lets you view all loaded mods, enable or disable them directly from the game, and restart to apply changes — no file renaming or manual folder work required.

Features

In-game mod list showing active and disabled mods

Enable or disable mods through the UI

Automatic detection of the Mods folder

Displays a restart-required message after changes

Button to open the Mods folder in Windows Explorer

How It Works

ModManager scans the Mods directory for both .dll and .disabled files.
Each mod entry in the list includes:

A toggle to enable or disable it

The mod name, version, and author

When you toggle a mod, ModManager renames the file from .dll  to .disable and viceverse and shows a warning to restart the game.

Dependencies:
MelonLoader Installed on the game.
