[![MapEditorReborn](https://cdn.discordapp.com/attachments/835633260339003392/971053338412089364/unknown.png)](https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley)
[![Discord](https://img.shields.io/discord/947849283514814486?color=%235865F2&label=Discord&style=for-the-badge)](https://discord.gg/JwAfeSd79u) [![Downloads](https://img.shields.io/github/downloads/Michal78900/MapEditorReborn/total?color=brown&label=Downloads&style=for-the-badge)](https://github.com/Michal78900/MapEditorReborn/releases) [![Patreons](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Fshieldsio-patreon.vercel.app%2Fapi%3Fusername%3DMapEditorReborn%26type%3Dpatrons&style=for-the-badge)](https://www.patreon.com/MapEditorReborn) ![Lines](https://img.shields.io/tokei/lines/github/Michal78900/MapEditorReborn?style=for-the-badge)

**<details><summary>Localized Readmes</summary>**
  
- [Português](https://github.com/Michal78900/MapEditorReborn/blob/dev/Localization/README-Portugu%C3%AAs.md)
- [Español](https://github.com/Michal78900/MapEditorReborn/blob/dev/Localization/README-Español.md)
- [Русский](https://github.com/Michal78900/MapEditorReborn/blob/dev/Localization/README-Русский.md)

</details>

# MapEditorReborn
MapEditorReborn is an [SCP: Secret Laboratory](https://store.steampowered.com/app/700330/SCP_Secret_Laboratory/) plugin allowing to spawn and modify various objects.

### Index:
>- <a href="README.md#Instalation">Instalation</a>
>- <a href="README.md#Usage">Usage</a>
>    - <a href="README.md#--toolgun">Toolgun</a>
>    - <a href="README.md#--commands">Commands</a>
>- <a href="README.md#Contributing">Contributing</a>
>- <a href="README.md#Qucik=Links">Quick Links</a>
>- <a href="README.md#Credits">Credits</a>

# Instalation
You will need **latest** [EXILED Version](https://github.com/Exiled-Team/EXILED/releases) installed on your server.

Put your [`MapEditorReborn.dll`](https://github.com/Michal78900/MapEditorReborn/releases) file in `EXILED\Plugins\` path.
Once your plugin will load, it will create directory `EXILED\Configs\MapEditorReborn\`; This directory will contain two sub-directiories **Schematics** and **Maps**

If you want to install `SL-CustomObjects` please click this [link](https://github.com/Michal78900/MapEditorReborn/tree/dev/SL-CustomObjects).

# Usage

### - ToolGun
ToolGun allows you to spawn various objects, on the SCP:SL map. Once object has been spawned they can be saved[^2] into the map file. <br>
To access ToolGun you need to execute `mp toolgun` command[^1]. <br>

To change ToolGun's Current Spawnable Object, press <kbd>T</kbd> on your keyboard.
You can change the ToolGun mode by setting their Flashlight on/off( <kbd>F</kbd> ), and by Zooming in/out( <kbd>🖱 RMB</kbd> )

### ToolGun modes:
- Create: Flashlight on, Zommed out
- Delete: Flashlight off, Zommed out
- Select: Flashlight on, Zommed in
- Copy: Flashlight off, Zommed in

### - Commands
Typing in `mp` command into [Text Based Remote Admin](https://en.scpslgame.com/index.php/Remote_Admin), will show you all avalible subcommands

# Developement
How to contribute to MER Using EXILED Framework

<h3>
<details><summary>Quick Links</summary>
  
- [Discord](https://discord.gg/JwAfeSd79u)
</details>
</h3>
  
# Credits
- Original plugin idea and code overhaul by [Killers0992](https://github.com/Killers0992)
- Another code overhaul and documentation making by [Nao](https://github.com/NaoUnderscore)
- Testing the plugin by Cegła, The Jukers server staff and others
- Plugin made by [Michal78900](https://github.com/Michal78900)
- The Surface map was taken from [SCP: Secret Laboratory](https://store.steampowered.com/app/700330/SCP_Secret_Laboratory/) game files, under use of [CC-BY-SA 3.](https://creativecommons.org/licenses/by/3.0/) Original credit goes to Northwood and Undertow Games. https://scpslgame.com/ https://www.scpcbgame.com/
- This project uses [NaughtyAttributes](https://github.com/dbrizov/NaughtyAttributes) made by dbrizov under MIT license https://github.com/dbrizov/NaughtyAttributes

[^1]: [Text Based Remote Admin](https://en.scpslgame.com/index.php/Remote_Admin).
[^2]: To save current spawned objects to the Map file, execute `mp save [Name]` command.
