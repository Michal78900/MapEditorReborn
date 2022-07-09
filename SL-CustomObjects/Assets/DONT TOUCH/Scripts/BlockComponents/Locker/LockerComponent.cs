using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class LockerComponent : SchematicBlock
{
    [ReorderableList]
    public List<LockerChamber> Chambers = new List<LockerChamber>();
    
    [ReorderableList]
    public List<string> AllowedRoleTypes = new List<string>()
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
    
    [Tooltip("The chance for this locker to spawn.")]
    [Label("Chance %")]
    [MinValue(0f), MaxValue(100f)]
    public float Chance = 100f;
    
    [HideInInspector]
    public LockerType LockerType;
    
    public override BlockType BlockType => BlockType.Locker;
}