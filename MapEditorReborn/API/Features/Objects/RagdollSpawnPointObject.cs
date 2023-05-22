// -----------------------------------------------------------------------
// <copyright file="RagdollSpawnPointObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using MapEditorReborn.Exiled.Features;
using Mirror;
using PlayerRoles.Ragdolls;

namespace MapEditorReborn.API.Features.Objects;

using Serializable;
using PlayerStatsSystem;
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
    public override bool IsScalable
    {
        get => false;
    }

    /// <summary>
    /// Initializes the <see cref="RagdollSpawnPointObject"/>.
    /// </summary>
    /// <param name="ragdollSpawnPointSerializable">The <see cref="RagdollSpawnPointSerializable"/> to instantiate.</param>
    /// <returns>Instance of this component.</returns>
    public RagdollSpawnPointObject Init(RagdollSpawnPointSerializable ragdollSpawnPointSerializable)
    {
        Base = ragdollSpawnPointSerializable;

        ForcedRoomType = ragdollSpawnPointSerializable.RoomType != Exiled.Enums.RoomType.Unknown ? ragdollSpawnPointSerializable.RoomType : FindRoom().Type;
        UpdateObject();

        return this;
    }

    /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
    public override void UpdateObject()
    {
        OnDestroy();

        if (Random.Range(0, 101) > Base.SpawnChance)
            return;

        if (CurrentLoadedMap != null && string.IsNullOrEmpty(Base.Name) && CurrentLoadedMap.RagdollRoleNames.TryGetValue(Base.RoleType, out var ragdollNames))
        {
            Base.Name = ragdollNames[Random.Range(0, ragdollNames.Count)];
        }
        
        RagdollData ragdollInfo;

        if (byte.TryParse(Base.DeathReason, out var deathReasonId) && deathReasonId <= 22)
            ragdollInfo = new RagdollData(PluginAPI.Core.Server.Instance.ReferenceHub, new UniversalDamageHandler(-1f, DeathTranslations.TranslationsById[deathReasonId]), Base.RoleType, transform.position, transform.rotation, Base.Name, double.MaxValue);
        else
            ragdollInfo = new RagdollData(PluginAPI.Core.Server.Instance.ReferenceHub, new CustomReasonDamageHandler(Base.DeathReason), Base.RoleType, transform.position, transform.rotation, Base.Name, double.MaxValue);

       
        GameObject oGameObject = Instantiate(PluginAPI.Core.Server.Instance.GameObject);
        if (oGameObject.TryGetComponent(out BasicRagdoll component))
            component.NetworkInfo = ragdollInfo;

        NetworkServer.Spawn(oGameObject);
        AttachedRagdoll = component;
    }

    private void OnDestroy()
    {
        NetworkServer.Destroy(AttachedRagdoll.gameObject);
        AttachedRagdoll = null;
    }

    /// <summary>
    /// The attached <see cref="Ragdoll"/>.
    /// </summary>
    public BasicRagdoll AttachedRagdoll;
}