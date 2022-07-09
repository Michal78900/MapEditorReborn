using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class SerializableLockerItem
{
    public SerializableLockerItem()
    {
    }
    
    public SerializableLockerItem(LockerItem lockerItem)
    {
        ItemType = lockerItem.ItemType;
        CustomItem = lockerItem.CustomItem;
        Attachments = lockerItem.Attachments;
        Chance = lockerItem.Chance;
    }
    
    public ItemType ItemType { get; set; }
    
    public string CustomItem { get; set; }
    
    public List<AttachmentName> Attachments { get; set; }

    public float Chance { get; set; }
}