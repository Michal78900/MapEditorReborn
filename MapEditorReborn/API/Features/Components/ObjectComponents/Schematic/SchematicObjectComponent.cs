namespace MapEditorReborn.API.Features.Components.ObjectComponents
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using AdminToys;
    using Enums;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Extensions;
    using Mirror;
    using Objects;
    using Objects.Schematics;
    using UnityEngine;

    using static API;

    using MapEditorObject = MapEditorObject;

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
            AssetBundle.UnloadAllAssetBundles(false);

            // UpdateObject();

            return this;
        }

        public void CreateRecursiveFromID(int id, List<SchematicBlockData> blocks, Transform parentGameObject)
        {
            Transform childGameObjectTransform = CreateObject(SchematicData.Blocks.Find(c => c.ObjectId == id), parentGameObject) ?? transform; // Create the object first before creating children.

            foreach (SchematicBlockData block in SchematicData.Blocks.FindAll(c => c.ParentId == id))
            {
                CreateRecursiveFromID(block.ObjectId, blocks, childGameObjectTransform); // The child now becomes the parent
            }
        }

        public Transform CreateObject(SchematicBlockData block, Transform parentGameObject)
        {
            if (block == null)
                return null;

            GameObject gameObject = null;

            switch (block.BlockType)
            {
                case BlockType.Primitive:
                    {
                        if (Instantiate(ObjectType.Primitive.GetObjectByMode()).TryGetComponent(out PrimitiveObjectToy primitiveObject))
                        {
                            gameObject = primitiveObject.gameObject;

                            gameObject.name = block.Name;

                            gameObject.transform.parent = parentGameObject;
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
                        blockOriginalScales.Add(gameObject, block.Scale);

                        break;
                    }

                case BlockType.Empty:
                    {
                        gameObject = new GameObject(block.Name)
                        {
                            layer = 2, // Ignore Raycast
                        };

                        gameObject.transform.parent = parentGameObject;
                        gameObject.transform.localPosition = block.Position;
                        gameObject.transform.localEulerAngles = block.Rotation;
                        // gameObject.transform.localScale = block.Scale;

                        AttachedBlocks.Add(gameObject);
                        // blockOriginalScales.Add(gameObject, Vector3.one);

                        break;
                    }
            }

            if (!string.IsNullOrEmpty(block.AnimatorName))
            {
                string path = Path.Combine(DirectoryPath, block.AnimatorName);

                if (!File.Exists(path))
                {
                    Log.Warn($"{gameObject.name} block of {name} should have a {block.AnimatorName} animator attached, but the file does not exist!");
                    return gameObject.transform;
                }

                gameObject.AddComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)AssetBundle.LoadFromFile(path).LoadAllAssets()[0];
            }

            return gameObject.transform;
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
        /// Gets a <see cref="List{T}"/> of <see cref="SchematicBlockComponent"/> which contains all attached blocks.
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

            foreach (GameObject gameObject in blockOriginalScales.Keys)
            {
                gameObject.transform.localScale = Vector3.Scale(blockOriginalScales[gameObject], transform.root.localScale);

                // gameObject.transform.localScale = gameObject.transform.lossyScale;
            }

            // Timing.RunCoroutine(UpdateBlocks());
        }

        /*
        private IEnumerator<float> UpdateBlocks()
        {
            foreach (SchematicBlockComponent block in AttachedBlocks)
            {
                block.UpdateObject();

                if (UpdateDelay >= 0f)
                    yield return UpdateDelay == 0f ? Timing.WaitForOneFrame : Timing.WaitForSeconds(UpdateDelay);
            }

            yield return Timing.WaitForOneFrame;
        }
        */

        /*
        private void OnDestroy()
        {
            foreach (SchematicBlockComponent block in AttachedBlocks)
            {
                block?.Destroy();
            }
        }
        */

        private Dictionary<GameObject, Vector3> blockOriginalScales = new Dictionary<GameObject, Vector3>();

        private static readonly float UpdateDelay = MapEditorReborn.Singleton.Config.SchematicBlockSpawnDelay;
    }
}
