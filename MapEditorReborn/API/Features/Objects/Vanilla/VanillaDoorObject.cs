namespace MapEditorReborn.API.Features.Objects.Vanilla
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Interactables.Interobjects.DoorUtils;
    using Mirror;
    using Serializable;
    using Serializable.Vanilla;

    public class VanillaDoorObject : DoorObject
    {
        private VanillaDoorSerializable _vanillaBase;
        private BreakableDoor? _breakableDoor;

        public override DoorObject Init(DoorSerializable doorSerializable)
        {
            _breakableDoor = Door as BreakableDoor;
            _vanillaBase = new(Door.IsOpen, Door.RequiredPermissions.RequiredPermissions, _breakableDoor?.IgnoredDamage ?? DoorDamageType.Weapon, _breakableDoor?.MaxHealth ?? 0f);
            Base = doorSerializable;

            Door.IsOpen = doorSerializable.IsOpen;
            Door.ChangeLock(doorSerializable.IsLocked ? DoorLockType.SpecialDoorFeature : DoorLockType.None);
            Door.RequiredPermissions.RequiredPermissions = doorSerializable.KeycardPermissions;
            if (_breakableDoor != null)
            {
                _breakableDoor.IgnoredDamage = doorSerializable.IgnoredDamageSources;
                _breakableDoor.MaxHealth = doorSerializable.DoorHealth;
                _breakableDoor.Health = doorSerializable.DoorHealth;
                _remainingHealth = doorSerializable.DoorHealth;
            }

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
            if (_breakableDoor != null)
            {
                _breakableDoor.IgnoredDamage = _vanillaBase.IgnoredDamageSources;
                _breakableDoor.MaxHealth = _vanillaBase.DoorHealth;
                _breakableDoor.Health = _vanillaBase.DoorHealth;

                if (!_breakableDoor.IsDestroyed && _remainingHealth > 0)
                    return;

                _breakableDoor.Base.Network_destroyed = false;
            }

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