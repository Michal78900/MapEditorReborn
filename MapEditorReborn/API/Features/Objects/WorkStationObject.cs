// -----------------------------------------------------------------------
// <copyright file="WorkstationObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Objects
{
    using Exiled.API.Enums;
    using Serializable;
    using InventorySystem.Items.Firearms.Attachments;
    using MapGeneration.Distributors;

    /// <summary>
    /// Component added to spawned WorkstationObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class WorkstationObject : MapEditorObject
    {
        private void Awake()
        {
            StructurePositionSync = GetComponent<StructurePositionSync>();
        }

        /// <summary>
        /// Initializes the <see cref="WorkstationObject"/>.
        /// </summary>
        /// <param name="workStationSerializable">The <see cref="WorkstationSerializable"/> to instantiate.</param>
        /// <returns>Instance of this compoment.</returns>
        public WorkstationObject Init(WorkstationSerializable workStationSerializable)
        {
            Base = workStationSerializable;

            if (TryGetComponent(out WorkstationController workstationController))
            {
                Workstation = workstationController;

                ForcedRoomType = workStationSerializable.RoomType != RoomType.Unknown ? workStationSerializable.RoomType : FindRoom().Type;

                UpdateObject();

                return this;
            }

            return null;
        }

        public MapEditorObject Init(SchematicBlockData block)
        {
            base.Init(block);

            Base = new (block);

            if (TryGetComponent(out WorkstationController workstationController))
                Workstation = workstationController;

            UpdateObject();

            return this;
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public WorkstationSerializable Base;

        public WorkstationController Workstation { get; private set; }

        public StructurePositionSync StructurePositionSync { get; private set; }

        /// <inheritdoc cref="UpdateObject()"/>
        public override void UpdateObject()
        {
            Workstation.NetworkStatus = (byte)(Base.IsInteractable ? 0 : 4);

            if (!IsSchematicBlock)
                base.UpdateObject();
        }
    }
}
