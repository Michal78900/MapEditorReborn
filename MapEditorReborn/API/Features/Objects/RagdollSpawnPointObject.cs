// -----------------------------------------------------------------------
// <copyright file="RagdollSpawnPointObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Objects
{
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using PlayerRoles.Ragdolls;
    using PlayerStatsSystem;
    using Serializable;
    using UnityEngine;
    using static API;

    /// <summary>
    /// Component added to RagdollSpawnPointObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class RagdollSpawnPointObject : MapEditorObject
    {
        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public RagdollSpawnPointSerializable Base;

        /// <inheritdoc cref="MapEditorObject.IsScalable"/>
        public override bool IsScalable => false;

        /// <summary>
        /// Initializes the <see cref="RagdollSpawnPointObject"/>.
        /// </summary>
        /// <param name="ragdollSpawnPointSerializable">The <see cref="RagdollSpawnPointSerializable"/> to instantiate.</param>
        /// <returns>Instance of this component.</returns>
        public RagdollSpawnPointObject Init(RagdollSpawnPointSerializable ragdollSpawnPointSerializable)
        {
            Base = ragdollSpawnPointSerializable;

            ForcedRoomType = ragdollSpawnPointSerializable.RoomType != RoomType.Unknown ? ragdollSpawnPointSerializable.RoomType : FindRoom().Type;
            UpdateObject();

            return this;
        }

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            //OnDestroy();

            if (Random.Range(0, 101) > Base.SpawnChance)
                return;

            if (CurrentLoadedMap != null && string.IsNullOrEmpty(Base.Name) && CurrentLoadedMap.RagdollRoleNames.TryGetValue(Base.RoleType, out List<string> ragdollNames))
            {
                Base.Name = ragdollNames[Random.Range(0, ragdollNames.Count)];
            }


            RagdollData ragdollInfo;

            if (byte.TryParse(Base.DeathReason, out byte deathReasonId) && deathReasonId <= 22)
                ragdollInfo = new RagdollData(Server.Host.ReferenceHub, new UniversalDamageHandler(-1f, DeathTranslations.TranslationsById[deathReasonId]), Base.RoleType, transform.position, transform.rotation, Base.Name, double.MaxValue);
            else
                ragdollInfo = new RagdollData(Server.Host.ReferenceHub, new CustomReasonDamageHandler(Base.DeathReason), Base.RoleType, transform.position, transform.rotation, Base.Name, double.MaxValue);

            if (!Ragdoll.TryCreate(ragdollInfo, out Ragdoll ragdoll))
                return;

            AttachedRagdoll = ragdoll;
        }

        private void OnDestroy()
        {
            if (AttachedRagdoll == null)
                return;

            AttachedRagdoll.Destroy();
            AttachedRagdoll = null;
        }

        /// <summary>
        /// The attached <see cref="Ragdoll"/>.
        /// </summary>
        public Ragdoll AttachedRagdoll;
    }
}
