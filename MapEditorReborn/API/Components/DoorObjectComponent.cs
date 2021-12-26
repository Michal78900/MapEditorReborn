namespace MapEditorReborn.API
{
    using UnityEngine;

    /// <summary>
    /// Used for handling custom shit.
    /// </summary>
    public class DoorObjectComponent : MonoBehaviour
    {
        /// <summary>
        /// When set to <see langword="false"/> the door won't open on warhead activation.
        /// </summary>
        public bool OpenOnWarheadActivation = false;
    }
}
