namespace MapEditorReborn.API
{
    using Exiled.API.Enums;

    /// <summary>
    /// Component added to spawned WorkstationObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class WorkStationObjectComponent : MapEditorObject
    {
        /// <summary>
        /// Initializes the <see cref="WorkStationObjectComponent"/>.
        /// </summary>
        /// <param name="workStationObject">The <see cref="WorkStationObject"/> to instantiate.</param>
        /// <returns>Instance of this compoment.</returns>
        public WorkStationObjectComponent Init(WorkStationObject workStationObject)
        {
            Base = workStationObject;

            ForcedRoomType = workStationObject.RoomType != RoomType.Unknown ? workStationObject.RoomType : FindRoom().Type;
            UpdateObject();

            return this;
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public WorkStationObject Base;
    }
}
