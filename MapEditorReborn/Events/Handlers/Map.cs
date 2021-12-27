namespace MapEditorReborn.Events.Handlers
{
    using EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// <see cref="API.Features.Objects.MapSchematic"/> related events.
    /// </summary>
    public static class Map
    {
        /// <summary>
        /// Invoked before loading a map.
        /// </summary>
        public static event CustomEventHandler<LoadingMapEventArgs> LoadingMap;

        /// <summary>
        /// Invoked before unloading a map.
        /// </summary>
        public static event CustomEventHandler<UnloadingMapEventArgs> UnloadingMap;

        /// <summary>
        /// Called before loading a map.
        /// </summary>
        /// <param name="ev">The <see cref="LoadingMapEventArgs"/> instance.</param>
        internal static void OnLoadingMap(LoadingMapEventArgs ev) => LoadingMap.InvokeSafely(ev);

        /// <summary>
        /// Called before unloading a map.
        /// </summary>
        /// <param name="ev">The <see cref="UnloadingMapEventArgs"/> instance.</param>
        internal static void OnUnloadingMap(UnloadingMapEventArgs ev) => UnloadingMap.InvokeSafely(ev);
    }
}
