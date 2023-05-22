// -----------------------------------------------------------------------
// <copyright file="SchematicObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MapEditorReborn.Exiled.Features;
using MapEditorReborn.Exiled.Features.Items;
using MapEditorReborn.Exiled.Features.Pickups;
using PluginAPI.Core;

namespace MapEditorReborn.API.Features.Objects;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using AdminToys;
using Configs;
using Enums;
using Extensions;
using MEC;
using Mirror;
using Serializable;
using UnityEngine;
using Utf8Json;
using Random = UnityEngine.Random;

/// <summary>
/// Component added to SchematicObject. Is is used for easier idendification of the object and it's variables.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class SchematicObject : MapEditorObject
{
    /// <summary>
    /// Initializes the <see cref="SchematicObject"/>.
    /// </summary>
    /// <param name="schematicSerializable">The <see cref="SchematicSerializable"/> to instantiate.</param>
    /// <param name="data">The object data from a file.</param>
    /// <returns>Instance of this component.</returns>
    public SchematicObject Init(SchematicSerializable schematicSerializable, SchematicObjectDataList data)
    {
        Base = schematicSerializable;
        Log.Info(Base.SchematicName);
        
        SchematicData = data;
        DirectoryPath = data.Path;
        Log.Info(DirectoryPath);
        
        ForcedRoomType = schematicSerializable.RoomType != Exiled.Enums.RoomType.Unknown ? schematicSerializable.RoomType : FindRoom().Type;
        Log.Info(ForcedRoomType.ToString());
        
        ObjectFromId = new Dictionary<int, Transform>(SchematicData.Blocks.Count + 1)
        {
            { data.RootObjectId, transform },
        };

        CreateRecursiveFromID(data.RootObjectId, data.Blocks, transform);
        CreateTeleporters();
        AddRigidbodies();

        if (Math.Abs(MapEditorReborn.Singleton.Config.SchematicBlockSpawnDelay - (-1f)) > 0)
        {
            Timing.RunCoroutine(SpawnDelayed());
        }
        else
        {
            foreach (var networkIdentity in NetworkIdentities)
                NetworkServer.Spawn(networkIdentity.gameObject);

            AddAnimators();
            IsBuilt = true;
        }

        AttachedBlocks.CollectionChanged += OnCollectionChanged;
        UpdateObject();

        return this;
    }

    /// <summary>
    /// The base config of the object which contains its properties.
    /// </summary>
    public SchematicSerializable Base;

    /// <summary>
    /// Gets a <see cref="SchematicObjectDataList"/> used to build a schematic.
    /// </summary>
    public SchematicObjectDataList SchematicData { get; private set; }

    /// <summary>
    /// Gets a schematic directory path.
    /// </summary>
    public string DirectoryPath { get; private set; }

    /// <summary>
    /// Gets a <see cref="List{T}"/> of <see cref="GameObject"/> which contains all attached blocks.
    /// </summary>
    public ObservableCollection<GameObject> AttachedBlocks { get; private set; } = new();

    /// <summary>
    /// Gets the original position.
    /// </summary>
    public Vector3 OriginalPosition { get; private set; }

    /// <summary>
    /// Gets the original rotation.
    /// </summary>
    public Vector3 OriginalRotation { get; private set; }

    /// <summary>
    /// Gets the schematic name.
    /// </summary>
    public string Name
    {
        get => Base.SchematicName;
    }

    public AnimationController AnimationController
    {
        get => AnimationController.Get(this);
    }

    /// <summary>
    /// Gets a value indicating whether this schematic is at top of transform hierarchy.
    /// </summary>
    public bool IsRootSchematic
    {
        get => transform.root == transform;
    }

    /// <summary>
    /// Gets the read-only collections of <see cref="NetworkIdentity"/> in this schematic.
    /// </summary>
    public ReadOnlyCollection<NetworkIdentity> NetworkIdentities
    {
        get
        {
            if (_networkIdentities != null) return _networkIdentities;
            
            List<NetworkIdentity> list = new();

            foreach (var block in AttachedBlocks)
            {
                if (block.TryGetComponent(out NetworkIdentity networkIdentity))
                {
                    list.Add(networkIdentity);
                }
            }

            _networkIdentities = list.AsReadOnly();

            return _networkIdentities;
        }
    }

    /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
    public override void UpdateObject()
    {
        if (IsRootSchematic && Base.SchematicName != name.Split(new[] { '-' })[1])
        {
            var newObject = ObjectSpawner.SpawnSchematic(Base, transform.position, transform.rotation, transform.localScale);

            if (newObject != null)
            {
                API.SpawnedObjects[API.SpawnedObjects.IndexOf(this)] = newObject;

                Destroy();
                return;
            }

            Base.SchematicName = name.Replace("CustomSchematic-", string.Empty);
        }

        OriginalPosition = RelativePosition;
        OriginalRotation = RelativeRotation;

        foreach (var gameObject in AttachedBlocks)
        {
            if (gameObject.TryGetComponent(out WorkstationObject _))
            {
                NetworkServer.UnSpawn(gameObject);

                var block = SchematicData.Blocks.Find(c => c.ObjectId == _transformProperties[gameObject.transform.GetInstanceID()]);
                gameObject.transform.position = transform.position + block.Position;
                gameObject.transform.eulerAngles = transform.eulerAngles + block.Rotation;
                gameObject.transform.localScale = Vector3.Scale(transform.localScale, block.Scale);

                NetworkServer.Spawn(gameObject);

                continue;
            }

            if (gameObject.TryGetComponent(out LockerObject locker))
            {
                locker.UpdateObject();
                continue;
            }

            if (gameObject.TryGetComponent(out TeleportObject teleport))
                teleport.FixTransform();
        }

        if (!IsRootSchematic)
            return;

        //Timing.CallDelayed(0.1f, () => Patches.OverridePositionPatch.ResetValues());
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => _networkIdentities = null;

    private void CreateRecursiveFromID(int id, List<SchematicBlockData> blocks, Transform parentGameObject)
    {
        var childGameObjectTransform = CreateObject(blocks.Find(c => c.ObjectId == id), parentGameObject) ?? transform; // Create the object first before creating children.
        var parentSchematics = blocks.Where(bl => bl.BlockType == BlockType.Schematic).Select(bl => bl.ObjectId).ToArray();

        // Gets all the ObjectIds of all the schematic blocks inside "blocks" argument.
        foreach (var block in blocks.FindAll(c => c.ParentId == id))
        {
            if (parentSchematics.Contains(block.ParentId)) // The block is a child of some schematic inside "parentSchematics" array, therefore it will be skipped to avoid spawning it and its children twice.
                continue;

            CreateRecursiveFromID(block.ObjectId, blocks, childGameObjectTransform); // The child now becomes the parent
        }
    }

    private Transform CreateObject(SchematicBlockData block, Transform parentTransform)
    {
        if (block == null)
            return null;

        GameObject gameObject = null;
        RuntimeAnimatorController animatorController;

        Log.Info($"{block.BlockType.ToString()} {block.Name}");
        switch (block.BlockType)
        {
            case BlockType.Empty:
            {
                gameObject = new GameObject(block.Name)
                {
                    transform =
                    {
                        parent = parentTransform,
                        localPosition = block.Position,
                        localEulerAngles = block.Rotation,
                    },
                };

                break;
            }

            case BlockType.Primitive:
            {
                if (Instantiate(ObjectType.Primitive.GetObjectByMode(), parentTransform).TryGetComponent(out PrimitiveObjectToy primitiveToy))
                    gameObject = primitiveToy.gameObject.AddComponent<PrimitiveObject>().Init(block).gameObject;

                break;
            }

            case BlockType.Light:
            {
                if (Instantiate(ObjectType.LightSource.GetObjectByMode(), parentTransform).TryGetComponent(out LightSourceToy lightSourceToy))
                {
                    gameObject = lightSourceToy.gameObject.AddComponent<LightSourceObject>().Init(block).gameObject;

                    if (TryGetAnimatorController(block.AnimatorName, out animatorController))
                        _animators.Add(lightSourceToy._light.gameObject, animatorController);
                }

                break;
            }

            case BlockType.Pickup:
            {
                Pickup pickup = null;

                if (block.Properties.TryGetValue("Chance", out var property) && Random.Range(0, 101) > float.Parse(property.ToString()))
                {
                    gameObject = new("Empty Pickup")
                    {
                        transform = { parent = parentTransform, localPosition = block.Position, localEulerAngles = block.Rotation, localScale = block.Scale },
                    };
                    break;
                }

                if (block.Properties.TryGetValue("CustomItem", out property) && !string.IsNullOrEmpty(property.ToString()))
                {
                    // Unsupported
                    break;
                }
                
                var item = Item.Create((ItemType)Enum.Parse(typeof(ItemType), block.Properties["ItemType"].ToString()));

                if (item is Firearm firearm && block.Properties.TryGetValue("Attachements", out property))
                    firearm.AddAttachment(property as List<InventorySystem.Items.Firearms.Attachments.AttachmentName>);

                pickup = item.CreatePickup(Vector3.zero);
                
                gameObject = pickup.Base.gameObject;
                gameObject.name = block.Name;

                gameObject.transform.parent = parentTransform;
                gameObject.transform.localPosition = block.Position;
                gameObject.transform.localEulerAngles = block.Rotation;
                gameObject.transform.localScale = block.Scale;

                if (block.Properties.ContainsKey("Locked"))
                    API.PickupsLocked.Add(pickup.Serial);

                if (block.Properties.TryGetValue("Uses", out property))
                    API.PickupsUsesLeft.Add(pickup.Serial, int.Parse(property.ToString()));

                break;
            }

            case BlockType.Workstation:
            {
                if (Instantiate(ObjectType.WorkStation.GetObjectByMode(), parentTransform).TryGetComponent(out InventorySystem.Items.Firearms.Attachments.WorkstationController workstation))
                {
                    gameObject = workstation.gameObject.AddComponent<WorkstationObject>().Init(block).gameObject;

                    gameObject.transform.parent = null;
                    NetworkServer.Spawn(gameObject);

                    _transformProperties.Add(gameObject.transform.GetInstanceID(), block.ObjectId);
                }

                break;
            }

            case BlockType.Locker:
            {
                if (block.Properties.TryGetValue("Chance", out var property) && Random.Range(0, 101) > float.Parse(property.ToString()))
                {
                    gameObject = new("Empty Locker")
                    {
                        transform = { localPosition = block.Position, localEulerAngles = block.Rotation, localScale = block.Scale },
                    };
                }
                else
                {
                    var lockerType = (LockerType)Enum.Parse(typeof(LockerType), block.Properties["LockerType"].ToString());
                    gameObject = Instantiate(lockerType.GetLockerObjectByType(), parentTransform).AddComponent<LockerObject>().Init(block).gameObject;
                }

                break;
            }

            case BlockType.Schematic:
            {
                var schematicName = block.Properties["SchematicName"].ToString();

                gameObject = ObjectSpawner.SpawnSchematic(schematicName, transform.position + block.Position, Quaternion.Euler(transform.eulerAngles + block.Rotation)).gameObject;
                gameObject.transform.parent = parentTransform;

                gameObject.name = schematicName;

                break;
            }
        }

        AttachedBlocks.Add(gameObject);
        ObjectFromId.Add(block.ObjectId, gameObject.transform);

        if (block.BlockType != BlockType.Light && TryGetAnimatorController(block.AnimatorName, out animatorController))
            _animators.Add(gameObject, animatorController);

        return gameObject.transform;
    }

    private bool TryGetAnimatorController(string animatorName, out RuntimeAnimatorController animatorController)
    {
        animatorController = null;

        if (string.IsNullOrEmpty(animatorName))
            return false;

        var animatorObject = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(x => x.mainAsset.name == animatorName)?.LoadAllAssets().First(x => x is RuntimeAnimatorController);

        if (animatorObject is null)
        {
            var path = Path.Combine(DirectoryPath, animatorName);

            if (!File.Exists(path))
            {
                Log.Warning($"{gameObject.name} block of {name} should have a {animatorName} animator attached, but the file does not exist!");
                return false;
            }

            animatorObject = AssetBundle.LoadFromFile(path).LoadAllAssets().First(x => x is RuntimeAnimatorController);
        }

        animatorController = (RuntimeAnimatorController)animatorObject;
        return true;
    }

    private void AddAnimators()
    {
        foreach (var pair in _animators)
            pair.Key.AddComponent<Animator>().runtimeAnimatorController = pair.Value;

        _animators = null;
        AssetBundle.UnloadAllAssetBundles(false);
    }

    private IEnumerator<float> SpawnDelayed()
    {
        foreach (var networkIdentity in NetworkIdentities)
        {
            NetworkServer.Spawn(networkIdentity.gameObject);
            yield return Timing.WaitForSeconds(Config.SchematicBlockSpawnDelay);
        }

        AddAnimators();
        IsBuilt = true;

        if (Base.CullingType != CullingType.Distance)
            yield break;

        foreach (var networkIdentity in NetworkIdentities)
        {
            foreach (Player player in Player.GetPlayers())
            {
                player.DestroyNetworkIdentity(networkIdentity);
            }
        }
    }

    private void CreateTeleporters()
    {
        var teleportPath = Path.Combine(DirectoryPath, $"{Name}-Teleports.json");
        if (!File.Exists(teleportPath))
            return;

        foreach (var teleport in JsonSerializer.Deserialize<List<SerializableTeleport>>(File.ReadAllText(teleportPath)))
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.name = teleport.Name;
            gameObject.transform.localScale = teleport.Scale;

            if (teleport.RoomType == Exiled.Enums.RoomType.Surface)
            {
                gameObject.transform.parent = ObjectFromId[teleport.ParentId];
                gameObject.transform.localPosition = teleport.Position;
                gameObject.transform.localEulerAngles = teleport.Rotation;
            }
            else
            {
                Room room = API.GetRandomRoom(teleport.RoomType);
                gameObject.transform.position = API.GetRelativePosition(teleport.Position, room);
                gameObject.transform.rotation = API.GetRelativeRotation(teleport.Rotation, room);
                gameObject.transform.parent = ObjectFromId[teleport.ParentId];
            }

            AttachedBlocks.Add(gameObject);
            ObjectFromId.Add(teleport.ObjectId, gameObject.transform);

            var teleportObject = gameObject.AddComponent<TeleportObject>();
            teleportObject.IsSchematicBlock = true;
            teleportObject.Init(teleport);

            if (teleport.RoomType != Exiled.Enums.RoomType.Surface)
                teleportObject.SetPreviousTransform();
        }
    }

    private void AddRigidbodies()
    {
        var rigidbodyPath = Path.Combine(DirectoryPath, $"{Name}-Rigidbodies.json");
        if (!File.Exists(rigidbodyPath))
            return;

        foreach (var dict in JsonSerializer.Deserialize<Dictionary<int, SerializableRigidbody>>(File.ReadAllText(rigidbodyPath)))
        {
            if (!ObjectFromId[dict.Key].gameObject.TryGetComponent(out Rigidbody rigidbody))
                rigidbody = ObjectFromId[dict.Key].gameObject.AddComponent<Rigidbody>();

            rigidbody.isKinematic = dict.Value.IsKinematic;
            rigidbody.useGravity = dict.Value.UseGravity;
            rigidbody.constraints = dict.Value.Constraints;
            rigidbody.mass = dict.Value.Mass;
        }
    }

    private void OnDestroy()
    {
        //Patches.OverridePositionPatch.ResetValues();
        AnimationController.Dictionary.Remove(this);

        // TEMP
        foreach (var gameObject in AttachedBlocks)
        {
            if (_transformProperties.ContainsKey(gameObject.transform.GetInstanceID()))
                NetworkServer.Destroy(gameObject);
        }

        Events.Handlers.Schematic.OnSchematicDestroyed(new Events.EventArgs.SchematicDestroyedEventArgs(this, Name));
    }

    internal bool IsBuilt = false;
    internal Dictionary<int, Transform> ObjectFromId = new();

    private ReadOnlyCollection<NetworkIdentity> _networkIdentities;
    private Dictionary<int, int> _transformProperties = new();
    private Dictionary<GameObject, RuntimeAnimatorController> _animators = new();

    private static readonly Config Config = MapEditorReborn.Singleton.Config;
}