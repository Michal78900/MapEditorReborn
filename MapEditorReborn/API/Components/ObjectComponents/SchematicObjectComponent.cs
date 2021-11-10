namespace MapEditorReborn.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
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
        public SchematicObjectComponent Init(SchematicObject schematicObject, Dictionary<GameObject, Tuple<Vector3, Vector3, Vector3>> gameObjects)
        {
            Base = schematicObject;
            ForcedRoomType = schematicObject.RoomType != RoomType.Unknown ? schematicObject.RoomType : FindRoom().Type;

            attachedObjects = gameObjects;

            foreach (GameObject gameObject in attachedObjects.Keys)
            {
                if (gameObject.name != "Work Station(Clone)")
                    prevScale.Add(gameObject, gameObject.transform.localScale);
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

            foreach (var yes in attachedObjects)
            {
                if (yes.Key.name == "Work Station(Clone)")
                {
                    yes.Key.transform.position = transform.TransformPoint(yes.Value.Item1);
                    yes.Key.transform.rotation = transform.rotation * Quaternion.Euler(yes.Value.Item2);
                }

                yes.Key.transform.localScale = Vector3.Scale(yes.Value.Item3, transform.localScale);

                if (yes.Key.name == "Work Station(Clone)" || yes.Key.transform.localScale != prevScale[yes.Key])
                {
                    if (prevScale.ContainsKey(yes.Key))
                        prevScale[yes.Key] = yes.Key.transform.localScale;

                    NetworkServer.UnSpawn(yes.Key);
                    NetworkServer.Spawn(yes.Key);
                }
            }
        }

        private void OnDestroy()
        {
            foreach (GameObject gameObject in attachedObjects.Keys.ToList())
            {
                if (gameObject != null)
                    Destroy(gameObject);
            }

            attachedObjects.Clear();
        }

        private Dictionary<GameObject, Tuple<Vector3, Vector3, Vector3>> attachedObjects;
        private Dictionary<GameObject, Vector3> prevScale = new Dictionary<GameObject, Vector3>();
    }
}
