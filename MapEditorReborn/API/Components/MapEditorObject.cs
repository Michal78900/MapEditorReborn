namespace MapEditorReborn.API
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using UnityEngine;

    /// <summary>
    /// Component added to all MapEditorReborn objects. It contains properties that are common for most of the objects.
    /// </summary>
    public class MapEditorObject : MonoBehaviour
    {
        /// <summary>
        /// Gets the relative position of the object to the <see cref="Room"/> it is currently in.
        /// </summary>
        public Vector3 RelativePosition => CurrentRoom.Type == RoomType.Surface ? transform.position : CurrentRoom.transform.InverseTransformPoint(transform.position);

        /// <summary>
        /// Gets the relative rotation of the object to the <see cref="Room"/> it is currently in.
        /// It will also take into account if the object had a random rotation.
        /// </summary>
        public Vector3 RelativeRotation
        {
            get
            {
                Vector3 rotation = CurrentRoom.Type == RoomType.Surface ? transform.eulerAngles : transform.eulerAngles - CurrentRoom.transform.eulerAngles;

                if (gameObject.TryGetComponent(out ObjectRotationComponent rotationComponent))
                {
                    if (rotationComponent.XisRandom)
                        rotation.x = -1f;

                    if (rotationComponent.YisRandom)
                        rotation.y = -1f;

                    if (rotationComponent.ZisRandom)
                        rotation.z = -1f;
                }

                return rotation;
            }
        }

        /// <summary>
        /// Gets the scale of the object.
        /// </summary>
        public Vector3 Scale => transform.localScale;

        /// <summary>
        /// Gets the room in which the object is currently in. Must be asign manually.
        /// </summary>
        public Room CurrentRoom;

        /// <summary>
        /// Destroys the object.
        /// </summary>
        public void Destroy() => Destroy(gameObject);
    }
}
