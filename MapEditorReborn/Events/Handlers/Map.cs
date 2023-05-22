// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.Handlers;

using API.Features.Serializable;
using EventArgs;

/// <summary>
/// <see cref="MapSchematic"/> related events.
/// </summary>
public static class Map
{
    /// <summary>
    /// Invoked before loading a map.
    /// </summary>
    public static event Events.CustomEventHandler<LoadingMapEventArgs> LoadingMap;

    /// <summary>
    /// Invoked before unloading a map.
    /// </summary>
    public static event Events.CustomEventHandler<UnloadingMapEventArgs> UnloadingMap;

    /// <summary>
    /// Called before loading a map.
    /// </summary>
    /// <param name="ev">The <see cref="LoadingMapEventArgs"/> instance.</param>
    internal static void OnLoadingMap(LoadingMapEventArgs ev) => LoadingMap?.Invoke(ev);

    /// <summary>
    /// Called before unloading a map.
    /// </summary>
    /// <param name="ev">The <see cref="UnloadingMapEventArgs"/> instance.</param>
    internal static void OnUnloadingMap(UnloadingMapEventArgs ev) => UnloadingMap?.Invoke(ev);
}