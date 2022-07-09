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
        Item = !string.IsNullOrEmpty(lockerItem.CustomItem) ? lockerItem.CustomItem : lockerItem.ItemType.ToString();
        Count = lockerItem.Count;
        Attachments = lockerItem.Attachments;
        Chance = lockerItem.Chance;
    }
    
    public string Item { get; set; }
    
    public uint Count { get; set; }
    
    public List<AttachmentName> Attachments { get; set; }

    public float Chance { get; set; }
}