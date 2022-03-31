using UnityEngine;

public class SerializableRigidbody
{
    public SerializableRigidbody()
    {
    }

    public SerializableRigidbody(bool isKinematic, bool useGravity, RigidbodyConstraints constraints, float mass)
    {
        IsKinematic = isKinematic;
        UseGravity = useGravity;
        Constraints = constraints;
        Mass = mass;
    }

    public bool IsKinematic { get; set; } = false;

    public bool UseGravity { get; set; } = true;

    public RigidbodyConstraints Constraints { get; set; } = RigidbodyConstraints.None;

    public float Mass { get; set; } = 1f;
}
