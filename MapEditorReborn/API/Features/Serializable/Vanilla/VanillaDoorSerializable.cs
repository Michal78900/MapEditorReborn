namespace MapEditorReborn.API.Features.Serializable.Vanilla
{
    using Exiled.API.Enums;
    using Interactables.Interobjects.DoorUtils;
    using UnityEngine;
    using YamlDotNet.Serialization;
    using KeycardPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions;

    public class VanillaDoorSerializable : DoorSerializable
    {
        public VanillaDoorSerializable()
        {
        }

        public VanillaDoorSerializable(bool isOpen, KeycardPermissions keycardPermissions, DoorDamageType ignoredDamageSources, float doorHealth)
        {
            IsOpen = isOpen;
            KeycardPermissions = keycardPermissions;
            IgnoredDamageSources = ignoredDamageSources;
            DoorHealth = doorHealth;
        }

        [YamlIgnore]
        public override DoorType DoorType { get; set; }

        [YamlIgnore]
        public override Vector3 Position { get; set; }

        [YamlIgnore]
        public override Vector3 Rotation { get; set; }

        [YamlIgnore]
        public override Vector3 Scale { get; set; }

        [YamlIgnore]
        public override RoomType RoomType { get; set; }
    }
}