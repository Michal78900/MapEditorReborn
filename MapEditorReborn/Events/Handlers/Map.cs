// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.Handlers
{
    using API.Features.Serializable;
    using EventArgs;
    using Exiled.Events.Features;

    /// <summary>
    /// <see cref="MapSchematic"/> related events.
    /// </summary>
    public static class Map
    {
        /// <summary>
        /// Invoked before loading a map.
        /// </summary>
        public static Event<LoadingMapEventArgs> LoadingMap { get; set; } = new();

        /// <summary>
        /// Invoked before unloading a map.
        /// </summary>
        public static Event<UnloadingMapEventArgs> UnloadingMap { get; set; } = new();

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
