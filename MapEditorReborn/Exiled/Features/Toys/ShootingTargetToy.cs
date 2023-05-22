// -----------------------------------------------------------------------
// <copyright file="ShootingTargetToy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MapEditorReborn.API.Enums;
using MapEditorReborn.Exiled.Enums;
using MapEditorReborn.Exiled.Interfaces;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;

namespace MapEditorReborn.Exiled.Features.Toys
{

    using Object = UnityEngine.Object;
    using ShootingTarget = AdminToys.ShootingTarget;

    /// <summary>
    /// A wrapper class for <see cref="ShootingTarget"/>.
    /// </summary>
    public class ShootingTargetToy : AdminToy, IWrapper<ShootingTarget>
    {
        private static readonly Dictionary<string, ShootingTargetType> TypeLookup = new()
        {
            { "sportTargetPrefab", ShootingTargetType.Sport },
            { "dboyTargetPrefab", ShootingTargetType.ClassD },
            { "binaryTargetPrefab", ShootingTargetType.Binary },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ShootingTargetToy"/> class.
        /// </summary>
        /// <param name="target">The base <see cref="ShootingTarget"/> class.</param>
        internal ShootingTargetToy(ShootingTarget target)
            : base(target, AdminToyType.ShootingTarget)
        {
            Base = target;
            Type = TypeLookup.TryGetValue(Base.gameObject.name.Substring(0, Base.gameObject.name.Length - 7), out ShootingTargetType type) ? type : ShootingTargetType.Unknown;
        }

        /// <summary>
        /// Gets the base-game <see cref="ShootingTarget"/> for this target.
        /// </summary>
        public ShootingTarget Base { get; }

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the target.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the bullseye.
        /// </summary>
        public GameObject Bullseye => Base._bullsEye.gameObject;

        /// <summary>
        /// Gets the <see cref="Interactables.Verification.IVerificationRule"/> for this target.
        /// </summary>
        public Interactables.Verification.IVerificationRule VerificationRule => Base.VerificationRule;

        /// <summary>
        /// Gets the bullseye location of the target.
        /// </summary>
        public Vector3 BullseyePosition => Base._bullsEye.position;

        /// <summary>
        /// Gets the bullseye radius of the target.
        /// </summary>
        public float BullseyeRadius => Base._bullsEyeRadius;

        /// <summary>
        /// Gets or sets the max health of the target.
        /// </summary>
        public int MaxHealth
        {
            get => Base._maxHp;
            set
            {
                if (!IsSynced)
                    throw new InvalidOperationException("Attempted to set MaxHealth while target was not in sync mode.");
                Base._maxHp = value;
                Base.RpcSendInfo(MaxHealth, AutoResetTime);
            }
        }

        /// <summary>
        /// Gets or sets the remaining health of the target.
        /// </summary>
        public float Health
        {
            get => Base._hp;
            set
            {
                if (!IsSynced)
                    throw new InvalidOperationException("Attempted to set Health while target was not in sync mode.");
                Base._hp = value;
            }
        }

        /// <summary>
        /// Gets or sets the remaining health of the target.
        /// </summary>
        public int AutoResetTime
        {
            get => Base._autoDestroyTime;
            set
            {
                if (!IsSynced)
                    throw new InvalidOperationException("Attempted to set AutoResetTime while target was not in sync mode.");
                Base._autoDestroyTime = Mathf.Max(0, value);
                Base.RpcSendInfo(MaxHealth, AutoResetTime);
            }
        }

        /// <summary>
        /// Gets or sets the size scale of the target.
        /// </summary>
        public new Vector3 Scale
        {
            get => GameObject.transform.localScale;
            set
            {
                NetworkServer.UnSpawn(GameObject);
                GameObject.transform.localScale = value;
                NetworkServer.Spawn(GameObject);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the target is in sync mode.
        /// </summary>
        public bool IsSynced
        {
            get => Base.Network_syncMode;
            set => Base.Network_syncMode = value;
        }

        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        public ShootingTargetType Type { get; }

        /// <summary>
        /// Creates a new <see cref="ShootingTargetToy"/>.
        /// </summary>
        /// <param name="type">The <see cref="ShootingTargetType"/> of the <see cref="ShootingTargetToy"/>.</param>
        /// <param name="position">The position of the <see cref="ShootingTargetToy"/>.</param>
        /// <param name="rotation">The rotation of the <see cref="ShootingTargetToy"/>.</param>
        /// <param name="scale">The scale of the <see cref="ShootingTargetToy"/>.</param>
        /// <param name="spawn">Whether the <see cref="ShootingTargetToy"/> should be initially spawned.</param>
        /// <returns>The new <see cref="ShootingTargetToy"/>.</returns>
        public static ShootingTargetToy Create(ShootingTargetType type, Vector3? position = null, Vector3? rotation = null, Vector3? scale = null, bool spawn = true)
        {
            ShootingTargetToy shootingTargetToy;

            switch (type)
            {
                case ShootingTargetType.ClassD:
                    {
                        shootingTargetToy = new ShootingTargetToy(Object.Instantiate(ToysHelper.DboyShootingTargetObject));
                        break;
                    }

                case ShootingTargetType.Binary:
                    {
                        shootingTargetToy = new ShootingTargetToy(Object.Instantiate(ToysHelper.BinaryShootingTargetObject));
                        break;
                    }

                default:
                    {
                        shootingTargetToy = new ShootingTargetToy(Object.Instantiate(ToysHelper.SportShootingTargetObject));
                        break;
                    }
            }

            shootingTargetToy.AdminToyBase.transform.position = position ?? Vector3.zero;
            shootingTargetToy.AdminToyBase.transform.eulerAngles = rotation ?? Vector3.zero;
            shootingTargetToy.AdminToyBase.transform.localScale = scale ?? Vector3.one;

            if (spawn)
                shootingTargetToy.Spawn();

            return shootingTargetToy;
        }

        /// <summary>
        /// Gets the <see cref="ShootingTargetToy"/> belonging to the <see cref="ShootingTarget"/>.
        /// </summary>
        /// <param name="shootingTarget">The <see cref="ShootingTarget"/> instance.</param>
        /// <returns>The corresponding <see cref="ShootingTargetToy"/> instance.</returns>
        public static ShootingTargetToy Get(ShootingTarget shootingTarget)
        {
            AdminToy adminToy = Map.Toys.FirstOrDefault(x => x.AdminToyBase == shootingTarget);
            return adminToy is not null ? adminToy as ShootingTargetToy : new ShootingTargetToy(shootingTarget);
        }

        /// <summary>
        /// Clears the target and resets its health.
        /// </summary>
        public void Clear() => Base.ClearTarget();

        /// <summary>
        /// Damages the target with the given damage, item, footprint, and hit location.
        /// </summary>
        /// <param name="damage">The damage to be dealt.</param>
        /// <param name="damageHandler">The <see cref="DamageHandlerBase"/> dealing the damage.</param>
        /// <param name="exactHit">The exact location of the hit.</param>
        /// <returns>Whether or not the damage was sent.</returns>
        public bool Damage(float damage, DamageHandlerBase damageHandler, Vector3 exactHit) => Base.Damage(damage, damageHandler, exactHit);
    }
}