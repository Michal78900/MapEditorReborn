# SL-CustomObjects
 The project that allows you to create schematics out of in-game items in SCP: Secret Labortatory.

## Requirements
 - [**UnityHub**](https://unity3d.com/get-unity/download)<br><br>
 - [**Unity Editor 2019.4.16f1**](https://download.unity3d.com/download_unity/e05b6e02d63e/Windows64EditorInstaller/UnitySetup64-2019.4.16f1.exe)<br>
   The version matters if you want to create animation, it can be probably any 2019 version.<br><br>
 - About 1 GB of free space. The reason behind it is, when you first launch the project Unity needs to create the **Library** folder for faster loading times etc.
 <br>

## Setting it up
 - **Step 1**<br>
 [Download the latest verion of SL-CustomObjects](https://github.com/Michal78900/MapEditorReborn/releases)<br><br>
 
 - **Step 2**<br>
 Open Unity Hub and add the project.
 ![alt text](https://cdn.discordapp.com/attachments/947851609294114817/985225885026226206/unknown.png)<br><br>
 
 - **Step 3**<br>
 Copy the Scene file you want from `Assets/Resources/Scenes` folder to `My Projects` folder. This will prevent removal of it when updating the project in a future.<br><br>

 - **Step 4**<br>
 Create an empty object and add **Schematic component** to it. The name of that object will be the name of the schematic **(it can't start with the number)**. Use stuff from `Assets/Resources/Blocks` to create your own schematic by attaching them to the root object.<br><br>

 - **Step 5**<br>
 When you've done, press <kbd>F6</kbd> to compile the schematic.<br><br>

 - **Step 6**<br>
 Open the export folder and move **the entire schematic directory** onto your server into `MapEditorReborn/Schematics` folder.<br><br>

 - **Step 7**<br>
 Spawn the schematic in-game using `mp create schematicName` command.
