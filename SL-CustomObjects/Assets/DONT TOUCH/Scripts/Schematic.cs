using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#pragma warning disable CS0618

[ExecuteInEditMode]
public class Schematic : SchematicBlock
{
    public override BlockType BlockType => BlockType.Schematic;

    public void CompileSchematic()
    {
        string parentDirectoryPath = Config.ExportPath ??
                                     Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                         "MapEditorReborn_CompiledSchematics");
        string schematicDirectoryPath = Path.Combine(parentDirectoryPath, name);

        if (!Directory.Exists(parentDirectoryPath))
            Directory.CreateDirectory(parentDirectoryPath);

        if (Directory.Exists(schematicDirectoryPath))
            Directory.Delete(schematicDirectoryPath, true);

        if (File.Exists($"{schematicDirectoryPath}.zip"))
            File.Delete($"{schematicDirectoryPath}.zip");

        Directory.CreateDirectory(schematicDirectoryPath);

        int rootObjectId = transform.GetInstanceID();
        /*
        SchematicObjectDataList blockList = new SchematicObjectDataList(rootObjectId);
        Dictionary<int, SerializableRigidbody> rigidbodyDictionary = new Dictionary<int, SerializableRigidbody>();
        List<SerializableTeleport> teleports = new List<SerializableTeleport>();
        */
        BlockList.RootObjectId = rootObjectId;
        RigidbodyDictionary.Clear();
        Teleports.Clear();

        if (TryGetComponent(out Rigidbody rigidbody))
            RigidbodyDictionary.Add(rootObjectId, new SerializableRigidbody(rigidbody));

        // transform.localScale = Vector3.one;

