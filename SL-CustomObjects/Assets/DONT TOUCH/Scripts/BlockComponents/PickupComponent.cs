
using UnityEngine;

public class PickupComponent : SchematicBlock
{
    public ItemType ItemType;

    public bool UseGravity = true;

    public bool CanBePickedUp = true;

    public override BlockType BlockType => BlockType.Pickup;
}

