using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

public static class Decompiler
{
    [MenuItem("SchematicManager/Import Schematic")]
    private static void PortBack()
    {
        string inportPath = SchematicManager.Config.ExportPath;
        if (!Directory.Exists(inportPath))
            Directory.CreateDirectory(inportPath);

        _schematicDirectoryPath = EditorUtility.OpenFolderPanel("Select folder with the schematic", inportPath, "");
        if (string.IsNullOrEmpty(_schematicDirectoryPath))
        {
            Debug.LogError("Invalid schematic directory. Path is empty.");
            return;
        }

        _schematicName = Path.GetFileName(_schematicDirectoryPath);
        string jsonFilePath = Path.Combine(_schematicDirectoryPath, $"{_schematicName}.json");
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError("No json file found in the schematic directory!");
            return;
        }

        _blockPrefabs = Resources.LoadAll<GameObject>("Blocks").ToList();
        _schematicData = JsonConvert.DeserializeObject<SchematicObjectDataList>(File.ReadAllText(jsonFilePath));

        _rootTransform = new GameObject(_schematicName).AddComponent<Schematic>().transform;
        _objectFromId = new Dictionary<int, Transform>(_schematicData.Blocks.Count + 1)
        {
            { _schematicData.RootObjectId, _rootTransform },
        };

        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
        Debug.Log("Importing schematic...");
        
        CreateRecursiveFromID(_schematicData.RootObjectId, _schematicData.Blocks, _rootTransform);
        CreateTeleporters();
        AddRigidbodies();

