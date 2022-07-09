using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class LockerItem
{
    [Tooltip("The ItemType of this pickup.")]
    public ItemType ItemType;

    [Label("Custom Item name/ID")]
    public string CustomItem;

    [ReorderableList]
    public List<AttachmentName> Attachments = new List<AttachmentName>();

    public float Chance = 100;
}