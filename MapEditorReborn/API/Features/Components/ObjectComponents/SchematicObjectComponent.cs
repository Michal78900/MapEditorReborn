namespace MapEditorReborn.API.Features.Components.ObjectComponents
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AdminToys;
    using Enums;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Extensions;
    using MEC;
    using Mirror;
    using Objects;
    using Objects.Schematics;
    using UnityEngine;

    using static API;

    using Object = UnityEngine.Object;

#pragma warning disable CS0618

    /// <summary>
    /// Component added to SchematicObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class SchematicObjectComponent : MapEditorObject
    {
        /// <summary>
        /// Initializes the <see cref="SchematicObjectComponent"/>.
        /// </summary>
        /// <param name="schematicObject">The <see cref="SchematicObject"/> to instantiate.</param>
        /// <param name="data">The object data from a file.</param>
        /// <returns>Instance of this compoment.</returns>
        public SchematicObjectComponent Init(SchematicObject schematicObject, SchematicObjectDataList data)
        {
            Base = schematicObject;
            SchematicData = data;
            DirectoryPath = data.Path;
            ForcedRoomType = schematicObject.RoomType != RoomType.Unknown ? schematicObject.RoomType : FindRoom().Type;

            CreateRecursiveFromID(data.RootObjectId, data.Blocks, transform);
            built = true;
            Timing.CallDelayed(1f, () => AssetBundle.UnloadAllAssetBundles(false));

            UpdateObject();

            return this;
        }

        /// <summary>
        /// Gets the schematic name.
        /// </summary>
        public string Name => Base.SchematicName;

        private void CreateRecursiveFromID(int id, List<SchematicBlockData> blocks, Transform parentGameObject)
        {
            Transform childGameObjectTransform = CreateObject(SchematicData.Blocks.Find(c => c.ObjectId == id), parentGameObject) ?? transform; // Create the object first before creating children.

            foreach (SchematicBlockData block in SchematicData.Blocks.FindAll(c => c.ParentId == id))
            {
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
                            layer = 2, // Ignore Raycast
                        };

                        gameObject.transform.parent = parentTransform;
                        gameObject.transform.localPosition = block.Position;
                        gameObject.transform.localEulerAngles = block.Rotation;

                        AttachedBlocks.Add(gameObject);

                        break;
                    }

                case BlockType.Primitive:
                    {
                        if (Instantiate(ObjectType.Primitive.GetObjectByMode(), parentTransform).TryGetComponent(out PrimitiveObjectToy primitiveObject))
                        {
                            gameObject = primitiveObject.gameObject;

                            gameObject.name = block.Name;

                            gameObject.transform.localPosition = block.Position;
                            gameObject.transform.localEulerAngles = block.Rotation;
                            gameObject.transform.localScale = block.Scale;

                            primitiveObject.NetworkPrimitiveType = (PrimitiveType)Enum.Parse(typeof(PrimitiveType), block.Properties["PrimitiveType"].ToString());
                            primitiveObject.NetworkMaterialColor = GetColorFromString(block.Properties["Color"].ToString());
                            primitiveObject.NetworkMovementSmoothing = 60;

                            NetworkServer.Spawn(gameObject);
                            primitiveObject.UpdatePositionServer();
                        }

                        AttachedBlocks.Add(primitiveObject.gameObject);

                        break;
                    }

                case BlockType.Light:
                    {
                        if (Instantiate(ObjectType.LightSource.GetObjectByMode(), parentTransform).TryGetComponent(out LightSourceToy lightSourceToy))
                        {
                            gameObject = lightSourceToy.gameObject;
                            gameObject.name = block.Name;

                            gameObject.transform.localPosition = block.Position;

                            lightSourceToy._light.color = GetColorFromString(block.Properties["Color"].ToString());
                            lightSourceToy._light.intensity = float.Parse(block.Properties["Intensity"].ToString());
                            lightSourceToy._light.range = float.Parse(block.Properties["Range"].ToString());
                            lightSourceToy._light.shadows = bool.Parse(block.Properties["Shadows"].ToString()) ? LightShadows.Soft : LightShadows.None;

                            lightSourceToy.NetworkMovementSmoothing = 60;

                            NetworkServer.Spawn(gameObject);
                            lightSourceToy.UpdatePositionServer();
                        }

                        if (TryGetAnimatorController(block.AnimatorName, out animatorController))
                            Timing.RunCoroutine(AddAnimatorDelayed(lightSourceToy._light.gameObject, animatorController));

                        AttachedBlocks.Add(gameObject);

                        return gameObject.transform;
                    }

                case BlockType.Pickup:
                    {
                        Pickup pickup = Item.Create((ItemType)Enum.Parse(typeof(ItemType), block.Properties["ItemType"].ToString())).CreatePickup(Vector3.zero);
                        gameObject = pickup.Base.gameObject;
                        gameObject.name = block.Name;

                        gameObject.transform.parent = parentTransform;
                        gameObject.transform.localPosition = block.Position;
                        gameObject.transform.localEulerAngles = block.Rotation;
                        gameObject.transform.localScale = block.Scale;

                        if (block.Properties.ContainsKey("Kinematic"))
                            pickup.Base.Rb.isKinematic = true;

                        if (block.Properties.ContainsKey("Locked"))
                            ItemSpawnPointComponent.LockedPickups.Add(pickup);

                        NetworkServer.Spawn(gameObject);

                        AttachedBlocks.Add(gameObject);

                        return gameObject.transform;
                    }

                case BlockType.Workstation:
                    {
                        gameObject = Instantiate(ObjectType.WorkStation.GetObjectByMode());
                        gameObject.name = block.Name;

                        gameObject.transform.parent = parentTransform;
                        gameObject.transform.localPosition = block.Position;
                        gameObject.transform.localEulerAngles = block.Rotation;
                        gameObject.transform.localScale = block.Scale;

                        gameObject.transform.parent = null;
                        NetworkServer.Spawn(gameObject);
                        gameObject.transform.parent = parentTransform;

                        AttachedBlocks.Add(gameObject);

                        return gameObject.transform;
                    }
            }

            if (TryGetAnimatorController(block.AnimatorName, out animatorController))
                Timing.RunCoroutine(AddAnimatorDelayed(gameObject, animatorController));

            return gameObject.transform;
        }

        private IEnumerator<float> AddAnimatorDelayed(GameObject gameObject, RuntimeAnimatorController animatorController)
        {
            yield return Timing.WaitUntilTrue(() => built);
            gameObject.AddComponent<Animator>().runtimeAnimatorController = animatorController;
        }

        private bool TryGetAnimatorController(string animatorName, out RuntimeAnimatorController animatorController)
        {
            animatorController = null;

            if (!string.IsNullOrEmpty(animatorName))
            {
                Object animatorObject = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(x => x.mainAsset.name == animatorName)?.LoadAllAssets().First(x => x is RuntimeAnimatorController);

                if (animatorObject == null)
                {
                    string path = Path.Combine(DirectoryPath, animatorName);

                    if (!File.Exists(path))
                    {
                        Log.Warn($"{gameObject.name} block of {name} should have a {animatorName} animator attached, but the file does not exist!");
                        return false;
                    }

                    animatorObject = AssetBundle.LoadFromFile(path).LoadAllAssets().First(x => x is RuntimeAnimatorController);
                }

                animatorController = animatorObject as RuntimeAnimatorController;
                return true;
            }

            return false;
        }

        /// <summary>
        /// The base config of the object which contains its properties.
        /// </summary>
        public SchematicObject Base;

        /// <summary>
        /// Gets a <see cref="SchematicObjectDataList"/> used to build a schematic.
        /// </summary>
        public SchematicObjectDataList SchematicData { get; private set; }

        public string DirectoryPath { get; private set; }

        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="GameObject"/> which contains all attached blocks.
        /// </summary>
        public List<GameObject> AttachedBlocks { get; private set; } = new List<GameObject>();

        /// <summary>
        /// Gets the original position.
        /// </summary>
        public Vector3 OriginalPosition { get; private set; }

        /// <summary>
        /// Gets the original rotation.
        /// </summary>
        public Vector3 OriginalRotation { get; private set; }

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            if (Base.SchematicName != name.Split(new[] { '-' })[1])
            {
                var newObject = ObjectSpawner.SpawnSchematic(Base, transform.position, transform.rotation, transform.localScale);

                if (newObject != null)
                {
                    SpawnedObjects[SpawnedObjects.FindIndex(x => x == this)] = newObject;

                    Destroy();
                    return;
                }

                Base.SchematicName = name.Replace("CustomSchematic-", string.Empty);
            }

            OriginalPosition = RelativePosition;
            OriginalRotation = RelativeRotation;

            foreach (GameObject gameObject in AttachedBlocks)
            {
                if (gameObject.TryGetComponent(out InventorySystem.Items.Firearms.Attachments.WorkstationController _))
                {
                    Transform prevParent = gameObject.transform.parent;
                    gameObject.transform.parent = null;
                    NetworkServer.UnSpawn(gameObject);
                    NetworkServer.Spawn(gameObject);
                    gameObject.transform.parent = prevParent;
                }
            }
        }

        private bool built = false;
    }
}
