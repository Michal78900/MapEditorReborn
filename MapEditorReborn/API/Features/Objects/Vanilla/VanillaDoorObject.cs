namespace MapEditorReborn.API.Features.Objects.Vanilla
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;
    using Mirror;
    using Serializable;
    using Serializable.Vanilla;
    using Object = UnityEngine.Object;

    public class VanillaDoorObject : DoorObject
    {
        private VanillaDoorSerializable _vanillaBase;

        public override DoorObject Init(DoorSerializable doorSerializable)
        {
            _vanillaBase = new(Door.IsOpen, Door.RequiredPermissions.RequiredPermissions, Door.Base is BreakableDoor breakableDoor ? breakableDoor._ignoredDamageSources : DoorDamageType.Weapon, Door.MaxHealth);
            Base = doorSerializable;

            Door.IsOpen = doorSerializable.IsOpen;
            Door.ChangeLock(doorSerializable.IsLocked ? DoorLockType.SpecialDoorFeature : DoorLockType.None);
            Door.RequiredPermissions.RequiredPermissions = doorSerializable.KeycardPermissions;
            Door.IgnoredDamageTypes = doorSerializable.IgnoredDamageSources;
            Door.MaxHealth = doorSerializable.DoorHealth;
            Door.Health = doorSerializable.DoorHealth;
            _remainingHealth = doorSerializable.DoorHealth;

            return this;
        }

        public override void UpdateObject()
        {
        }

        internal static void NameUnnamedDoors()
        {
            DoorNametagExtension.NamedDoors.Remove("049_GATE");
            DoorNametagExtension.NamedDoors.Add("049_GATE", null);
            Door.Get(DoorType.Scp049Gate).Base.gameObject.AddComponent<DoorNametagExtension>()._nametag = "049_GATE";
        }

        private void SetToDefault()
        {
            Door.IsOpen = _vanillaBase.IsOpen;
            Door.ChangeLock(DoorLockType.None);
            Door.RequiredPermissions.RequiredPermissions = _vanillaBase.KeycardPermissions;
            Door.IgnoredDamageTypes = _vanillaBase.IgnoredDamageSources;
            Door.MaxHealth = _vanillaBase.DoorHealth;
            Door.Health = _vanillaBase.DoorHealth;

            if (!Door.IsBroken && _remainingHealth > 0)
                return;

            if (Door.Base is BreakableDoor breakableDoor)
                breakableDoor.Network_destroyed = false;

            NetworkServer.UnSpawn(gameObject);
            NetworkServer.Spawn(gameObject);
        }

        internal static void SetDoor(string name, VanillaDoorSerializable vanillaDoorSerializable)
        {
            if (!name.Contains("GENERIC"))
            {
                Door door = Door.Get(name);
                if (door is null)
                {
                    Log.Warn($"\"{name}\" door does not exist!");
                    return;
                }

                VanillaDoors.Add((VanillaDoorObject)door.GameObject.AddComponent<VanillaDoorObject>().Init(vanillaDoorSerializable));
                return;
            }

            IEnumerable<Door> doors = Door.Get(x => x.Nametag == null && string.Equals(x.GameObject.name.GetBefore(' '), name.Split('_')[1], StringComparison.InvariantCultureIgnoreCase));

            foreach (Door door in doors)
                VanillaDoors.Add((VanillaDoorObject)door.GameObject.AddComponent<VanillaDoorObject>().Init(vanillaDoorSerializable));
        }

        internal static void UnSetAllDoors()
        {
            for (int i = 0; i < VanillaDoors.Count; i++)
            {
                VanillaDoors[i].SetToDefault();
                Destroy(VanillaDoors[i]);
            }

            VanillaDoors.Clear();
        }

        private static readonly List<VanillaDoorObject> VanillaDoors = new();
    }
}