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

        private void CreateRecursiveFromID(int id, List<SchematicBlockData> blocks, Transform parentGameObject)
        {
            Transform childGameObjectTransform = CreateObject(SchematicData.Blocks.Find(c => c.ObjectId == id), parentGameObject) ?? transform; // Create the object first before creating children.

            foreach (SchematicBlockData block in SchematicData.Blocks.FindAll(c => c.ParentId == id))
            {
                CreateRecursiveFromID(block.ObjectId, blocks, childGameObjectTransform); // The child now becomes the parent
            }
        }

        private Transform CreateObject(SchematicBlockData block, Transform parentGameObject)
        {
            if (block == null)
                return null;

            GameObject gameObject = null;

            switch (block.BlockType)
            {
                case BlockType.Empty:
                    {
                        gameObject = new GameObject(block.Name)
                        {
                            layer = 2, // Ignore Raycast
                        };

                        gameObject.transform.parent = parentGameObject;
                        gameObject.transform.localPosition = block.Position;
                        gameObject.transform.localEulerAngles = block.Rotation;

                        AttachedBlocks.Add(gameObject);

                        break;
                    }

                case BlockType.Primitive:
                    {
                        if (Instantiate(ObjectType.Primitive.GetObjectByMode(), parentGameObject).TryGetComponent(out PrimitiveObjectToy primitiveObject))
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
                        if (Instantiate(ObjectType.LightSource.GetObjectByMode(), parentGameObject).TryGetComponent(out LightSourceToy lightSourceToy))
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

                            if (!string.IsNullOrEmpty(block.AnimatorName))
                            {
                                Object animatorObject = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(x => x.mainAsset.name == block.AnimatorName)?.LoadAllAssets().First(x => x is RuntimeAnimatorController);

                                if (animatorObject == null)
                                {
                                    string path = Path.Combine(DirectoryPath, block.AnimatorName);

                                    if (!File.Exists(path))
                                    {
                                        Log.Warn($"{gameObject.name} block of {name} should have a {block.AnimatorName} animator attached, but the file does not exist!");
                                        return gameObject.transform;
                                    }

                                    animatorObject = AssetBundle.LoadFromFile(path).LoadAllAssets().First(x => x is RuntimeAnimatorController);
                                }

                                lightSourceToy._light.gameObject.AddComponent<Animator>().runtimeAnimatorController = animatorObject as RuntimeAnimatorController;
                            }
                        }

                        return gameObject.transform;
                    }

                case BlockType.Pickup:
                    {
                        Pickup pickup = Item.Create((ItemType)Enum.Parse(typeof(ItemType), block.Properties["ItemType"].ToString())).CreatePickup(Exiled.API.Extensions.RoleExtensions.GetRandomSpawnProperties(RoleType.NtfCaptain).Item1);
                        gameObject = pickup.Base.gameObject;

                        gameObject.transform.parent = parentGameObject;
                        gameObject.transform.localPosition = block.Position;
                        gameObject.transform.localEulerAngles = block.Rotation;
                        gameObject.transform.localScale = block.Scale;

                        if (block.Properties.ContainsKey("Kinematic"))
                            pickup.Base.Rb.isKinematic = true;

                        if (block.Properties.ContainsKey("Locked"))
                        {
                            Log.Info("Added unpickable thing");
                            ItemSpawnPointComponent.LockedPickups.Add(pickup);
                        }

                        NetworkServer.Spawn(gameObject);

                        return gameObject.transform;
                    }
            }

            if (!string.IsNullOrEmpty(block.AnimatorName))
            {
                Object animatorObject = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(x => x.mainAsset.name == block.AnimatorName)?.LoadAllAssets().First(x => x is RuntimeAnimatorController);

                if (animatorObject == null)
                {
                    string path = Path.Combine(DirectoryPath, block.AnimatorName);

                    if (!File.Exists(path))
                    {
                        Log.Warn($"{gameObject.name} block of {name} should have a {block.AnimatorName} animator attached, but the file does not exist!");
                        return gameObject.transform;
                    }

                    animatorObject = AssetBundle.LoadFromFile(path).LoadAllAssets().First(x => x is RuntimeAnimatorController);
                }

                Timing.RunCoroutine(AddAnimatorDelayed(gameObject, animatorObject as RuntimeAnimatorController));
                // gameObject.AddComponent<Animator>().runtimeAnimatorController = animatorObject as RuntimeAnimatorController;
            }

            return gameObject.transform;
        }

        private IEnumerator<float> AddAnimatorDelayed(GameObject gameObject, RuntimeAnimatorController animatorController)
        {
            yield return Timing.WaitUntilTrue(() => built);

            gameObject.AddComponent<Animator>().runtimeAnimatorController = animatorController;
        }

        private bool built = false;

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
                if (gameObject.TryGetComponent(out InventorySystem.Items.Pickups.ItemPickupBase pickup))
                    pickup.RefreshPositionAndRotation();
            }
        }
    }
}
