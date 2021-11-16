namespace MapEditorReborn.API
{
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using MEC;
    using Mirror;
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

            foreach (SchematicBlockData block in data.Blocks)
            {
                switch (block.ObjectType)
                {
                    case ObjectType.Item:
                        {
                            Pickup pickup = new Item(block.ItemType).Create(transform.TransformPoint(block.Position), transform.rotation * Quaternion.Euler(block.Rotation), Vector3.Scale(block.Scale, schematicObject.Scale));
                            pickup.Locked = true;
                            pickup.Base.GetComponent<Rigidbody>().isKinematic = true;

                            attachedBlocks.Add(pickup.Base.gameObject.AddComponent<SchematicBlockComponent>().Init(this, block.Position, block.Rotation, block.Scale, false));

                            break;
                        }

                    case ObjectType.Workstation:
                        {
                            GameObject gameObject = Instantiate(Handler.WorkstationObj, transform.TransformPoint(block.Position), transform.rotation * Quaternion.Euler(block.Rotation));
                            gameObject.transform.localScale = Vector3.Scale(block.Scale, schematicObject.Scale);
                            gameObject.GetComponent<InventorySystem.Items.Firearms.Attachments.WorkstationController>().NetworkStatus = 4;

                            attachedBlocks.Add(gameObject.AddComponent<SchematicBlockComponent>().Init(this, block.Position, block.Rotation, block.Scale, true));

                            break;
                        }
                }
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
                var newObject = Handler.SpawnSchematic(Base, null, transform.position, transform.rotation, transform.localScale);

                if (newObject != null)
                {
                    Handler.SpawnedObjects[Handler.SpawnedObjects.FindIndex(x => x == this)] = newObject;

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
