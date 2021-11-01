# SL-CustomObjects
 The project that allows you to create schematics out of in-game items in SCP: Secret Labortatory.
 
 ## Requirements
 - Unity Editor (the project is made in `Unity version 2020.3.0f1` but probably the version won't matter too much.
 - A couple of GBs of free space (probably 5 GB). The reason behind it is, when you first launch the project Unity needs to create the **Library** folder for faster loading times etc.
 - 
 Clone that repository and then open that cloned repo via unity editor
 ![alt text](https://cdn.discordapp.com/attachments/686969243782086702/828575872579403846/unknown.png)
  
 First add your project via, Add button ( find path of that cloned repo on your pc )
 
 ![alt text](https://cdn.discordapp.com/attachments/686969243782086702/828576157380902992/unknown.png)
 
 In Assets you should have CustomObjects scene, open that scene and do your stuff....
 
 ![alt text](https://cdn.discordapp.com/attachments/686969243782086702/828577094085771324/unknown.png)
 
 Enter playmode to create schematics in Assets/Schematics
 
 ![alt text](https://cdn.discordapp.com/attachments/686969243782086702/828577303519952906/unknown.png)
 
 On your server put that schematic file in Exiled/Plugins/MapEditor/schematics and then on server type
 
` mapeditor setcustomobject <schematicName> ( without schematic- )`
 
 
 If you wan to use that plugin in your one then add that plugin as reference and check
 
` MapEditor.Schematic class`
 
 Example schematics ingame:
 ![alt text](https://cdn.discordapp.com/attachments/675862006057664513/828568118296313867/unknown.png)
 ![alt text](https://cdn.discordapp.com/attachments/675862006057664513/828565186935390238/unknown.png)
 ![alt text](https://cdn.discordapp.com/attachments/675862006057664513/828560294371524618/unknown.png)
