namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Features.Serializable;

    /// <summary>
    /// Contains all information before the <see cref="MapSchematic"/> is loaded.
    /// </summary>
    public class LoadingMapEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingMapEventArgs"/> class.
        /// </summary>
        /// <param name="oldMap"><inheritdoc cref="OldMap"/></param>
        /// <param name="newMap"><inheritdoc cref="NewMap"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public LoadingMapEventArgs(MapSchematic oldMap, MapSchematic newMap, bool isAllowed = true)
        {
            OldMap = oldMap;
            NewMap = newMap;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the currently loaded map.
        /// </summary>
        public MapSchematic OldMap { get; }

        /// <summary>
        /// Gets or sets the map that will be loaded.
        /// </summary>
        public MapSchematic NewMap { get; set; }

        /// <summary>
        /// Gets or sets the response to be displayed if the event cannot be executed.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the map will be loaded.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
