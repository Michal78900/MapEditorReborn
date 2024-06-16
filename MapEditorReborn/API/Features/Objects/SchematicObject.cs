// -----------------------------------------------------------------------
// <copyright file="SchematicObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using AdminToys;
    using Configs;
    using Enums;
    using Events.EventArgs;
    using Events.Handlers;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.CustomItems.API.Features;
    using Extensions;
    using InventorySystem.Items.Firearms.Attachments;
    using MEC;
    using Mirror;
    using Serializable;
    using UnityEngine;
    using Utf8Json;
    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Component added to SchematicObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
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
            Log.Debug($"Initializing schematic \"{schematicSerializable.SchematicName}\"");

            Base = schematicSerializable;
            SchematicData = data;
            DirectoryPath = data.Path;
            ForcedRoomType = schematicSerializable.RoomType != RoomType.Unknown ? schematicSerializable.RoomType : FindRoom().Type;

            ObjectFromId = new Dictionary<int, Transform>(SchematicData.Blocks.Count + 1)
            {
                { data.RootObjectId, transform },
            };

            CreateRecursiveFromID(data.RootObjectId, data.Blocks, transform);
            CreateTeleporters();

            // if (Config.SchematicBlockSpawnDelay >= 0f)
            // {
                // Timing.RunCoroutine(SpawnDelayed());
            //}
            // else
            // {
                foreach (NetworkIdentity networkIdentity in NetworkIdentities)
                    NetworkServer.Spawn(networkIdentity.gameObject);

                // if (!AddAnimators() && isStatic)
                // {
                    // Log.Debug($"Schematic {Name} has no animators, making it static...");
                    // IsStatic = true;
                // }

                IsBuilt = true;
            // }

            bool hasRigidbodies = AddRigidbodies();
            bool isAnimated = AddAnimators();

