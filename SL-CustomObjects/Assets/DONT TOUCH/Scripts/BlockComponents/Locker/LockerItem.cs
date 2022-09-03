using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class LockerItem
{
    public LockerItem()
    {
    }

    public LockerItem(SerializableLockerItem serializableLockerItem)
    {
        if (Enum.TryParse(serializableLockerItem.Item, out ItemType itemType))
        {
            ItemType = itemType;
        }
        else
        {
            CustomItem = serializableLockerItem.Item;
        }
        
        Count = serializableLockerItem.Count;
        Attachments = serializableLockerItem.Attachments;
        Chance = serializableLockerItem.Chance;
    }
    
    [Tooltip("The ItemType of this pickup.")]
    public ItemType ItemType;

    [Label("Custom Item name/ID")]
    public string CustomItem;

    public uint Count = 1;

    [ReorderableList]
    public List<AttachmentName> Attachments = new List<AttachmentName>();

    public float Chance = 100;
}