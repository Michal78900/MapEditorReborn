# MapEditorReborn

A SCP: Secret Laboratory plugin allowing to spawn and modify various objects.

[EXILED](https://github.com/Exiled-Team/EXILED) version **2.11.1+** must be installed for this to work.

Place the "MapEditorReborn.dll" file in your **EXILED/Plugins** folder.
  
At the first start of the server with the plugin installed, a folder named **MapEditorReborn** will be created inside **EXILED/Configs** directory. This folder is used to store your map schematics files.

# Features:
- Spawning all types of Doors, Workstations, Item and Player spawnpoints.
- Customizable options for all of the objects.
- A ToolGun which can be used for spawning/deleting objects.
- [CustomItems](https://github.com/Exiled-Team/CustomItems) support.
- Spawning objects inside the Facility without them being displaced due to different layout.
- Automatically loading a random map each round.
- Reloading a map when the map file was overwritten.

# Default config:
```yml
map_editor_reborn:
# Is the plugin enabled.
  is_enabled: true
  # Is the debug mode enabled.
  debug: false
  # Enables FileSystemWatcher in this plugin. What it does is when you manually change values in a currently loaded map file, after saving the file the plugin will automatically reload the map in-game with the new changes so you won't need to do it yourself.
  enable_file_system_watcher: false
  # Should any map be loaded automatically. If there are multiple, the random one will be choosen.
  load_maps_on_start: []
  translations:
    mode_creating: <color=yellow>Mode:</color> <color=green>Creating</color>
    mode_deleting: <color=yellow>Mode:</color> <color=red>Deleting</color>
    mode_selecting: <color=yellow>Mode:</color> <color=yellow>Selecting</color>
    mode_copying: <color=yellow>Mode:</color> <color=#34B4EB>Copying to the ToolGun</color>
```
Map files are located inside **EXILED/Configs/MapEditorReborn** folder.
 Keep in mind that `load_maps_on_start:` **is a list:**
```yml
# Valid fomating
load_maps_on_start: 
- mapName

# Invalid formating
load_maps_on_start: mapName
```

## Setting the flags
In a `DoorObject` you have 2 options - `keycard_permissions:` and `ignored_damage_sources:`. Both of these are enum flags and can contain multiple values.

For example if I would like to make a door ignore all damage types (including the server command) I would set `ignore_damage_sources:` to `30` which is equivalent to `ServerCommand, Grenade, Weapon, Scp096`.

You basiically need to add needed values together. After serialization, the value will be changed to a string of words.

```csharp
    [Flags]
    public enum KeycardPermissions : ushort
    {
        None = 0,
        Checkpoints = 1,
        ExitGates = 2,
        Intercom = 4,
        AlphaWarhead = 8,
        ContainmentLevelOne = 16,
        ContainmentLevelTwo = 32,
        ContainmentLevelThree = 64,
        ArmoryLevelOne = 128,
        ArmoryLevelTwo = 256,
        ArmoryLevelThree = 512,
        ScpOverride = 1024
    }
	
    [Flags]
    public enum DoorDamageType : byte
    {
        None = 1,
        ServerCommand = 2,
        Grenade = 4,
        Weapon = 8,
        Scp096 = 16
    }
```

# The ToolGun
ToolGun is the most important thing in this plugin. It allows you to spawn/delete objects. The ToolGun can also copy and paste existing ones.

The ToolGun has **4** modes. Selecting them depends on the zoom of the weapon or if the flashlight is enabled or not.

**Creating** *(unzoomed - flashlight disabled)*
Spawns a selected object. You can change the selected object by pressing **R** key (reload key).
 
**Deleting** *(unzoomed - flashlight enabled)*
Deletes a shooted object. It can only delete objects spawned with this plugin.

**Copying to the ToolGun** *(zoomed - flashlight disabled)*
Copies the selected object. When you change back to **Create** mode you will now spawn a copy of this object instead. To reset a ToolGun to a default settings, simply change mode to **Copying to the ToolGun** and shoot in the floor/wall. (basically don't shoot at any spawned object)

**Selecting an object** *(zoomed - flashlight enabled)*
Selects the object. Selected object can be modified via commands. Player/Item spawnpoints can be only selected with indicators turned on.


# Commands
## All MapEditorReborn commands starts with `mp` prefix

### Utility Commands
These commands don't have any extra options. You only specify **1** argument.
| Command | Prefix | Required permission | Description
| :-------------: | :---------: | :---------: | :---------:
| **toolgun** | tg | `mpr.toolgun` | Gives sender a ToolGun. The same command will remove it, if the sender already has one. |
| **save** | s | `mpr.save` | Saves a map. It takes the map name as the argument. |
| **load** | l | `mpr.load` | Loads the map from the file. It takes the map name as the argument. |

### Modifying Commands
These commands have 2 or 3 options that must be specified before entering actual arguments. Use the command without anything to see these options.
| Command | Prefix | Required permission | Description
| :-------------: | :---------: | :---------: | :---------:
| **position** | pos | `mpr.position` | Changes the postion of the selected object. |
| **rotation** | rot | `mpr.rotation` | Changes the rotation of the selected object. |
| **scale** | si | `mpr.scale` | Changes the scale of the selected object. |

# Limitations
- ~~**You can't spawn doors inside the Facility.** This is related to a certain bug which crashes all the clients (players) when the door is spawned. This may change in 11.0 Parabellum Update.~~ Okay, so I technically you *can* spawn doors inside the facility, but I **really** recommend not to because it may crash your game. **You are doing it on your own risk**
- Spawned Workstations aren't actually functional - you can't modify weapons with them. This also may change in 11.0 Parabellum Update.
- For now, you can only modify the position, rotation and a object's scale via commands. Rest of the values can be only changed directly in the map's file.
- ~~Player/Item spawn point doesn't have a visible gameObject (this is why command showindicators exists). Because of that, once spawned you can't actually select them via ToolGun. The only way to modify them is mentioned previously manual values editing in the map's file.~~ You need to use `mp showindicators` first, and shot it's indicator to select these objects.

# Credits
- Original plugin idea by Killers0992
- Plugin made by Michal78900
