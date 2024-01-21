namespace MapEditorReborn.API.Features.Serializable
{
    using System.Collections.Generic;
    using Enums;
    using Exiled.API.Enums;
    using YamlDotNet.Serialization;

    public class SerializableTeleport : SchematicBlockData
    {
        public List<TargetTeleporter> TargetTeleporters { get; set; } = new()
        {
            new TargetTeleporter(),
        };

        public List<string> AllowedRoles { get; set; } = new()
        {
            "Scp0492",
            "Scp049",
            "Scp096",
            "Scp106",
            "Scp173",
            "Scp939",
            "Scp3114",
            "ZombieFlamingo",
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
            "AlphaFlamingo",
            "Flamingo",
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
}
