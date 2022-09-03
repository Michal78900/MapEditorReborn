using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class PickupComponent : SchematicBlock
{
    [Tooltip("The ItemType of this pickup.")]
    public ItemType ItemType;

    [Label("Custom Item name/ID")]
    public string CustomItem;

    [ReorderableList]
    public List<AttachmentName> Attachments = new List<AttachmentName>();

    [Tooltip("The chance for this pickup to spawn.")]
    [Label("Chance %")]
    [MinValue(0f), MaxValue(100f)]
    public float Chance = 100f;

    [Tooltip("Number of times you can use the pickup it dissappears. Set to -1 for no limit.")]
    [MinValue(-1)]
    public int NumberOfUses = 1;

    [Label("Can be picked up (untick for button)")]
    [Tooltip("Whether the pickup can be picked up in game. If set to false this can be used as custom button via a serperate plugin.")]
    [OnValueChanged("SwitchNumber")]
    public bool CanBePickedUp = true;
    
    private void SwitchNumber() => _ = CanBePickedUp ? NumberOfUses = 1 : NumberOfUses = -1;

    public override BlockType BlockType => BlockType.Pickup;

    public override bool Compile(SchematicBlockData block, Schematic _)
    {
        block.Rotation = transform.localEulerAngles;
        block.Scale = transform.localScale;

        block.BlockType = BlockType.Pickup;
        block.Properties = new Dictionary<string, object>
        {
            { "ItemType", ItemType },
            { "CustomItem", CustomItem },
            { "Attachments", Attachments },
            { "Chance", Chance },
            { "Uses", NumberOfUses },
        };

        if (!CanBePickedUp)
            block.Properties.Add("Locked", string.Empty);

        return true;
    }
}

