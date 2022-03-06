namespace MapEditorReborn.API.Features.Objects
{
    using Exiled.API.Enums;
    using Features.Serializable;
    using InventorySystem.Items.Firearms.Attachments;

    /// <summary>
    /// Component added to spawned WorkstationObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class WorkstationObject : MapEditorObject
    {
        /// <summary>
        /// Initializes the <see cref="WorkstationObject"/>.
        /// </summary>
        /// <param name="workStationSerializable">The <see cref="WorkstationSerializable"/> to instantiate.</param>
        /// <returns>Instance of this compoment.</returns>
        public WorkstationObject Init(WorkstationSerializable workStationSerializable)
        {
            Base = workStationSerializable;

            if (TryGetComponent(out WorkstationController workstationControllerComponent))
            {
                workstationController = workstationControllerComponent;

                ForcedRoomType = workStationSerializable.RoomType != RoomType.Unknown ? workStationSerializable.RoomType : FindRoom().Type;

                UpdateObject();

                return this;
            }

            return null;
        }

        /// <inheritdoc cref="UpdateObject()"/>
        public override void UpdateObject()
        {
            workstationController.NetworkStatus = (byte)(Base.IsInteractable ? 0 : 4);

            base.UpdateObject();
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public WorkstationSerializable Base;

        private WorkstationController workstationController;
    }
}