        foreach (Transform obj in GetComponentsInChildren<Transform>())
        {
            if (obj.CompareTag("EditorOnly") || obj == transform)
                continue;

            int objectId = obj.transform.GetInstanceID();

            SchematicBlockData block = new SchematicBlockData
            {
                Name = obj.name,
                ObjectId = objectId,
                ParentId = obj.parent.GetInstanceID(),
                Position = Quaternion.Euler(obj.parent.eulerAngles) * obj.localPosition,
            };

            if (obj.TryGetComponent(out SchematicBlock schematicBlock))
            {
                if (!schematicBlock.Compile(block, this))
                    continue;

                //switch (schematicBlock.BlockType)
                //{
                /*
                case BlockType.Primitive:
                {
                    if (obj.TryGetComponent(out PrimitiveComponent primitiveComponent))
                    {
                        block.Rotation = obj.localEulerAngles;
                        Vector3 scaleAbs = new Vector3(Mathf.Abs(obj.localScale.x), Mathf.Abs(obj.localScale.y), Mathf.Abs(obj.localScale.z));
                        block.Scale = primitiveComponent.Collidable ? scaleAbs : scaleAbs * -1f;

                        block.BlockType = BlockType.Primitive;
                        block.Properties = new Dictionary<string, object>
                        {
                            { "PrimitiveType", (PrimitiveType)Enum.Parse(typeof(PrimitiveType), obj.tag) },
                            { "Color", ColorUtility.ToHtmlStringRGBA(primitiveComponent.Color) },
                    }

                    break;
                }
                */

                /*
                case BlockType.Pickup:
                {
                    if (obj.TryGetComponent(out PickupComponent pickupComponent))
                    {
                        block.Rotation = obj.localEulerAngles;
                        block.Scale = obj.localScale;

                        block.BlockType = BlockType.Pickup;
                        block.Properties = new Dictionary<string, object>
                        {
                            { "ItemType", pickupComponent.ItemType },
                            { "CustomItem", pickupComponent.CustomItem },
                            { "Attachments", pickupComponent.Attachments },
                            { "Chance", pickupComponent.Chance },
                            { "Uses", pickupComponent.NumberOfUses },
                        };

                        if (!pickupComponent.CanBePickedUp)
                            block.Properties.Add("Locked", string.Empty);
                    }

                    break;
                }
                */

                /*
                case BlockType.Workstation:
                {
                    if (obj.TryGetComponent(out WorkstationComponent workstationComponent))
                    {
                        block.Rotation = obj.localEulerAngles;
                        block.Scale = obj.localScale;

                        block.BlockType = BlockType.Workstation;
                        block.Properties = new Dictionary<string, object>
                        {
                            { "IsInteractable", workstationComponent.IsInteractable },
                        };
                    }

                    break;
                }
                */

                /*
                case BlockType.Schematic:
                {

                    break;
                }
                */

                /*
                case BlockType.Teleport:
                {
                    if (obj.TryGetComponent(out TeleportComponent teleport))
                    {
                        if (!teleport.ValidateList(teleport.TargetTeleporters))
                        {
                            Debug.LogError($"The teleport list for the {teleport.name} is invalid! ({name})");
                            return;
                        }

                        block.Rotation = obj.localEulerAngles;
                        block.Scale = obj.localScale;

                        SerializableTeleport serializableTeleport = new SerializableTeleport(block)
                        {
                            RoomType = teleport.RoomType,
                            TargetTeleporters = new List<TargetTeleporter>(teleport.TargetTeleporters.Count),
                            AllowedRoles = teleport.AllowedRoleTypes,
                            Cooldown = teleport.Cooldown,
                            TeleportSoundId = teleport.SoundOnTeleport,
                            TeleportFlags = teleport.TeleportFlags,
                            LockOnEvent = teleport.LockOnEvent,
                        };

                        if (!teleport.PlaySoundOnTeleport)
                            serializableTeleport.TeleportSoundId = -1;

                        if (teleport.OverridePlayerXRotation &&
                            teleport.TeleportFlags.HasFlag(TeleportFlags.Player))
                            serializableTeleport.PlayerRotationX = teleport.PlayerRotationX;

                        if (teleport.OverridePlayerYRotation &&
                            teleport.TeleportFlags.HasFlag(TeleportFlags.Player))
                            serializableTeleport.PlayerRotationY = teleport.PlayerRotationY;

                        for (int i = 0; i < teleport.TargetTeleporters.Count; i++)
                        {
                            if (teleport.TargetTeleporters[i].Teleporter == null)
                                continue;

                            teleport.TargetTeleporters[i].Id = teleport.TargetTeleporters[i].Teleporter.transform.GetInstanceID();
                            teleport.TargetTeleporters[i].Chance = teleport.TargetTeleporters[i].ChanceToTeleport;
                        }

                        serializableTeleport.TargetTeleporters = teleport.TargetTeleporters;

                        Teleports.Add(serializableTeleport);
                    }

                    continue;
                }
                */

                /*
                case BlockType.Locker:
                {
                    if (obj.TryGetComponent(out LockerComponent locker))
                    {
                        block.Rotation = obj.localEulerAngles;
                        block.Scale = obj.localScale;
                        block.BlockType = BlockType.Locker;

                        Dictionary<int, List<SerializableLockerItem>> chambers = new Dictionary<int, List<SerializableLockerItem>>(locker.Chambers.Count);
                        int i = 0;

                        foreach (LockerChamber chamber in locker.Chambers)
                        {
                            List<SerializableLockerItem> listOfItems = new List<SerializableLockerItem>(chamber.PossibleItems.Count);

                            foreach (LockerItem possibleItem in chamber.PossibleItems)
                            {
                                listOfItems.Add(new SerializableLockerItem(possibleItem));
                            }

                            chambers.Add(i, listOfItems);
                            i++;
                        }

                        block.Properties = new Dictionary<string, object>
                        {
                            { "LockerType", locker.LockerType },
                            { "Chambers", chambers },
                            { "ShuffleChambers", locker.ShuffleChambers },
                            { "AllowedRoleTypes", locker.AllowedRoleTypes },
                            { "KeycardPermissions", locker.KeycardPermissions },
                            { "OpenedChambers", locker.OpenedChambers },
                            { "InteractLock", locker.InteractLock },
                            { "Chance", locker.Chance },
                        };
                    }

                    break;
                
                }
                */
                // }
            }
            else
            {
                // Light
                if (obj.TryGetComponent(out Light lightComponent))
                {
                    block.BlockType = BlockType.Light;
                    block.Properties = new Dictionary<string, object>
                    {
                        { "Color", ColorUtility.ToHtmlStringRGBA(lightComponent.color) },
                        { "Intensity", lightComponent.intensity },
                        { "Range", lightComponent.range },
                        { "Shadows", lightComponent.shadows != LightShadows.None },
                    };
                }
                else // Empty transform
                {
                    obj.localScale = Vector3.one;

                    block.BlockType = BlockType.Empty;
                    block.Rotation = obj.localEulerAngles;
                }
            }

            if (obj.TryGetComponent(out Animator animator) && animator.runtimeAnimatorController != null)
            {
                RuntimeAnimatorController runtimeAnimatorController = animator.runtimeAnimatorController;
                block.AnimatorName = runtimeAnimatorController.name;

                BuildPipeline.BuildAssetBundle(runtimeAnimatorController,
                    runtimeAnimatorController.animationClips,
                    Path.Combine(schematicDirectoryPath, runtimeAnimatorController.name),
                    AssetBundleBuildOptions, EditorUserBuildSettings.activeBuildTarget);
            }

            if (obj.TryGetComponent(out rigidbody))
                RigidbodyDictionary.Add(objectId, new SerializableRigidbody(rigidbody));

            BlockList.Blocks.Add(block);
        }

