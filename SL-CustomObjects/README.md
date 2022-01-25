# SL-CustomObjects
 The project that allows you to create schematics out of in-game items in SCP: Secret Labortatory.

# This tutorial is outdated a bit. You now use primitives instead in-game items. The whole tutorial will be updated in a future. 

 ## Requirements
 - Unity Editor `2019.4.16f1` **The version matters if you want to create animation**
 - A couple of GBs of free space (probably 5 GB). The reason behind it is, when you first launch the project Unity needs to create the **Library** folder for faster loading times etc.
 <br>

## Setting it up
- Step 1
 [**Download the MapEditorReborn source code**](https://github.com/Michal78900/MapEditorReborn/archive/refs/heads/dev.zip)
 
 - Step 2
 Unzip the file and keep only the **SL-CustomObjects** folder. You can remove the rest.
 ![alt text](https://cdn.discordapp.com/attachments/859071008468238386/908418425725018192/unknown.png)
 
 - Step 3
 Open Unity Hub and add the project.
 ![alt text](https://cdn.discordapp.com/attachments/859071008468238386/908131196297437224/AddingTheProject.png)
 
 - Step 4
 In Assets you should have CustomObjects scene, open that scene and do your stuff....
 ![alt text](https://cdn.discordapp.com/attachments/859071008468238386/908131204820271105/Scene.png)
  ![alt text](https://cdn.discordapp.com/attachments/859071008468238386/908131170385023076/AddingSchematicComponent.png)
 ![alt text](https://cdn.discordapp.com/attachments/859071008468238386/908131200516915260/DragAndDrop.png)
![alt text](https://cdn.discordapp.com/attachments/859071008468238386/908131198935642152/Building.png)

- Step 5
Put created directory into `EXILED/Configs/MapEditorReborn/Schematics` folder.
After that, you can use `mp create schematicName` to spawn the schematic.
 ![alt text](https://cdn.discordapp.com/attachments/859071008468238386/908131208548974692/SpawnedInGame.png)