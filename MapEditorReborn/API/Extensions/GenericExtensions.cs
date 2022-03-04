namespace MapEditorReborn.API.Extensions
{
    using System.Globalization;
    using Enums;
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
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
            switch (targetType)
            {
                case ShootingTargetType.Sport:
                    return ObjectType.SportShootingTarget.GetObjectByMode();

                case ShootingTargetType.ClassD:
                    return ObjectType.DboyShootingTarget.GetObjectByMode();

                case ShootingTargetType.Binary:
                    return ObjectType.BinaryShootingTarget.GetObjectByMode();

                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts a spawnpoint's <see cref="string"/> tag to the corresponding <see cref="SpawnableTeam"/>.
        /// </summary>
        /// <param name="spawnPointTag">The spawnpoint's <see cref="string"/> tag to convert.</param>
        /// <returns>The corresponding <see cref="SpawnableTeam"/>.</returns>
        public static SpawnableTeam ConvertToSpawnableTeam(this string spawnPointTag)
        {
            switch (spawnPointTag)
            {
                case "SP_049":
                    return SpawnableTeam.Scp049;

                case "SP_079":
                    return SpawnableTeam.Scp079;

                case "SCP_096":
                    return SpawnableTeam.Scp096;

                case "SP_106":
                    return SpawnableTeam.Scp106;

                case "SP_173":
                    return SpawnableTeam.Scp173;

                case "SCP_939":
                    return SpawnableTeam.Scp939;

                case "SP_CDP":
                    return SpawnableTeam.ClassD;

                case "SP_RSC":
                    return SpawnableTeam.Scientist;

                case "SP_GUARD":
                    return SpawnableTeam.FacilityGuard;

                case "SP_MTF":
                    return SpawnableTeam.MTF;

                case "SP_CI":
                    return SpawnableTeam.Chaos;

                default:
                    return SpawnableTeam.Tutorial;
            }
        }

        /// <inheritdoc cref="Item.Spawn(Vector3, Quaternion)"/>
        public static Pickup CreatePickup(this Item item, Vector3 position, Quaternion rotation = default, Vector3? scale = null)
        {
            item.Base.PickupDropModel.Info.ItemId = item.Type;
            item.Base.PickupDropModel.Info.Position = position;
            item.Base.PickupDropModel.Info.Weight = item.Weight;
            item.Base.PickupDropModel.Info.Rotation = new LowPrecisionQuaternion(rotation);
            item.Base.PickupDropModel.NetworkInfo = item.Base.PickupDropModel.Info;
            item.Base.Category = ItemCategory.None;

            InventorySystem.Items.Pickups.ItemPickupBase ipb = Object.Instantiate(item.Base.PickupDropModel, position, rotation);
            if (ipb is InventorySystem.Items.Firearms.FirearmPickup firearmPickup)
            {
                if (item is Firearm firearm)
                {
                    firearmPickup.Status = new InventorySystem.Items.Firearms.FirearmStatus(firearm.Ammo, InventorySystem.Items.Firearms.FirearmStatusFlags.MagazineInserted, firearmPickup.Status.Attachments);
                }
                else
                {
                    byte ammo;
                    switch (item.Base)
                    {
                        case InventorySystem.Items.Firearms.AutomaticFirearm auto:
                            ammo = auto._baseMaxAmmo;
                            break;
                        case InventorySystem.Items.Firearms.Shotgun shotgun:
                            ammo = shotgun._ammoCapacity;
                            break;
                        case InventorySystem.Items.Firearms.Revolver revolver:
                            ammo = revolver.AmmoManagerModule.MaxAmmo;
                            break;
                        default:
                            ammo = 0;
                            break;
                    }

                    uint code = firearmPickup.Status.Attachments;
                    firearmPickup.Status = new InventorySystem.Items.Firearms.FirearmStatus(ammo, InventorySystem.Items.Firearms.FirearmStatusFlags.MagazineInserted, code);
                }

                firearmPickup.NetworkStatus = firearmPickup.Status;
            }

            ipb.transform.localScale = scale ?? Vector3.one;

            ipb.InfoReceived(default, item.Base.PickupDropModel.NetworkInfo);

            return Pickup.Get(ipb);
        }

        /// <inheritdoc cref="Exiled.API.Extensions.ReflectionExtensions.CopyProperties(object, object)"/>
        public static T CopyProperties<T>(this T target, object source)
        {
            Exiled.API.Extensions.ReflectionExtensions.CopyProperties(target, source);
            return target;
        }

        public static bool TryParseToFloat(this string s, out float result) => float.TryParse(s.Replace(',', '.'), NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result);
    }
}