/*
            if ((hasRigidbodies || isAnimated) && isStatic)
            {
                IsStatic = false;
            }
            else
            {
                IsStatic = isStatic;
                if (IsStatic)
                    Log.Debug($"Schematic {Name} has no animators, making it static...");
            }
            */

            // bool value = !isAnimated && isStatic;
            // IsStatic = value;
            // if (value)
                // Log.Debug($"Schematic {Name} has no animators, making it static...");

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
        public string Name => Base.SchematicName;

        public AnimationController AnimationController => AnimationController.Get(this);

        /// <summary>
        /// Gets a value indicating whether this schematic is at top of transform hierarchy.
        /// </summary>
        public bool IsRootSchematic => transform.root == transform;

        /// <summary>
        /// Gets the read-only collections of <see cref="NetworkIdentity"/> in this schematic.
        /// </summary>
        public ReadOnlyCollection<NetworkIdentity> NetworkIdentities
        {
            get
            {
                if (_networkIdentities != null)
                    return _networkIdentities;

                List<NetworkIdentity> list = new();
                foreach (GameObject block in AttachedBlocks)
                {
                    if (block.TryGetComponent(out NetworkIdentity networkIdentity))
                        list.Add(networkIdentity);
                }

                _networkIdentities = list.AsReadOnly();
                return _networkIdentities;
            }
        }

        public ReadOnlyCollection<AdminToyBase> AdminToyBases
        {
            get
            {
                if (_adminToyBases != null)
                    return _adminToyBases;

                List<AdminToyBase> list = new();
                foreach (NetworkIdentity netId in NetworkIdentities)
                {
                    if (netId.TryGetComponent(out AdminToyBase adminToyBase))
                        list.Add(adminToyBase);
                }

                _adminToyBases = list.AsReadOnly();
                return _adminToyBases;
            }
        }

        public bool IsStatic
        {
            get => _isStatic;
            set
            {
                foreach (AdminToyBase toy in AdminToyBases)
                {
                    if (toy.TryGetComponent(out PrimitiveObject primitiveObject))
                    {
                        primitiveObject.IsStatic = value;
                        continue;
                    }

                    /*
                    if (toy.TryGetComponent(out LightSourceObject lightSourceObject))
                    {
                        lightSourceObject.IsStatic = false;
                    }
                    */
                }

                _isStatic = value;
            }
        }

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            if (IsRootSchematic && Base.SchematicName != name.Split('-')[1])
            {
                SchematicObject newObject = ObjectSpawner.SpawnSchematic(Base, transform.position, transform.rotation, transform.localScale, null);

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

            foreach (GameObject gameObject in AttachedBlocks)
            {
                if (gameObject.TryGetComponent(out WorkstationObject _))
                {
                    NetworkServer.UnSpawn(gameObject);

                    SchematicBlockData block = SchematicData.Blocks.Find(c => c.ObjectId == _transformProperties[gameObject.transform.GetInstanceID()]);
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

            // if (IsStatic)
            // {
            foreach (AdminToyBase adminToyBase in AdminToyBases)
            {
                if (adminToyBase.TryGetComponent(out PrimitiveObject primitiveObject))
                {
                    primitiveObject.UpdateObject();
                }
            }
            // }

            // if (!IsRootSchematic)
                // return;

            // Timing.CallDelayed(0.1f, () => Patches.OverridePositionPatch.ResetValues());
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _networkIdentities = null;
            _adminToyBases = null;
        }

        private void CreateRecursiveFromID(int id, List<SchematicBlockData> blocks, Transform parentGameObject)
        {
            Transform childGameObjectTransform = CreateObject(blocks.Find(c => c.ObjectId == id), parentGameObject) ?? transform; // Create the object first before creating children.
            int[] parentSchematics = blocks.Where(bl => bl.BlockType == BlockType.Schematic).Select(bl => bl.ObjectId).ToArray();

            // Gets all the ObjectIds of all the schematic blocks inside "blocks" argument.
            foreach (SchematicBlockData block in blocks.FindAll(c => c.ParentId == id))
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
                            localScale = Vector3.one,
                        },
                    };

                    break;
                }

                case BlockType.Primitive:
                {
                    if (Instantiate(ObjectType.Primitive.GetObjectByMode(), parentTransform).TryGetComponent(out PrimitiveObjectToy primitiveToy))
                    {
                        gameObject = primitiveToy.gameObject.AddComponent<PrimitiveObject>().Init(block).gameObject;
                    }

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

                    if (block.Properties.TryGetValue("Chance", out object property) && Random.Range(0, 101) > float.Parse(property.ToString()))
                    {
                        gameObject = new("Empty Pickup")
                        {
                            transform = { parent = parentTransform, localPosition = block.Position, localEulerAngles = block.Rotation, localScale = block.Scale },
                        };
                        break;
                    }

                    if (block.Properties.TryGetValue("CustomItem", out property) && !string.IsNullOrEmpty(property.ToString()))
                    {
                        string customItemName = property.ToString();

                        if (!CustomItem.TryGet(customItemName, out CustomItem customItem))
                        {
                            Log.Error($"CustomItem with the name {customItemName} does not exist!");
                            gameObject = new("Invalid Pickup")
                            {
                                transform = { parent = parentTransform, localPosition = block.Position, localEulerAngles = block.Rotation, localScale = block.Scale },
                            };

                            AttachedBlocks.Add(gameObject);
                            ObjectFromId.Add(block.ObjectId, gameObject.transform);
                        }
                        else
                        {
                            pickup = customItem.Spawn(Vector3.zero);
                        }
                    }
                    else
                    {
                        Item item = Item.Create((ItemType)Enum.Parse(typeof(ItemType), block.Properties["ItemType"].ToString()));

                        if (item is Firearm firearm && block.Properties.TryGetValue("Attachements", out property))
                            firearm.AddAttachment(property as List<AttachmentName>);

                        pickup = item.CreatePickup(Vector3.zero);
                    }

                    gameObject = pickup.Base.gameObject;
                    gameObject.name = block.Name;

                    NetworkServer.UnSpawn(gameObject);

                    gameObject.transform.parent = parentTransform;
                    gameObject.transform.localPosition = block.Position;
                    gameObject.transform.localEulerAngles = block.Rotation;
                    gameObject.transform.localScale = block.Scale;

                    NetworkServer.Spawn(gameObject);

                    if (block.Properties.ContainsKey("Locked"))
                        API.PickupsLocked.Add(pickup.Serial);

                    if (block.Properties.TryGetValue("Uses", out property))
                        API.PickupsUsesLeft.Add(pickup.Serial, int.Parse(property.ToString()));

                    break;
                }

                case BlockType.Workstation:
                {
                    if (Instantiate(ObjectType.WorkStation.GetObjectByMode(), parentTransform).TryGetComponent(out WorkstationController workstation))
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
                    if (block.Properties.TryGetValue("Chance", out object property) && Random.Range(0, 101) > float.Parse(property.ToString()))
                    {
                        gameObject = new("Empty Locker")
                        {
                            transform = { localPosition = block.Position, localEulerAngles = block.Rotation, localScale = block.Scale },
                        };
                    }
                    else
                    {
                        LockerType lockerType = (LockerType)Enum.Parse(typeof(LockerType), block.Properties["LockerType"].ToString());
                        gameObject = Instantiate(lockerType.GetLockerObjectByType(), parentTransform).AddComponent<LockerObject>().Init(block).gameObject;
                    }

                    break;
                }

                case BlockType.Schematic:
                {
                    string schematicName = block.Properties["SchematicName"].ToString();

                    gameObject = ObjectSpawner.SpawnSchematic(schematicName, transform.position + block.Position, Quaternion.Euler(transform.eulerAngles + block.Rotation), null, null, true).gameObject;
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

            Object animatorObject = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(x => x.mainAsset.name == animatorName)?.LoadAllAssets().First(x => x is RuntimeAnimatorController);

            if (animatorObject is null)
            {
                string path = Path.Combine(DirectoryPath, animatorName);

                if (!File.Exists(path))
                {
                    Log.Warn($"{gameObject.name} block of {Name} should have a {animatorName} animator attached, but the file does not exist!");
                    return false;
                }

                animatorObject = AssetBundle.LoadFromFile(path).LoadAllAssets().First(x => x is RuntimeAnimatorController);
            }

            animatorController = (RuntimeAnimatorController)animatorObject;
            return true;
        }

        private bool AddAnimators()
        {
            bool isAnimated = false;
            if (!_animators.IsEmpty())
            {
                isAnimated = true;
                foreach (KeyValuePair<GameObject, RuntimeAnimatorController> pair in _animators)
                    pair.Key.AddComponent<Animator>().runtimeAnimatorController = pair.Value;
            }

            _animators = null;
            AssetBundle.UnloadAllAssetBundles(false);
            return isAnimated;
        }

        /*
        private IEnumerator<float> SpawnDelayed()
        {
            foreach (NetworkIdentity networkIdentity in NetworkIdentities)
            {
                NetworkServer.Spawn(networkIdentity.gameObject);
                yield return Timing.WaitForSeconds(Config.SchematicBlockSpawnDelay);
            }

            IsBuilt = true;
        }
        */

        private void CreateTeleporters()
        {
            string teleportPath = Path.Combine(DirectoryPath, $"{Name}-Teleports.json");
            if (!File.Exists(teleportPath))
                return;

            foreach (SerializableTeleport teleport in JsonSerializer.Deserialize<List<SerializableTeleport>>(File.ReadAllText(teleportPath)))
            {
                GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                gameObject.name = teleport.Name;
                gameObject.transform.localScale = teleport.Scale;

                if (teleport.RoomType == RoomType.Surface)
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

                TeleportObject teleportObject = gameObject.AddComponent<TeleportObject>();
                teleportObject.IsSchematicBlock = true;
                teleportObject.Init(teleport);

                if (teleport.RoomType != RoomType.Surface)
                    teleportObject.SetPreviousTransform();
            }
        }

        private bool AddRigidbodies()
        {
            bool hasRigidbodies = false;
            string rigidbodyPath = Path.Combine(DirectoryPath, $"{Name}-Rigidbodies.json");
            if (!File.Exists(rigidbodyPath))
                return false;

            foreach (KeyValuePair<int, SerializableRigidbody> dict in JsonSerializer.Deserialize<Dictionary<int, SerializableRigidbody>>(File.ReadAllText(rigidbodyPath)))
            {
                if (!ObjectFromId[dict.Key].gameObject.TryGetComponent(out Rigidbody rigidbody))
                    rigidbody = ObjectFromId[dict.Key].gameObject.AddComponent<Rigidbody>();

                rigidbody.isKinematic = dict.Value.IsKinematic;
                rigidbody.useGravity = dict.Value.UseGravity;
                rigidbody.constraints = dict.Value.Constraints;
                rigidbody.mass = dict.Value.Mass;

                hasRigidbodies = true;
            }

            return hasRigidbodies;
        }

        private void OnDestroy()
        {
            // Patches.OverridePositionPatch.ResetValues();
            AnimationController.Dictionary.Remove(this);

            // TEMP
            foreach (GameObject gameObject in AttachedBlocks)
            {
                if (_transformProperties.ContainsKey(gameObject.transform.GetInstanceID()))
                    NetworkServer.Destroy(gameObject);
            }

            Schematic.OnSchematicDestroyed(new SchematicDestroyedEventArgs(this, Name));
        }

        internal bool IsBuilt;
        internal Dictionary<int, Transform> ObjectFromId = new();

        private bool _isStatic;
        private ReadOnlyCollection<NetworkIdentity>? _networkIdentities;
        private ReadOnlyCollection<AdminToyBase>? _adminToyBases;
        private Dictionary<int, int> _transformProperties = new();
        private Dictionary<GameObject, RuntimeAnimatorController> _animators = new();

        private static readonly Config Config = MapEditorReborn.Singleton.Config;
    }
}