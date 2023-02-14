// -----------------------------------------------------------------------
// <copyright file="WorkstationObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Objects
{
    using Exiled.API.Enums;
    using InventorySystem.Items.Firearms.Attachments;
    using MapGeneration.Distributors;
    using Serializable;
    using UnityEngine;

    /// <summary>
    /// Component added to spawned WorkstationObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class WorkstationObject : MapEditorObject
    {
        private void Awake()
        {
            Workstation = GetComponent<WorkstationController>();
            StructurePositionSync = GetComponent<StructurePositionSync>();
        }

        /// <summary>
        /// Initializes the <see cref="WorkstationObject"/>.
        /// </summary>
        /// <param name="workStationSerializable">The <see cref="WorkstationSerializable"/> to instantiate.</param>
        /// <returns>Instance of this component.</returns>
        public WorkstationObject Init(WorkstationSerializable workStationSerializable)
        {
            Base = workStationSerializable;
            ForcedRoomType = workStationSerializable.RoomType != RoomType.Unknown ? workStationSerializable.RoomType : FindRoom().Type;
            UpdateObject();

            return this;
        }

        public MapEditorObject Init(SchematicBlockData block)
        {
            base.Init(block);

            Base = new(block);
            UpdateObject();

            return this;
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public WorkstationSerializable Base;

        /// <summary>
        /// Gets the <see cref="WorkstationController"/> of the object.
        /// </summary>
        public WorkstationController Workstation { get; private set; }

        /// <summary>
        /// Gets the <see cref="StructurePositionSync"/> of the object.
        /// </summary>
        public StructurePositionSync StructurePositionSync { get; private set; }

        /// <inheritdoc cref="UpdateObject()"/>
        public override void UpdateObject()
        {
            StructurePositionSync.Network_position = transform.position;
            StructurePositionSync.Network_rotationY = (sbyte)Mathf.RoundToInt(transform.rotation.eulerAngles.y / 5.625f);
            Workstation.NetworkStatus = (byte)(Base.IsInteractable ? 0 : 4);

            if (!IsSchematicBlock)
                base.UpdateObject();
        }
    }
}