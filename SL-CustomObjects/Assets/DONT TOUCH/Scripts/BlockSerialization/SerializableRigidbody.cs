using UnityEngine;

public class SerializableRigidbody
{
    public SerializableRigidbody()
    {
    }

    public SerializableRigidbody(Rigidbody rigidbody)
    {
        IsKinematic = rigidbody.isKinematic;
        UseGravity = rigidbody.useGravity;
        Constraints = rigidbody.constraints;
        Mass = rigidbody.mass;
    }

    public bool IsKinematic { get; set; } = false;

    public bool UseGravity { get; set; } = true;

    public RigidbodyConstraints Constraints { get; set; } = RigidbodyConstraints.None;

    public float Mass { get; set; } = 1f;
}
