// -----------------------------------------------------------------------
// <copyright file="GenericExtensions.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Extensions
{
    using System.Globalization;
    using Enums;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using InventorySystem.Items.Pickups;
    using UnityEngine;

    /// <summary>
    /// The extensions class which contains a few useful methods.
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        /// Gets or sets the <see cref="GameObject"/> prefab given a specified <see cref="ShootingTargetType"/>.
        /// </summary>
        /// <param name="targetType">The <see cref="ShootingTargetType"/> to check.</param>
        /// <returns>The corresponding <see cref="GameObject"/> prefab.</returns>
        public static GameObject GetShootingTargetObjectByType(this ShootingTargetType targetType)
        {
            return targetType switch
            {
                ShootingTargetType.Sport => ObjectType.SportShootingTarget.GetObjectByMode(),
                ShootingTargetType.ClassD => ObjectType.DboyShootingTarget.GetObjectByMode(),
                ShootingTargetType.Binary => ObjectType.BinaryShootingTarget.GetObjectByMode(),
                _ => null,
            };
        }

        /// <summary>
        /// Converts a spawnpoint's <see cref="string"/> tag to the corresponding <see cref="SpawnableTeam"/>.
        /// </summary>
        /// <param name="spawnPointTag">The spawnpoint's <see cref="string"/> tag to convert.</param>
        /// <returns>The corresponding <see cref="SpawnableTeam"/>.</returns>
        public static SpawnableTeam ConvertToSpawnableTeam(this string spawnPointTag)
        {
            return spawnPointTag switch
            {
                "SP_049" => SpawnableTeam.Scp049,
                "SP_079" => SpawnableTeam.Scp079,
                "SCP_096" => SpawnableTeam.Scp096,
                "SP_106" => SpawnableTeam.Scp106,
                "SP_173" => SpawnableTeam.Scp173,
                "SCP_939" => SpawnableTeam.Scp939,
                "SP_CDP" => SpawnableTeam.ClassD,
                "SP_RSC" => SpawnableTeam.Scientist,
                "SP_GUARD" => SpawnableTeam.FacilityGuard,
                "SP_MTF" => SpawnableTeam.MTF,
                "SP_CI" => SpawnableTeam.Chaos,
                _ => SpawnableTeam.Tutorial,
            };
        }

        /// <inheritdoc cref="Item.Spawn(Vector3, Quaternion)"/>
        public static Pickup CreatePickup(this Item item, Vector3 position, Quaternion rotation = default, Vector3? scale = null)
        {
            item.Base.PickupDropModel.Info.ItemId = item.Type;
            item.Base.PickupDropModel.Position = position;
            item.Base.PickupDropModel.Info.WeightKg = item.Weight;
            item.Base.PickupDropModel.Rotation = rotation;
            item.Base.PickupDropModel.NetworkInfo = item.Base.PickupDropModel.Info;
            item.Base.Category = ItemCategory.None;

            ItemPickupBase ipb = Object.Instantiate(item.Base.PickupDropModel, position, rotation);
            /*
            if (ipb is FirearmPickup firearmPickup)
            {
                if (item is Firearm firearm)
                {
                    firearmPickup.Status = new FirearmStatus(firearm.Ammo, FirearmStatusFlags.MagazineInserted, firearmPickup.Status.Attachments);
                }
                else
                {
                    byte ammo = item.Base switch
                    {
                        AutomaticFirearm auto => auto._baseMaxAmmo,
                        Shotgun shotgun => shotgun._ammoCapacity,
                        Revolver revolver => revolver.AmmoManagerModule.MaxAmmo,
                        _ => 0,
                    };

                    uint code = firearmPickup.Status.Attachments;
                    firearmPickup.Status = new FirearmStatus(ammo, FirearmStatusFlags.MagazineInserted, code);
                }

                firearmPickup.NetworkStatus = firearmPickup.Status;
            }
            */

            ipb.transform.localScale = scale ?? Vector3.one;

            ipb.InfoReceivedHook(default, item.Base.PickupDropModel.NetworkInfo);

            return Pickup.Get(ipb);
        }

        /// <inheritdoc cref="Exiled.API.Extensions.ReflectionExtensions.CopyProperties(object, object)"/>
        public static T CopyProperties<T>(this T target, object source)
        {
            ReflectionExtensions.CopyProperties(target, source);
            return target;
        }

        public static bool TryParseToFloat(this string s, out float result) => float.TryParse(s.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out result);
    }
}
