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
        string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MapEditorReborn_CompiledSchematics");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string mapData = EditorUtility.OpenFilePanel("Select map json", dir, "json");
        if (string.IsNullOrEmpty(mapData))
            return;

        GameObject gobjOld = GameObject.Find(mapData.Split('/')[mapData.Split('/').Length - 1].Split('.')[0]);
        DestroyImmediate(gobjOld);
        GameObject gobj = new GameObject();
        gobj.name = mapData.Split('/')[mapData.Split('/').Length - 1].Split('.')[0];
        gobj.AddComponent<Schematic>();
        mapData = File.ReadAllText(mapData);

        SchematicObjectDataList list = JsonConvert.DeserializeObject<SchematicObjectDataList>(mapData);
        _primitives = new List<GameObject>();
        _normalObjects = new List<GameObject>();
        foreach (var file in Directory.GetFiles(Application.dataPath + "/Blocks/Primitives")) //workaround for prefabs not being in the Resources folder
        {
            if (!file.EndsWith(".prefab"))
                continue;
            _primitives.Add(AssetDatabase.LoadAssetAtPath<GameObject>(file.Replace(Application.dataPath, "Assets")));
        }
        foreach (var file in Directory.GetFiles(Application.dataPath + "/Blocks"))
        {
            if (!file.EndsWith(".prefab"))
                continue;
            _normalObjects.Add(AssetDatabase.LoadAssetAtPath<GameObject>(file.Replace(Application.dataPath, "Assets")));
        }
        _rootGameObject = gobj.transform;
        _schematicData = list;
        CreateRecursiveFromID(list.RootObjectId, list.Blocks, gobj.transform);
    }
    
    private static void CreateRecursiveFromID(int id, List<SchematicBlockData> blocks, Transform parentGameObject)
    {
        Transform childGameObjectTransform = CreateObject(_schematicData.Blocks.Find(c => c.ObjectId == id), parentGameObject) ?? _rootGameObject;
        if (childGameObjectTransform == null)
            return;
        foreach (SchematicBlockData block in _schematicData.Blocks.FindAll(c => c.ParentId == id))
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
                {
                    object primtype = Enum.Parse(typeof(PrimitiveType), @object.Properties["PrimitiveType"].ToString());
                    GameObject primBase = _primitives.FirstOrDefault(s => s.name == primtype.ToString());
                    GameObject prim = Instantiate(primBase, rootObject);
                    if (prim.TryGetComponent(out PrimitiveComponent primitiveComponent))
                    {
                        prim.transform.localPosition = @object.Position;
                        prim.name = @object.Name;
                        prim.transform.localEulerAngles = @object.Rotation;
                        prim.transform.localScale = @object.Scale;
                        if (@object.Properties != null)
                        {
                            bool canParse = ColorUtility.TryParseHtmlString("#" + @object.Properties["Color"].ToString(),
                                out Color color);
                            if (canParse)
                            {
                                primitiveComponent.Color = color;
                                Renderer _renderer = prim.GetComponent<Renderer>();
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
                                Debug.LogWarning($"Couldn't parse {@object.Properties["Color"]} as unity color");
                            }
                        }
                    }

                    return prim.transform;
                }

            case BlockType.Light:
                {
                    GameObject baseObject = _normalObjects.FirstOrDefault(s => s.name == "LightSource");
                    GameObject lightObject = Instantiate(baseObject, rootObject);
                    if (lightObject.TryGetComponent(out Light lightComponent))
                    {
                        lightObject.transform.localPosition = @object.Position;
                        lightObject.name = @object.Name;
                        bool canParse =
                            ColorUtility.TryParseHtmlString("#" + @object.Properties["Color"].ToString(), out Color color);
                        if (canParse)
                        {
                            lightComponent.color = color;
                        }
                        else
                        {
                            Debug.LogWarning($"Couldn't parse {@object.Properties["Color"]} as unity color");
                        }

                        if (@object.Properties != null)
                        {
                            lightComponent.intensity = float.Parse(@object.Properties["Intensity"].ToString());
                            lightComponent.range = float.Parse(@object.Properties["Range"].ToString());
                            lightComponent.shadows = bool.Parse(@object.Properties["Shadows"].ToString())
                                ? LightShadows.Soft
                                : LightShadows.None;
                        }
                    }

                    return lightObject.transform;
                }

            case BlockType.Pickup:
                {
                    GameObject basePickup = _normalObjects.FirstOrDefault(s => s.name == "Pickup");
                    GameObject pickupObject = Instantiate(basePickup, rootObject);
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
                }

            case BlockType.Workstation:
                {
                    GameObject workstationBase = _normalObjects.FirstOrDefault(s => s.name == "Workstation");
                    GameObject workstationObject = Instantiate(workstationBase, rootObject);
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
                }
            
            case BlockType.Empty:
                {
                    GameObject emptyObject = new GameObject(@object.Name);
                    emptyObject.transform.parent = rootObject;
                    emptyObject.transform.localPosition = @object.Position;

                    return emptyObject.transform;
                }
        }

        return null;
    }

    [SerializeField]
    public bool OpenDirectoryAfterCompilying;

    [SerializeField]
    public string ExportPath;

    private static Transform _rootGameObject;
    private static SchematicObjectDataList _schematicData;
    private static List<GameObject> _primitives;
    private static List<GameObject> _normalObjects;
}
