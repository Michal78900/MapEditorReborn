namespace MapEditorReborn.API
{
    /// <summary>
    /// Component added to spawned WorkstationObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class WorkstationObjectComponent : MapEditorObject
    {
        /// <summary>
        /// When set to <see langword="false"/> you won't be able to interact with this workstation.
        /// </summary>
        public bool IsInteractable = true;
    }
}
