using NaughtyAttributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class TeleportComponent : SchematicBlock
{
    public override BlockType BlockType => BlockType.Teleport;

    public RoomType RoomType = RoomType.Surface;
    
    [ReorderableList]
    public List<TargetTeleporter> TargetTeleporters = new List<TargetTeleporter>() { new TargetTeleporter() { ChanceToTeleport = 100 } };

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

    public float Cooldown = 10f;

    public TeleportFlags TeleportFlags = TeleportFlags.Player;

    public LockOnEvent LockOnEvent = LockOnEvent.None;

    [ShowIf("TeleportFlags", TeleportFlags.Player)]
    public bool OverridePlayerXRotation = false;

    [ShowIf("OverridePlayerXRotation")]
    public float PlayerRotationX;

    [ShowIf("TeleportFlags", TeleportFlags.Player)]
    public bool OverridePlayerYRotation = false;

    [ShowIf("OverridePlayerYRotation")]
    public float PlayerRotationY;
}

[System.Serializable]
public class TargetTeleporter
{
    public int Id { get; set; }

    public float Chance { get; set; }

    [JsonIgnore]
    [Tooltip("Drag and drop target teleporter here.")]
    public TeleportComponent Teleporter;

    [JsonIgnore]
    [Tooltip("Set chance of teleporting to this teleporter.")]
    public float ChanceToTeleport = 100f;

}
