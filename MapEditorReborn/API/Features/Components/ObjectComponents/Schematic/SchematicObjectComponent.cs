namespace MapEditorReborn.API.Features.Components.ObjectComponents
{
    using System;
    using System.Collections.Generic;
    using AdminToys;
    using Enums;
    using Events.EventArgs;
    using Events.Handlers;
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
            ForcedRoomType = schematicObject.RoomType != RoomType.Unknown ? schematicObject.RoomType : FindRoom().Type;

            CreateRecursiveFromID(data.RootObjectId, data.Blocks, transform);
            AssetBundle.UnloadAllAssetBundles(false);

            UpdateObject();

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

            switch (block.BlockType)
            {
                case BlockType.Primitive:
                    {
                        if (Instantiate(ObjectType.Primitive.GetObjectByMode()).TryGetComponent(out PrimitiveObjectToy primitiveObject))
                        {
                            primitiveObject.name = block.Name;

                            primitiveObject.transform.parent = parentGameObject;
                            primitiveObject.transform.localPosition = block.Position;
                            primitiveObject.transform.localEulerAngles = block.Rotation;
                            primitiveObject.transform.localScale = block.Scale;

                            primitiveObject.NetworkPrimitiveType = (PrimitiveType)Enum.Parse(typeof(PrimitiveType), block.Properties["PrimitiveType"].ToString());
                            primitiveObject.NetworkMaterialColor = GetColorFromString(block.Properties["Color"].ToString());
                            primitiveObject.NetworkMovementSmoothing = 60;

                            NetworkServer.Spawn(primitiveObject.gameObject);
                        }

                        Log.Info("Primitive spawned!");

                        AttachedBlocks.Add(primitiveObject.gameObject);

                        return primitiveObject.transform;
                    }

                case BlockType.Empty:
                    {
                        Transform emptyObjectTransform = new GameObject(block.Name).transform;

                        emptyObjectTransform.parent = parentGameObject;
                        emptyObjectTransform.localPosition = block.Position;
                        emptyObjectTransform.localEulerAngles = block.Rotation;
                        emptyObjectTransform.localScale = block.Scale;

                        Log.Info("Empty spawned!");

                        AttachedBlocks.Add(emptyObjectTransform.gameObject);

                        return emptyObjectTransform;
                    }
            }

            return null;
        }

        /// <summary>
        /// The base config of the object which contains its properties.
        /// </summary>
        public SchematicObject Base;

        /// <summary>
        /// Gets a <see cref="SchematicObjectDataList"/> used to build a schematic.
        /// </summary>
        public SchematicObjectDataList SchematicData { get; private set; }

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

            // Timing.RunCoroutine(UpdateBlocks());
        }

        /*
        /// <summary>
        /// Moves the <see cref="AttachedBlocks"/>.
        /// </summary>
        /// <returns><see cref="IEnumerator{T}"/> which represents one frame delay.</returns>
        public IEnumerator<float> MoveBlocks()
        {
            foreach (SchematicBlockComponent block in AttachedBlocks)
            {
                block.UpdateObject();
            }

            yield return Timing.WaitForOneFrame;
        }

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

        private void OnDestroy()
        {
            foreach (SchematicBlockComponent block in AttachedBlocks)
            {
                block?.Destroy();
            }
        }
        */

        private static readonly float UpdateDelay = MapEditorReborn.Singleton.Config.SchematicBlockSpawnDelay;
        /*
foreach (PrimitiveObject primitive in data.Primitives)
{
    if (Instantiate(ObjectType.Primitive.GetObjectByMode(), transform.TransformPoint(primitive.Position), transform.rotation * Quaternion.Euler(primitive.Rotation)).TryGetComponent(out PrimitiveObjectToy primitiveObject))
    {
        primitiveObject.transform.localScale = Vector3.Scale(primitive.Scale, schematicObject.Scale);

        primitiveObject.name = $"CustomSchematicBlock-Primitive{primitive.PrimitiveType}";

        primitiveObject.gameObject.AddComponent<PrimitiveObjectComponent>().Init(primitive, false);
        AttachedBlocks.Add(primitiveObject.gameObject.AddComponent<SchematicBlockComponent>().Init(this, primitive.Position, primitive.Rotation, primitive.Scale));
    }
}

foreach (LightSourceObject lightSource in data.LightSources)
{
    if (Instantiate(ObjectType.LightSource.GetObjectByMode(), transform.TransformPoint(lightSource.Position), Quaternion.identity).TryGetComponent(out LightSourceToy lightSourceToy))
    {
        lightSourceToy.name = "CustomSchematicBlock-LightSource";
        lightSourceToy.gameObject.AddComponent<LightSourceComponent>().Init(lightSource, false);
        AttachedBlocks.Add(lightSourceToy.gameObject.AddComponent<SchematicBlockComponent>().Init(this, lightSource.Position, Vector3.zero, Vector3.one));
    }
}

foreach (ItemSpawnPointObject item in data.Items)
{
    Pickup pickup = new Item((ItemType)Enum.Parse(typeof(ItemType), item.Item)).Create(transform.TransformPoint(item.Position), transform.rotation * Quaternion.Euler(item.Rotation), Vector3.Scale(item.Scale, schematicObject.Scale));

    pickup.Locked = true;

    if (pickup.Base.TryGetComponent(out Rigidbody rb))
        rb.isKinematic = true;

    pickup.Base.name = $"CustomSchematicBlock-Item{pickup.Type}";

    AttachedBlocks.Add(pickup.Base.gameObject.AddComponent<SchematicBlockComponent>().Init(this, item.Position, item.Rotation, item.Scale));
}

foreach (var workStation in data.WorkStations)
{
    GameObject gameObject = Instantiate(ObjectType.WorkStation.GetObjectByMode(), transform.TransformPoint(workStation.Position), transform.rotation * Quaternion.Euler(workStation.Rotation));
    gameObject.transform.localScale = Vector3.Scale(workStation.Scale, schematicObject.Scale);

    if (gameObject.TryGetComponent(out InventorySystem.Items.Firearms.Attachments.WorkstationController workstationController))
        workstationController.NetworkStatus = 4;

    gameObject.name = "CustomSchematicBlock-Workstation";

    AttachedBlocks.Add(gameObject.AddComponent<SchematicBlockComponent>().Init(this, workStation.Position, workStation.Rotation, workStation.Scale));
}
*/

    }
}
