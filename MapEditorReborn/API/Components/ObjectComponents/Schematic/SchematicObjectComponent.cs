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

                primitiveObject.name = $"CustomSchematicBlock-Primitive{primitive.PrimitiveType}";

                primitiveObject.gameObject.AddComponent<PrimitiveObjectComponent>().Init(primitive, false);
                attachedBlocks.Add(primitiveObject.gameObject.AddComponent<SchematicBlockComponent>().Init(this, primitive.Position, primitive.Rotation, primitive.Scale));
            }

            foreach (var item in data.Items)
            {
                Pickup pickup = new Item((ItemType)System.Enum.Parse(typeof(ItemType), item.Item)).Create(transform.TransformPoint(item.Position), transform.rotation * Quaternion.Euler(item.Rotation), Vector3.Scale(item.Scale, schematicObject.Scale));
                pickup.Locked = true;
                pickup.Base.GetComponent<Rigidbody>().isKinematic = true;

                pickup.Base.name = $"CustomSchematicBlock-Item{pickup.Type}";

                attachedBlocks.Add(pickup.Base.gameObject.AddComponent<SchematicBlockComponent>().Init(this, item.Position, item.Rotation, item.Scale));
            }

            foreach (var workstaion in data.WorkStations)
            {
                GameObject gameObject = Instantiate(ToolGunMode.WorkStation.GetObjectByMode(), transform.TransformPoint(workstaion.Position), transform.rotation * Quaternion.Euler(workstaion.Rotation));
                gameObject.transform.localScale = Vector3.Scale(workstaion.Scale, schematicObject.Scale);
                gameObject.GetComponent<InventorySystem.Items.Firearms.Attachments.WorkstationController>().NetworkStatus = 4;

                gameObject.name = "CustomSchematicBlock-Workstation";

                attachedBlocks.Add(gameObject.AddComponent<SchematicBlockComponent>().Init(this, workstaion.Position, workstaion.Rotation, workstaion.Scale));
            }

            UpdateObject();
            Timing.RunCoroutine(UpdateAnimation(data.ParentAnimationFrames));

            return this;
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public SchematicObject Base;
        public Vector3 OriginalPosition;
        public Vector3 OriginalRotation;

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

            OriginalPosition = RelativePosition;
            OriginalRotation = RelativeRotation;

            Timing.RunCoroutine(UpdateBlocks());
        }

        private IEnumerator<float> UpdateAnimation(List<AnimationFrame> frames)
        {
            foreach (AnimationFrame frame in frames)
            {
                Vector3 remainingPosition = frame.PositionAdded;
                Vector3 remainingRotation = frame.RotationAdded;
                Vector3 deltaPosition = remainingPosition / Mathf.Abs(frame.PositionRate);
                Vector3 deltaRotation = remainingRotation / Mathf.Abs(frame.RotationRate);

                yield return Timing.WaitForSeconds(frame.Delay);

                while (true)
                {
                    if (remainingPosition != Vector3.zero)
                    {
                        transform.position += deltaPosition;
                        remainingPosition -= deltaPosition;
                    }

                    if (remainingRotation != Vector3.zero)
                    {
                        transform.Rotate(deltaRotation, Space.World);
                        remainingRotation -= deltaRotation;
                    }

                    Timing.RunCoroutine(UpdateBlocks());

                    if (remainingPosition.sqrMagnitude <= 1 && remainingRotation.sqrMagnitude <= 1)
                        break;

                    yield return Timing.WaitForSeconds(frame.FrameLength);
                }
            }
        }

        private IEnumerator<float> UpdateBlocks()
        {
            float delay = MapEditorReborn.Singleton.Config.SchematicBlockSpawnDelay;

            foreach (SchematicBlockComponent block in attachedBlocks)
            {
                block.UpdateObject();

                if (delay >= 0f)
                    yield return delay == 0f ? Timing.WaitForOneFrame : Timing.WaitForSeconds(delay);
            }

            yield return Timing.WaitForOneFrame;
        }

        private void OnDestroy()
        {
            foreach (SchematicBlockComponent block in attachedBlocks)
            {
                block?.Destroy();
            }
        }

        private List<SchematicBlockComponent> attachedBlocks = new List<SchematicBlockComponent>();
    }
}
