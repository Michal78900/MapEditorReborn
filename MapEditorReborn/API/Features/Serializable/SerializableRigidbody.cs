namespace MapEditorReborn.API.Features.Serializable
{
    using UnityEngine;

    public class SerializableRigidbody
    {
        public bool IsKinematic { get; set; } = false;

        public bool UseGravity { get; set; } = true;

        public RigidbodyConstraints Constraints { get; set; } = RigidbodyConstraints.None;

        public float Mass { get; set; } = 1f;
    }
}