        Debug.Log($"Successfully imported {_schematicName} schematic in {stopwatch.ElapsedMilliseconds} ms!");
        NullifyFields();
    }

    private static void CreateRecursiveFromID(int id, List<SchematicBlockData> blocks, Transform parentGameObject)
    {
        Transform childGameObjectTransform = CreateObject(blocks.Find(c => c.ObjectId == id), parentGameObject) ?? _rootTransform; // Create the object first before creating children.
        int[] parentSchematics = blocks.Where(bl => bl.BlockType == BlockType.Schematic).Select(bl => bl.ObjectId).ToArray();

        // Gets all the ObjectIds of all the schematic blocks inside "blocks" argument.
        foreach (SchematicBlockData block in blocks.FindAll(c => c.ParentId == id))
        {
            if (parentSchematics.Contains(block.ParentId)) // The block is a child of some schematic inside "parentSchematics" array, therefore it will be skipped to avoid spawning it and its children twice.
                continue;

            CreateRecursiveFromID(block.ObjectId, blocks, childGameObjectTransform); // The child now becomes the parent
        }
    }

    private static Transform CreateObject(SchematicBlockData block, Transform rootObject)
    {
        if (block == null)
            return null;

        GameObject gameObject = null;
        RuntimeAnimatorController animatorController;
        SerializableRigidbody serializableRigidbody;

        switch (block.BlockType)
        {
            case BlockType.Empty:
                {
                    gameObject = new GameObject(block.Name);
                    gameObject.transform.parent = rootObject;
                    gameObject.transform.localPosition = block.Position;

                    _objectFromId.Add(block.ObjectId, gameObject.transform);

                    break;
                }

            case BlockType.Primitive:
                {
                    object primtype = Enum.Parse(typeof(PrimitiveType), block.Properties["PrimitiveType"].ToString());
                    GameObject primBase = _blockPrefabs.FirstOrDefault(s => s.name == primtype.ToString());
                    gameObject = Object.Instantiate(primBase, rootObject);
                    gameObject.name = block.Name;
                    gameObject.transform.localPosition = block.Position;
                    gameObject.transform.localEulerAngles = block.Rotation;
                    gameObject.transform.localScale = new Vector3(Mathf.Abs(block.Scale.x), Mathf.Abs(block.Scale.y), Mathf.Abs(block.Scale.z));

                    if (gameObject.TryGetComponent(out PrimitiveComponent primitiveComponent))
                    {
                        primitiveComponent.Collidable = block.Scale.x >= 0f;
                        
                        if (block.Properties != null)
                        {
                            if (ColorUtility.TryParseHtmlString("#" + block.Properties["Color"], out Color color))
                            {
                                primitiveComponent.Color = color;
                                Renderer _renderer = gameObject.GetComponent<Renderer>();
                                Material shared = color.a >= 1f ? new Material((Material)Resources.Load("Materials/Regular")) : new Material((Material)Resources.Load("Materials/Transparent"));
                                _renderer.sharedMaterial = shared;
                                _renderer.sharedMaterial.color = color;
                            }
                            else
                            {
                                Debug.LogWarning($"Couldn't parse {block.Properties["Color"]} as unity color");
                            }
                        }

                        _objectFromId.Add(block.ObjectId, gameObject.transform);
                    }

                    break;
                }

            case BlockType.Light:
                {
                    GameObject baseObject = _blockPrefabs.FirstOrDefault(s => s.name == "LightSource");
                    gameObject = Object.Instantiate(baseObject, rootObject);
                    gameObject.name = block.Name;
                    gameObject.transform.localPosition = block.Position;

                    if (gameObject.TryGetComponent(out Light lightComponent))
                    {
                        bool canParse =
                            ColorUtility.TryParseHtmlString("#" + block.Properties["Color"].ToString(), out Color color);
                        if (canParse)
                        {
                            lightComponent.color = color;
                        }
                        else
                        {
                            Debug.LogWarning($"Couldn't parse {block.Properties["Color"]} as unity color");
                        }

                        if (block.Properties != null)
                        {
                            lightComponent.intensity = float.Parse(block.Properties["Intensity"].ToString());
                            lightComponent.range = float.Parse(block.Properties["Range"].ToString());
                            lightComponent.shadows = bool.Parse(block.Properties["Shadows"].ToString())
                                ? LightShadows.Soft
                                : LightShadows.None;
                        }

                        _objectFromId.Add(block.ObjectId, gameObject.transform);
                    }

                    return gameObject.transform;
                }

            case BlockType.Pickup:
                {
                    GameObject basePickup = _blockPrefabs.FirstOrDefault(s => s.name == "Pickup");
                    gameObject = Object.Instantiate(basePickup, rootObject);
                    gameObject.name = block.Name;
                    gameObject.transform.localPosition = block.Position;
                    gameObject.transform.localEulerAngles = block.Rotation;
                    gameObject.transform.localScale = block.Scale;

                    if (gameObject.TryGetComponent(out PickupComponent pickupComponent) && block.Properties != null)
                    {
                        pickupComponent.ItemType = (ItemType)Enum.Parse(typeof(ItemType), block.Properties["ItemType"].ToString());
                        pickupComponent.CanBePickedUp = !block.Properties.ContainsKey("Locked");
                        pickupComponent.Chance = float.Parse(block.Properties["Chance"].ToString());
                    }

                    _objectFromId.Add(block.ObjectId, gameObject.transform);

                    return gameObject.transform;
                }

            case BlockType.Workstation:
                {
                    GameObject workstationBase = _blockPrefabs.FirstOrDefault(s => s.name == "Workstation");
                    gameObject = Object.Instantiate(workstationBase, rootObject);
                    gameObject.name = block.Name;
                    gameObject.transform.localPosition = block.Position;
                    gameObject.transform.localEulerAngles = block.Rotation;
                    gameObject.transform.localScale = block.Scale;

                    if (gameObject.TryGetComponent(out WorkstationComponent workstationComponent) && block.Properties != null)
                        workstationComponent.IsInteractable = bool.Parse(block.Properties["IsInteractable"].ToString());

                    _objectFromId.Add(block.ObjectId, gameObject.transform);

                    return gameObject.transform;
                }

            case BlockType.Locker:
            {
                object lockerType =  Enum.Parse(typeof(LockerType), block.Properties["LockerType"].ToString());
                GameObject lockerBase = _blockPrefabs.FirstOrDefault(s => s.name.Contains(lockerType.ToString()));
                gameObject = Object.Instantiate(lockerBase, rootObject);
                gameObject.name = block.Name;
                gameObject.transform.localPosition = block.Position;
                gameObject.transform.localEulerAngles = block.Rotation;
                gameObject.transform.localScale = block.Scale;
                
                if (gameObject.TryGetComponent(out LockerComponent lockerComponent) && block.Properties != null)
                {
                    Dictionary<int, List<SerializableLockerItem>> dict = JsonConvert.DeserializeObject<Dictionary<int, List<SerializableLockerItem>>>(JsonConvert.SerializeObject(block.Properties["Chambers"]));
                    lockerComponent.Chambers.Clear();
                    
                    for (int i = 0; i < dict.Count; i++)
                    {
                        List<LockerItem> possibleItems = dict[i].Select(item => new LockerItem(item)).ToList();

                        LockerChamber lockerChamber = new LockerChamber
                        {
                            PossibleItems = possibleItems
                        };
                        lockerComponent.Chambers.Add(lockerChamber);
                    }

                    lockerComponent.AllowedRoleTypes = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(block.Properties["AllowedRoleTypes"]));
                    lockerComponent.ShuffleChambers = bool.Parse(block.Properties["ShuffleChambers"].ToString());
                    lockerComponent.KeycardPermissions = (KeycardPermissions)Enum.Parse(typeof(KeycardPermissions), block.Properties["KeycardPermissions"].ToString());
                    lockerComponent.OpenedChambers = ushort.Parse(block.Properties["OpenedChambers"].ToString());
                    lockerComponent.InteractLock = bool.Parse(block.Properties["InteractLock"].ToString());
                    lockerComponent.Chance = float.Parse(block.Properties["Chance"].ToString());
                }
                
                return gameObject.transform;
            }
        }

        if (TryGetAnimatorController(block.AnimatorName, out animatorController))
            gameObject.AddComponent<Animator>().runtimeAnimatorController = animatorController;

        return gameObject.transform;
    }

    private static bool TryGetAnimatorController(string animatorName, out RuntimeAnimatorController animatorController)
    {
        animatorController = null;

        if (!string.IsNullOrEmpty(animatorName))
        {
            Object animatorObject = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(x => x.mainAsset.name == animatorName)?.LoadAllAssets().First(x => x is RuntimeAnimatorController);

            if (animatorObject == null)
            {
                string path = Path.Combine(_schematicDirectoryPath, animatorName);

                if (!File.Exists(path))
                    return false;

                animatorObject = AssetBundle.LoadFromFile(path).LoadAllAssets().First(x => x is RuntimeAnimatorController);
            }

            animatorController = animatorObject as RuntimeAnimatorController;
            return true;
        }

        return false;
    }

    private static void CreateTeleporters()
    {
        string teleportPath = Path.Combine(_schematicDirectoryPath, $"{_schematicName}-Teleports.json");
        if (!File.Exists(teleportPath))
            return;

        foreach (SerializableTeleport teleport in JsonConvert.DeserializeObject<List<SerializableTeleport>>(File.ReadAllText(teleportPath)))
        {
            GameObject gameObject = Object.Instantiate(_blockPrefabs.FirstOrDefault(x => x.name == "Teleporter"));
            gameObject.name = teleport.Name;
            gameObject.transform.parent = _objectFromId[teleport.ParentId];
            gameObject.transform.localPosition = teleport.Position;
            gameObject.transform.localEulerAngles = teleport.Rotation;
            gameObject.transform.localScale = teleport.Scale;

            if (gameObject.TryGetComponent(out TeleportComponent teleportComponent))
            {
                teleportComponent.TargetTeleporters = teleport.TargetTeleporters;
                teleportComponent.RoomType = teleport.RoomType;
                teleportComponent.AllowedRoleTypes = teleport.AllowedRoles;
                teleportComponent.Cooldown = teleport.Cooldown;
                teleportComponent.TeleportFlags = teleport.TeleportFlags;
                teleportComponent.LockOnEvent = teleport.LockOnEvent;
                teleportComponent.SoundOnTeleport = teleport.TeleportSoundId;

                if (teleport.PlayerRotationX.HasValue)
                {
                    teleportComponent.OverridePlayerXRotation = true;
                    teleportComponent.PlayerRotationX = teleport.PlayerRotationX.Value;
                }

                if (teleport.PlayerRotationY.HasValue)
                {
                    teleportComponent.OverridePlayerYRotation = true;
                    teleportComponent.PlayerRotationY = teleport.PlayerRotationY.Value;
                }
            }

            _objectFromId.Add(teleport.ObjectId, gameObject.transform);
        }

        foreach (TeleportComponent teleport in _rootTransform.GetComponentsInChildren<TeleportComponent>())
        {
            foreach (TargetTeleporter targetTeleporter in teleport.TargetTeleporters)
            {
                targetTeleporter.Teleporter = _objectFromId[targetTeleporter.Id].GetComponent<TeleportComponent>();
            }
        }
    }

    private static void AddRigidbodies()
    {
        string rigidbodyPath = Path.Combine(_schematicDirectoryPath, $"{_schematicName}-Rigidbodies.json");
        if (!File.Exists(rigidbodyPath))
            return;

        foreach (KeyValuePair<int, SerializableRigidbody> dict in JsonConvert.DeserializeObject<Dictionary<int, SerializableRigidbody>>(File.ReadAllText(rigidbodyPath)))
        {
            if (!_objectFromId[dict.Key].gameObject.TryGetComponent(out Rigidbody rigidbody))
                rigidbody = _objectFromId[dict.Key].gameObject.AddComponent<Rigidbody>();

            rigidbody.isKinematic = dict.Value.IsKinematic;
            rigidbody.useGravity = dict.Value.UseGravity;
            rigidbody.constraints = dict.Value.Constraints;
            rigidbody.mass = dict.Value.Mass;
        }
    }

    private static void NullifyFields()
    {
        _blockPrefabs = null;
        _rootTransform = null;
        _schematicName = null;
        _schematicDirectoryPath = null;
        _schematicData = null;
        _objectFromId = null;
        AssetBundle.UnloadAllAssetBundles(false);
    }

    private static List<GameObject> _blockPrefabs;
    private static Transform _rootTransform;
    private static string _schematicName;
    private static string _schematicDirectoryPath;
    private static SchematicObjectDataList _schematicData;
    private static Dictionary<int, Transform> _objectFromId;
}

