// -----------------------------------------------------------------------
// <copyright file="Schematic.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.Handlers;

using EventArgs;

/// <summary>
/// Schematic related events.
/// </summary>
public static class Schematic
{
    public static event Events.CustomEventHandler<SchematicSpawnedEventArgs> SchematicSpawned;

    public static event Events.CustomEventHandler<ButtonInteractedEventArgs> ButtonInteracted;

    public static event Events.CustomEventHandler<SchematicDestroyedEventArgs> SchematicDestroyed;

    internal static void OnSchematicSpawned(SchematicSpawnedEventArgs ev) => SchematicSpawned?.Invoke(ev);

    internal static void OnButtonInteract(ButtonInteractedEventArgs ev) => ButtonInteracted?.Invoke(ev);

    internal static void OnSchematicDestroyed(SchematicDestroyedEventArgs ev) => SchematicDestroyed?.Invoke(ev);
}