        File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{name}.json"),
            JsonConvert.SerializeObject(BlockList, Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        if (RigidbodyDictionary.Count > 0)
            File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{name}-Rigidbodies.json"),
                JsonConvert.SerializeObject(RigidbodyDictionary, Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        if (Teleports.Count > 0)
            File.WriteAllText(Path.Combine(schematicDirectoryPath, $"{name}-Teleports.json"),
                JsonConvert.SerializeObject(Teleports, Formatting.Indented,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        if (Config.ZipCompiledSchematics)
        {
            System.IO.Compression.ZipFile.CreateFromDirectory(schematicDirectoryPath, $"{schematicDirectoryPath}.zip",
                System.IO.Compression.CompressionLevel.Optimal, true);
            Directory.Delete(schematicDirectoryPath, true);
        }

        Debug.Log($"{name} has been successfully compiled!");
    }

    public void Update()
    {
        if (transform.localScale == Vector3.one)
            return;

        transform.localScale = Vector3.one;
        Debug.LogAssertion("<color=red>Do not change the scale of the root object or any other empty transform!</color>");
    }

    // This is only used in nested schematics (schematics inside other schematics)
    public override bool Compile(SchematicBlockData block, Schematic _)
    {
        block.Rotation = transform.localEulerAngles;

        block.BlockType = BlockType.Schematic;
        block.Properties = new Dictionary<string, object>
        {
            { "SchematicName", name }
        };

        return false;
    }

    internal readonly SchematicObjectDataList BlockList = new SchematicObjectDataList();
    internal readonly Dictionary<int, SerializableRigidbody> RigidbodyDictionary = new Dictionary<int, SerializableRigidbody>();
    internal readonly List<SerializableTeleport> Teleports = new List<SerializableTeleport>();

    private static BuildAssetBundleOptions AssetBundleBuildOptions => BuildAssetBundleOptions.ChunkBasedCompression |
                                                                      BuildAssetBundleOptions.ForceRebuildAssetBundle |
                                                                      BuildAssetBundleOptions.StrictMode;

    private static readonly Config Config = SchematicManager.Config;
}