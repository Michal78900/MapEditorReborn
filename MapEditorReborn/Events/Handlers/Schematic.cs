// -----------------------------------------------------------------------
// <copyright file="Schematic.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.Handlers
{
    using EventArgs;
    using Exiled.Events.Features;

    /// <summary>
    /// Schematic related events.
    /// </summary>
    public static class Schematic
    {
        public static Event<SchematicSpawnedEventArgs> SchematicSpawned { get; set; } = new ();

        public static Event<ButtonInteractedEventArgs> ButtonInteracted { get; set; } = new ();

        public static Event<SchematicDestroyedEventArgs> SchematicDestroyed { get; set; } = new ();

        internal static void OnSchematicSpawned(SchematicSpawnedEventArgs ev) => SchematicSpawned.InvokeSafely(ev);

        internal static void OnButtonInteract(ButtonInteractedEventArgs ev) => ButtonInteracted.InvokeSafely(ev);

        internal static void OnSchematicDestroyed(SchematicDestroyedEventArgs ev) => SchematicDestroyed.InvokeSafely(ev);
    }
}
