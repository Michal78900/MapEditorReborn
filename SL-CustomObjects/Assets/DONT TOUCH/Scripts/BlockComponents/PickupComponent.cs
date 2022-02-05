
using UnityEngine;

public class PickupComponent : SchematicBlock
{
    [Tooltip("The ItemType of this pickup.")]
    public ItemType ItemType;

    [Tooltip("Whether the pickup should ignore gravity and all other physic interactions.")]
    public bool UseGravity = true;

    [Tooltip("Whether the pickup can be picked up in game. If set to false this can be used as custom butoon via a serperate plugin.")]
    public bool CanBePickedUp = true;

    public override BlockType BlockType => BlockType.Pickup;
}

