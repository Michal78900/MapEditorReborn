using MapEditorReborn.Exiled.Enums;
using PlayerRoles;

namespace MapEditorReborn.API.Features.Serializable;

using System.Collections.Generic;
using Enums;
using YamlDotNet.Serialization;

public class SerializableTeleport : SchematicBlockData
{
    public List<TargetTeleporter> TargetTeleporters { get; set; } = new()
    {
        new TargetTeleporter(),
    };

    public List<RoleTypeId> AllowedRoles { get; set; } = new()
    {
        RoleTypeId.Scp0492,
        RoleTypeId.Scp049,
        RoleTypeId.Scp096,
        RoleTypeId.Scp106,
        RoleTypeId.Scp173,
        RoleTypeId.Scp939,
        RoleTypeId.ClassD,
        RoleTypeId.Scientist,
        RoleTypeId.FacilityGuard,
        RoleTypeId.NtfPrivate,
        RoleTypeId.NtfSergeant,
        RoleTypeId.NtfSpecialist,
        RoleTypeId.NtfCaptain,
        RoleTypeId.ChaosConscript,
        RoleTypeId.ChaosMarauder,
        RoleTypeId.ChaosRepressor,
        RoleTypeId.ChaosRifleman,
        RoleTypeId.Tutorial
    };

    public float Cooldown { get; set; } = 10f;

    public TeleportFlags TeleportFlags { get; set; } = TeleportFlags.Player;

    public LockOnEvent LockOnEvent { get; set; } = LockOnEvent.None;

    public int TeleportSoundId { get; set; } = -1;

    public float? PlayerRotationX { get; set; } = null;

    public float? PlayerRotationY { get; set; } = null;

    public RoomType RoomType { get; set; } = RoomType.Unknown;

    [YamlIgnore]
    public override string Name { get; set; }

    [YamlIgnore]
    public override int ParentId { get; set; }

    [YamlIgnore]
    public override string AnimatorName { get; set; }

    [YamlIgnore]
    public override BlockType BlockType { get; set; }

    [YamlIgnore]
    public override Dictionary<string, object> Properties { get; set; }
}