namespace MapEditorReborn.API
{
    /// <summary>
    /// Component added to spawned WorkstationObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class WorkStationObjectComponent : MapEditorObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkStationObjectComponent"/> class.
        /// </summary>
        /// <param name="workStationObject">The <see cref="WorkStationObject"/> to instantiate.</param>
        /// <returns>Instance of this compoment.</returns>
        public WorkStationObjectComponent Init(WorkStationObject workStationObject)
        {
            Base = workStationObject;

            UpdateObject();

            return this;
        }

        public WorkStationObject Base;
    }
}
