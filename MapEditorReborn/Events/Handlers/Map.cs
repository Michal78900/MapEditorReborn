namespace MapEditorReborn.Events.Handlers
{
    using API;
    using EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// <see cref="MapSchematic"/> related events.
    /// </summary>
    public static class Map
    {
        /// <summary>
        /// Invoked before loading a map.
        /// </summary>
        public static event CustomEventHandler<LoadingMapEventArgs> LoadingMap;

        /// <summary>
        /// Called before loading a map.
        /// </summary>
        /// <param name="ev">The <see cref="LoadingMapEventArgs"/> instance.</param>
        internal static void OnLoadingMap(LoadingMapEventArgs ev) => LoadingMap.InvokeSafely(ev);
    }
}
