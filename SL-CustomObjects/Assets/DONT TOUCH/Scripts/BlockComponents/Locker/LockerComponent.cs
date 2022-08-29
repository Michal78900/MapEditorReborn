using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class LockerComponent : SchematicBlock
{
    [ReorderableList]
    public List<LockerChamber> Chambers = new List<LockerChamber>();
    
    [ReorderableList]
    public List<string> AllowedRoleTypes = new List<string>
    {
        "Scp0492",
        "Scp049",
        "Scp096",
        "Scp106",
        "Scp173",
        "Scp93953",
        "Scp93989",
        "ClassD",
        "Scientist",
        "FacilityGuard",
        "NtfPrivate",
        "NtfSergeant",
        "NtfSpecialist",
        "NtfCaptain",
        "ChaosConscript",
        "ChaosRifleman",
        "ChaosRepressor",
        "ChaosMarauder",
        "Tutorial",
    };
    
    public bool ShuffleChambers = true;
    
    public KeycardPermissions KeycardPermissions = KeycardPermissions.None;
    
    public ushort OpenedChambers = 0;
    
    [Tooltip("Locks the locker after it was interacted with.")]
    public bool InteractLock = false;
    
    [Tooltip("The chance for this locker to spawn.")]
    [Label("Chance %")]
    [MinValue(0f), MaxValue(100f)]
    public float Chance = 100f;
    
    [HideInInspector]
    public LockerType LockerType;
    
    public override BlockType BlockType => BlockType.Locker;

    public override bool Compile(SchematicBlockData block, Schematic _)
    {
        block.Rotation = transform.localEulerAngles;
        block.Scale = transform.localScale;
        block.BlockType = BlockType.Locker;

        Dictionary<int, List<SerializableLockerItem>> chambers = new Dictionary<int, List<SerializableLockerItem>>(Chambers.Count);
        int i = 0;

        foreach (LockerChamber chamber in Chambers)
        {
            List<SerializableLockerItem> listOfItems = new List<SerializableLockerItem>(chamber.PossibleItems.Count);

            foreach (LockerItem possibleItem in chamber.PossibleItems)
            {
                listOfItems.Add(new SerializableLockerItem(possibleItem));
            }

            chambers.Add(i, listOfItems);
            i++;
        }

        block.Properties = new Dictionary<string, object>
        {
            { "LockerType", LockerType },
            { "Chambers", chambers },
            { "ShuffleChambers", ShuffleChambers },
            { "AllowedRoleTypes", AllowedRoleTypes },
            { "KeycardPermissions", KeycardPermissions },
            { "OpenedChambers", OpenedChambers },
            { "InteractLock", InteractLock },
            { "Chance", Chance },
        };

        return true;
    }
}