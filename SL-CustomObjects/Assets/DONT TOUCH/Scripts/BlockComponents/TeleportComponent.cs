﻿using NaughtyAttributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

public class TeleportComponent : SchematicBlock
{
    public override BlockType BlockType => BlockType.Teleport;
    
    public RoomType RoomType = RoomType.Surface;

    [ReorderableList]
    [ValidateInput("ValidateList", "The target teleport list is invalid. Make sure that your list:\n" +
        "- Is not empty\n" +
        "- Does not contain any duplicates\n" +
        "- One of the teleporters does not point to itself")]
    public List<TargetTeleporter> TargetTeleporters = new List<TargetTeleporter>() { new TargetTeleporter() { ChanceToTeleport = 100 } };

    [BoxGroup("Teleport properties")]
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

    [BoxGroup("Teleport properties")]
    public float Cooldown = 10f;

    [BoxGroup("Teleport properties")]
    public TeleportFlags TeleportFlags = TeleportFlags.Player;

    [BoxGroup("Teleport properties")]
    public LockOnEvent LockOnEvent = LockOnEvent.None;

    [BoxGroup("Player properties")]
    [ShowIf("TeleportFlags", TeleportFlags.Player)]
    [Tooltip("Plays the sound to the player on teleport.\n" +
        "Recommended values are:\n" +
        "- 2\n" +
        "- 6\n" +
        "- 7\n" +
        "- 24\n" +
        "- 27\n" +
        "- 30\n" +
        "- 31")]
    public bool PlaySoundOnTeleport = false;

    [BoxGroup("Player properties")]
    [ShowIf("PlaySoundOnTeleport")]
    [MinValue(0), MaxValue(31)]
    [Tooltip("Plays the sound to the player on teleport.\n" +
        "Recommended values are:\n" +
        "- 2\n" +
        "- 6\n" +
        "- 7\n" +
        "- 24\n" +
        "- 27\n" +
        "- 30\n" +
        "- 31")]
    public int SoundOnTeleport;

    [BoxGroup("Player properties")]
    [ShowIf("TeleportFlags", TeleportFlags.Player)]
    public bool OverridePlayerXRotation = false;

    [BoxGroup("Player properties")]
    [ShowIf("OverridePlayerXRotation")]
    [MinValue(-360f), MaxValue(360f)]
    public float PlayerRotationX;

    [BoxGroup("Player properties")]
    [ShowIf("TeleportFlags", TeleportFlags.Player)]
    public bool OverridePlayerYRotation = false;

    [BoxGroup("Player properties")]
    [ShowIf("OverridePlayerYRotation")]
    [MinValue(-360f), MaxValue(360f)]
    public float PlayerRotationY;

    internal bool ValidateList(List<TargetTeleporter> list)
    {
        if (list.Count == 0)
            return false;

        List<TeleportComponent> asd = new List<TeleportComponent>();

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Teleporter == null || list[i].Teleporter == this || asd.Contains(list[i].Teleporter))
                return false;

            asd.Add(list[i].Teleporter);
        }

        return true;
    }
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
