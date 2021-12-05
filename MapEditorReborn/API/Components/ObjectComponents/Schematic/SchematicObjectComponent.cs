namespace MapEditorReborn.API
{
    using System.Collections.Generic;
    using AdminToys;
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using MEC;
    using UnityEngine;

    /// <summary>
    /// Component added to SchematicObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class SchematicObjectComponent : MapEditorObject
    {
        /// <summary>
        /// Initializes the <see cref="SchematicObjectComponent"/>.
        /// </summary>
        /// <param name="schematicObject">The <see cref="SchematicObject"/> to instantiate.</param>
        /// <returns>Instance of this compoment.</returns>
        public SchematicObjectComponent Init(SchematicObject schematicObject, SaveDataObjectList data)
        {
            Base = schematicObject;
            ForcedRoomType = schematicObject.RoomType != RoomType.Unknown ? schematicObject.RoomType : FindRoom().Type;

            foreach (var primitive in data.Primitives)
            {
                PrimitiveObjectToy primitiveObject = Instantiate(ToolGunMode.Primitive.GetObjectByMode(), transform.TransformPoint(primitive.Position), transform.rotation * Quaternion.Euler(primitive.Rotation)).GetComponent<PrimitiveObjectToy>();
                primitiveObject.transform.localScale = Vector3.Scale(primitive.Scale, schematicObject.Scale);
                primitiveObject.NetworkMovementSmoothing = 60;
                primitiveObject.NetworkPrimitiveType = primitive.Type;
                primitiveObject.NetworkMaterialColor = GetColorFromString($"#{primitive.Color}");

                primitiveObject.name = $"CustomSchematicBlock-Primitive{primitive.Type}";

                attachedBlocks.Add(primitiveObject.gameObject.AddComponent<SchematicBlockComponent>().Init(this, primitive.Position, primitive.Rotation, primitive.Scale, true));
            }

            foreach (var item in data.Items)
            {
                Pickup pickup = new Item(item.ItemType).Create(transform.TransformPoint(item.Position), transform.rotation * Quaternion.Euler(item.Rotation), Vector3.Scale(item.Scale, schematicObject.Scale));
                pickup.Locked = true;
                pickup.Base.GetComponent<Rigidbody>().isKinematic = true;

                pickup.Base.name = $"CustomSchematicBlock-{pickup.Type}";

                attachedBlocks.Add(pickup.Base.gameObject.AddComponent<SchematicBlockComponent>().Init(this, item.Position, item.Rotation, item.Scale, false));
            }

            foreach (var workstaion in data.WorkStations)
            {
                GameObject gameObject = Instantiate(ToolGunMode.WorkStation.GetObjectByMode(), transform.TransformPoint(workstaion.Position), transform.rotation * Quaternion.Euler(workstaion.Rotation));
                gameObject.transform.localScale = Vector3.Scale(workstaion.Scale, schematicObject.Scale);
                gameObject.GetComponent<InventorySystem.Items.Firearms.Attachments.WorkstationController>().NetworkStatus = 4;

                gameObject.name = "CustomSchematicBlock-Workstation";

                attachedBlocks.Add(gameObject.AddComponent<SchematicBlockComponent>().Init(this, workstaion.Position, workstaion.Rotation, workstaion.Scale, true));

            }

            UpdateObject();

            return this;
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public SchematicObject Base;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            if (Base.SchematicName != name.Split(new[] { '-' })[1])
            {
                var newObject = Methods.SpawnSchematic(Base, null, transform.position, transform.rotation, transform.localScale);

                if (newObject != null)
                {
                    Methods.SpawnedObjects[Methods.SpawnedObjects.FindIndex(x => x == this)] = newObject;

                    Destroy();
                    return;
                }

                Base.SchematicName = name.Replace("CustomSchematic-", string.Empty);
            }

            Timing.RunCoroutine(UpdateBlocks());
        }

        private IEnumerator<float> UpdateBlocks()
        {
            float delay = MapEditorReborn.Singleton.Config.SchematicBlockSpawnDelay;
            yield return Timing.WaitForOneFrame;

            foreach (SchematicBlockComponent block in attachedBlocks)
            {
                block.UpdateObject();

                if (delay >= 0f)
                    yield return delay == 0f ? Timing.WaitForOneFrame : Timing.WaitForSeconds(delay);
            }
        }

        private void OnDestroy()
        {
            foreach (SchematicBlockComponent block in attachedBlocks)
            {
                if (block != null)
                    block?.Destroy();
            }

            attachedBlocks.Clear();
        }

        private List<SchematicBlockComponent> attachedBlocks = new List<SchematicBlockComponent>();
    }
}
