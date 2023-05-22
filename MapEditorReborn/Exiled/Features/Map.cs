// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Hazards;
using LightContainmentZoneDecontamination;
using MapEditorReborn.Exiled.Enums;
using MapEditorReborn.Exiled.Features.Items;
using MapEditorReborn.Exiled.Features.Pickups;
using MapEditorReborn.Exiled.Features.Toys;
using MapGeneration;
using MapGeneration.Distributors;
using PlayerRoles.PlayableScps.Scp939;
using UnityEngine;

namespace MapEditorReborn.Exiled.Features
{

    using Random = UnityEngine.Random;
    using Scp173GameRole = PlayerRoles.PlayableScps.Scp173.Scp173Role;
    using Scp939GameRole = PlayerRoles.PlayableScps.Scp939.Scp939Role;

    /// <summary>
    /// A set of tools to easily handle the in-game map.
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class Map
    {
        /// <summary>
        /// A list of <see cref="Locker"/>s on the map.
        /// </summary>
        internal static readonly List<Locker> LockersValue = new(250);

        /// <summary>
        /// A list of <see cref="PocketDimensionTeleport"/>s on the map.
        /// </summary>
        internal static readonly List<PocketDimensionTeleport> TeleportsValue = new(8);

        /// <summary>
        /// A list of <see cref="AdminToy"/>s on the map.
        /// </summary>
        internal static readonly List<AdminToy> ToysValue = new();

        private static readonly ReadOnlyCollection<PocketDimensionTeleport> ReadOnlyTeleportsValue = TeleportsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<Locker> ReadOnlyLockersValue = LockersValue.AsReadOnly();
        private static readonly ReadOnlyCollection<AdminToy> ReadOnlyToysValue = ToysValue.AsReadOnly();

        private static TantrumEnvironmentalHazard tantrumPrefab;
        private static Scp939AmnesticCloudInstance amnesticCloudPrefab;

        /// <summary>
        /// Gets the tantrum prefab.
        /// </summary>
        // public static TantrumEnvironmentalHazard TantrumPrefab
        // {
        //     get
        //     {
        //         if (tantrumPrefab == null)
        //         {
        //             Scp173GameRole scp173Role = RoleTypeId.Scp173.GetRoleBase() as Scp173GameRole;
        //
        //             if (scp173Role.SubroutineModule.TryGetSubroutine(out Scp173TantrumAbility scp173TantrumAbility))
        //                 tantrumPrefab = scp173TantrumAbility._tantrumPrefab;
        //         }
        //
        //         return tantrumPrefab;
        //     }
        // }

        /// <summary>
        /// Gets the amnestic cloud prefab.
        /// </summary>
        // public static Scp939AmnesticCloudInstance AmnesticCloudPrefab
        // {
        //     get
        //     {
        //         if (amnesticCloudPrefab == null)
        //         {
        //             Scp939GameRole scp939Role = RoleTypeId.Scp939.GetRoleBase() as Scp939GameRole;
        //
        //             if (scp939Role.SubroutineModule.TryGetSubroutine(out Scp939AmnesticCloudAbility ability))
        //                 amnesticCloudPrefab = ability._instancePrefab;
        //         }
        //
        //         return amnesticCloudPrefab;
        //     }
        // }

        /// <summary>
        /// Gets a value indicating whether decontamination has begun in the light containment zone.
        /// </summary>
        public static bool IsLczDecontaminated => DecontaminationController.Singleton._stopUpdating; // && !DecontaminationController.Singleton.disableDecontamination;

        /// <summary>
        /// Gets all <see cref="PocketDimensionTeleport"/> objects.
        /// </summary>
        public static ReadOnlyCollection<PocketDimensionTeleport> PocketDimensionTeleports => ReadOnlyTeleportsValue;

        /// <summary>
        /// Gets all <see cref="Locker"/> objects.
        /// </summary>
        public static ReadOnlyCollection<Locker> Lockers => ReadOnlyLockersValue;

        /// <summary>
        /// Gets all <see cref="AdminToy"/> objects.
        /// </summary>
        public static ReadOnlyCollection<AdminToy> Toys => ReadOnlyToysValue;

        /// <summary>
        /// Gets or sets the current seed of the map.
        /// </summary>
        public static int Seed
        {
            get => SeedSynchronizer.Seed;
            set
            {
                if (!SeedSynchronizer.MapGenerated)
                    SeedSynchronizer._singleton.Network_syncSeed = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="global::AmbientSoundPlayer"/>.
        /// </summary>
        public static AmbientSoundPlayer AmbientSoundPlayer { get; internal set; }

        /// <summary>
        /// Tries to find the room that a <see cref="GameObject"/> is inside, first using the <see cref="Transform"/>'s parents, then using a Raycast if no room was found.
        /// </summary>
        /// <param name="objectInRoom">The <see cref="GameObject"/> inside the room.</param>
        /// <returns>The <see cref="Room"/> that the <see cref="GameObject"/> is located inside. Can be <see langword="null"/>.</returns>
        /// <seealso cref="Room.Get(Vector3)"/>
        [Obsolete("Use Room.FindParentRoom(GameObject) instead.")]
        public static Room FindParentRoom(GameObject objectInRoom) => Room.FindParentRoom(objectInRoom);

        /// <summary>
        /// Turns off all lights in the facility.
        /// </summary>
        /// <param name="duration">The duration of the blackout.</param>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        public static void TurnOffAllLights(float duration, ZoneType zoneTypes = ZoneType.Unspecified)
        {
            foreach (FlickerableLightController controller in FlickerableLightController.Instances)
            {
                Room room = controller.GetComponentInParent<Room>();
                if (room is null)
                    continue;

                if (zoneTypes == ZoneType.Unspecified || (room is not null && (zoneTypes == room.Zone)))
                    controller.ServerFlickerLights(duration);
            }
        }

        /// <summary>
        /// Turns off all lights in the facility.
        /// </summary>
        /// <param name="duration">The duration of the blackout.</param>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        public static void TurnOffAllLights(float duration, IEnumerable<ZoneType> zoneTypes)
        {
            foreach (ZoneType zone in zoneTypes)
                TurnOffAllLights(duration, zone);
        }

        /// <summary>
        /// Gets a random <see cref="Locker"/>.
        /// </summary>
        /// <returns><see cref="Locker"/> object.</returns>
        public static Locker GetRandomLocker() => Lockers[Random.Range(0, Lockers.Count)];

        /// <summary>
        /// Gets a random <see cref="Pickup"/>.
        /// </summary>
        /// <param name="type">Filters by <see cref="ItemType"/>.</param>
        /// <returns><see cref="Pickup"/> object.</returns>
        public static Pickup GetRandomPickup(ItemType type = ItemType.None)
        {
            List<Pickup> pickups = (type != ItemType.None ? Pickup.List.Where(p => p.Type == type) : Pickup.List).ToList();
            return pickups[Random.Range(0, pickups.Count)];
        }

        /// <summary>
        /// Plays a random ambient sound.
        /// </summary>
        public static void PlayAmbientSound() => AmbientSoundPlayer.GenerateRandom();

        /// <summary>
        /// Plays an ambient sound.
        /// </summary>
        /// <param name="id">The id of the sound to play.</param>
        public static void PlayAmbientSound(int id)
        {
            if (id >= AmbientSoundPlayer.clips.Length)
                throw new IndexOutOfRangeException($"There are only {AmbientSoundPlayer.clips.Length} sounds available.");

            AmbientSoundPlayer.RpcPlaySound(AmbientSoundPlayer.clips[id].index);
        }

        /// <summary>
        /// Clears the lazy loading game object cache.
        /// </summary>
        internal static void ClearCache()
        {
            Room.RoomIdentifierToRoom.Clear();
            Door.DoorVariantToDoor.Clear();
            Pickup.BaseToPickup.Clear();
            Item.BaseToItem.Clear();
            TeleportsValue.Clear();
            LockersValue.Clear();
            //Ragdoll.BasicRagdollToRagdoll.Clear();
            Firearm.ItemTypeToFirearmInstance.Clear();
            Firearm.BaseCodesValue.Clear();
            Firearm.AvailableAttachmentsValue.Clear();
        }
    }
}