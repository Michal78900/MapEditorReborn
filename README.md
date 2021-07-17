# MapEditorReborn

A SCP: Secret Laboratory plugin allowing to spawn and modify various objects.

[EXILED](https://github.com/Exiled-Team/EXILED) version **2.11.1+** must be installed for this to work.

Place the "SerpentsHand.dll" file in your **EXILED/Plugins** folder.
  
At the first start of the server with the plugin installed, a folder named **MapEditorReborn** will be created inside **EXILED/Configs** directory. This folder is used to store your map schematics files.

# Features:
- Spawning all types of Doors, Workstations, Item and Player spawn points
- Customizable options for all of the objects
- A ToolGun which can be used for spawning/deleting objects.
- [CustomItems](https://github.com/Exiled-Team/CustomItems) support
- You can spawn objects inside the Facility without them being displaced due to different layout
- Automatically loading a random map each round.
- Reloading a map when the map file was overwritten.

# The ToolGun
ToolGun is the most important thing in this plugin. It allows you to spawn/delete objects. The ToolGun can also copy and paste existing ones.

The ToolGun has **4** modes. Selecting them depends on the zoom of the weapon or if the flashlight is enabled or not. 
**Creating** *(unzoomed - flashlight disabled)*
 Spawns a selected object. You can change the selected object by pressing **R** key (reload key).
 
**Deleting** *(unzoomed - flashlight enabled)*
Deletes a shooted object. It can only delete objects spawned by this plugin.

**Copying to the ToolGun** *(zoomed - flashlight disabled)*
Copies the selected object. When you change back to **Create** mode you will now spawn copy of this object instead. To reset a ToolGun to a default settings, simply change mode to **Copying to the ToolGun** and shoot in the floor/wall. (basically don't shoot at any spawned object)

~~**Selecting an object** *(currently unused)*~~


# Commands
All MapEditorReborn commands starts with `mp` prefix
| Command | Prefix | Required permission | Description
| :-------------: | :---------: | :---------: | :---------:
| **toolgun** | tg | `mpr.toolgun` | Gives sender a ToolGun. The same command will remove it, if the sender already has one. |
| **save** | s | `mpr.save` | Saves a map. It takes the map name as the argument. |
| **load** | l | `mpr.load` | Loads the map from the file. It takes the map name as the argument. |
| **showindicators** | si | `mpr.showindicators` | Shows where ItemSpawnPoint and PlayerSpawnPoint are located. |

# Limitations
- **You can't spawn doors inside the Facility.** This is related to a certain bug which crashes all the clients (players) when the door is spawned. This may change in 11.0 Parabelum Update.
- Spawned Workstations aren't actually functional - you can't modify weapons in them. This also may change in 11.0 Parabelum Update.
- For now, the only option to modify an object is manualy editing values in the map's file, but the ToolGun can copy and paste a modified object.
- Player/Item spawn point doesn't have a visible gameObject (this is why command showindicators exists). Because of that, once spawned you can't actually select them via ToolGun. The only way to modify them is mentioned previously manaul values editing in the map's file.

# Credits
Original plugin idea by Killers0992