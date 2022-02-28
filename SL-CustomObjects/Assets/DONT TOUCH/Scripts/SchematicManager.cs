using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SchematicManager : EditorWindow
{
    static SchematicManager()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }

    public static SchematicManager Instance { get; private set; }

    [MenuItem("SchematicManager/Compile all _F6")]
    private static void CompileAll()
    {
        Instance = GetWindow<SchematicManager>("SchematicManager", false);
        Debug.ClearDeveloperConsole();

        foreach (Schematic schematic in FindObjectsOfType<Schematic>())
        {
            schematic.CompileSchematic();
        }

        if (Instance.OpenDirectoryAfterCompilying)
            OpenDirectory();
    }

    [MenuItem("SchematicManager/Open schematics directory")]
    private static void OpenDirectory()
    {
        Instance = GetWindow<SchematicManager>("SchematicManager", false);

        if (!Directory.Exists(Instance.ExportPath))
            Directory.CreateDirectory(Instance.ExportPath);

        System.Diagnostics.Process.Start(Instance.ExportPath);
    }

    private static void LogPlayModeState(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
            CompileAll();
    }

    [MenuItem("SchematicManager/Settings")]
    private static void ShowWindow() => Instance = GetWindow<SchematicManager>("SchematicManager");

    private void OnGUI()
    {
        GUILayout.Label("Settings", EditorStyles.boldLabel);
        OpenDirectoryAfterCompilying = EditorGUILayout.ToggleLeft("Open schematics directory after compiling", OpenDirectoryAfterCompilying);

        GUILayout.Label($"Output path: {ExportPath}", EditorStyles.largeLabel);

        if (GUILayout.Button("Change output directory"))
        {
            string path = EditorUtility.OpenFolderPanel("Select output path", "", "");

            if (!string.IsNullOrEmpty(path))
                ExportPath = path;
        }

        if (string.IsNullOrEmpty(ExportPath))
            ExportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics");
    }
    
    [MenuItem("SchematicManager/Import Schematic")]
    private static void PortBack()
    {

        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var mapData = EditorUtility.OpenFilePanel("Select map json", dir, "json");
        if (string.IsNullOrEmpty(mapData))
            return;
        var gobjOld = GameObject.Find(mapData.Split('/')[mapData.Split('/').Length - 1].Split('.')[0]);
        DestroyImmediate(gobjOld);
        var gobj = new GameObject();
        gobj.name = mapData.Split('/')[mapData.Split('/').Length - 1].Split('.')[0];
        gobj.AddComponent<Schematic>();
        mapData = File.ReadAllText(mapData);

        SchematicObjectDataList list = JsonConvert.DeserializeObject<SchematicObjectDataList>(mapData);
        Primitives = new List<GameObject>();
        NormalObjects = new List<GameObject>();
        foreach (var file in Directory.GetFiles(Application.dataPath + "/Blocks/Primitives")) //workaround for prefabs not being in the Resources folder
        {
            if (!file.EndsWith(".prefab"))
                continue;
            Primitives.Add(AssetDatabase.LoadAssetAtPath<GameObject>(file.Replace(Application.dataPath, "Assets")));
        }
        foreach (var file in Directory.GetFiles(Application.dataPath + "/Blocks"))
        {
            if (!file.EndsWith(".prefab"))
                continue;
            NormalObjects.Add(AssetDatabase.LoadAssetAtPath<GameObject>(file.Replace(Application.dataPath, "Assets")));
        }
        RootGameObject = gobj.transform;
        SchematicData = list;
        CreateRecursiveFromID(list.RootObjectId, list.Blocks, gobj.transform);
    }
    
    private static void CreateRecursiveFromID(int id, List<SchematicBlockData> blocks, Transform parentGameObject)
    {
        Transform childGameObjectTransform = CreateObject(SchematicData.Blocks.Find(c => c.ObjectId == id), parentGameObject) ?? RootGameObject;
        if (childGameObjectTransform == null)
            return;
        foreach (SchematicBlockData block in SchematicData.Blocks.FindAll(c => c.ParentId == id))
        {
            CreateRecursiveFromID(block.ObjectId, blocks, childGameObjectTransform);
        }
    }

    private static Transform CreateObject(SchematicBlockData @object, Transform rootObject)
    {
        if (@object == null)
            return null;
        switch (@object.BlockType)
        {
            case BlockType.Primitive:
                var primtype = Enum.Parse(typeof(PrimitiveType), @object.Properties["PrimitiveType"].ToString());
                var primBase = Primitives.FirstOrDefault(s => s.name == primtype.ToString());
                var prim = Instantiate(primBase, rootObject);
                if (prim.TryGetComponent(out PrimitiveComponent primitiveComponent))
                {
                    prim.transform.localPosition = @object.Position;
                    prim.name = @object.Name;
                    prim.transform.localEulerAngles = @object.Rotation;
                    prim.transform.localScale = @object.Scale;
                    if (@object.Properties != null)
                    {
                        bool canParse = ColorUtility.TryParseHtmlString("#" + @object.Properties["Color"].ToString(),
                            out var color);
                        if (canParse)
                        {
                            primitiveComponent.Color = color;
                            var _renderer = prim.GetComponent<Renderer>();
                            Material shared = null;
                            if (color.a >= 1f)
                                shared = new Material((Material)Resources.Load("Materials/Regular"));
                            else
                                shared = new Material((Material)Resources.Load("Materials/Transparent"));
                            _renderer.sharedMaterial = shared;
                            _renderer.sharedMaterial.color = color;
                        }
                        else
                        {
                            Debug.LogWarning($"Couldnt parse {@object.Properties["Color"].ToString()} as unity color");
                        }
                    }
                }

                return prim.transform;
            case BlockType.Light:
                var baseObject = NormalObjects.FirstOrDefault(s => s.name == "LightSource");
                var lightObject = Instantiate(baseObject, rootObject);
                if (lightObject.TryGetComponent(out Light lightComponent))
                {
                    lightObject.transform.localPosition = @object.Position;
                    lightObject.name = @object.Name;
                    bool canParse =
                        ColorUtility.TryParseHtmlString("#" + @object.Properties["Color"].ToString(), out var color);
                    if (canParse)
                    {
                        lightComponent.color = color;
                    }
                    else
                    {
                        Debug.LogWarning($"Couldnt parse {@object.Properties["Color"].ToString()} as unity color");
                    }

                    if (@object.Properties != null)
                    {
                        lightComponent.intensity = Single.Parse(@object.Properties["Intensity"].ToString());
                        lightComponent.range = Single.Parse(@object.Properties["Range"].ToString());
                        lightComponent.shadows = bool.Parse(@object.Properties["Shadows"].ToString())
                            ? LightShadows.Soft
                            : LightShadows.None;
                    }
                }

                return lightObject.transform;
            case BlockType.Pickup:
                var basePickup = NormalObjects.FirstOrDefault(s => s.name == "Pickup");
                var pickupObject = Instantiate(basePickup, rootObject);
                if (pickupObject.TryGetComponent(out PickupComponent pickupComponent))
                {
                    pickupObject.transform.localPosition = @object.Position;
                    pickupObject.name = @object.Name;
                    pickupObject.transform.localEulerAngles = @object.Rotation;
                    pickupObject.transform.localScale = @object.Scale;

                    if (@object.Properties != null)
                    {
                        pickupComponent.ItemType =
                            (ItemType)Enum.Parse(typeof(ItemType), @object.Properties["ItemType"].ToString());
                        pickupComponent.UseGravity = !@object.Properties.ContainsKey("Kinematic");
                        pickupComponent.CanBePickedUp = !@object.Properties.ContainsKey("Locked");
                    }
                }
                
                return pickupObject.transform;
            case BlockType.Workstation:
                var workstationBase = NormalObjects.FirstOrDefault(s => s.name == "Workstation");
                var workstationObject = Instantiate(workstationBase, rootObject);
                if (workstationObject.TryGetComponent(out WorkstationComponent workstationComponent))
                {
                    workstationObject.transform.localPosition = @object.Position;
                    workstationObject.name = @object.Name;
                    workstationObject.transform.localEulerAngles = @object.Rotation;
                    workstationObject.transform.localScale = @object.Scale;

                    if (@object.Properties != null)
                        workstationComponent.IsInteractable =
                            bool.Parse(@object.Properties["IsInteractable"].ToString());
                }

                return workstationObject.transform;
            
            case BlockType.Empty:
                var emptyObject = new GameObject();
                emptyObject.name = @object.Name;
                emptyObject.transform.parent = rootObject;
                emptyObject.transform.localPosition = @object.Position;
                return emptyObject.transform;
            break;    
        }

        return null;
    }

    [SerializeField]
    public bool OpenDirectoryAfterCompilying;

    [SerializeField]
    public string ExportPath;

    public static Transform RootGameObject;
    public static SchematicObjectDataList SchematicData;
    public static List<GameObject> Primitives;
    public static List<GameObject> NormalObjects;
}
