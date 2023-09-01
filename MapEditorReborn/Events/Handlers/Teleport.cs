// -----------------------------------------------------------------------
// <copyright file="Teleport.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.Handlers
{
    using EventArgs;
    using Exiled.Events.Features;

    /// <summary>
    /// Teleport related events.
    /// </summary>
    public static class Teleport
    {
        /// <summary>
        /// Invoked before teleporting.
        /// </summary>
        public static Event<TeleportingEventArgs> Teleporting { get; set; } = new ();

        /// <summary>
        /// Called before teleporting.
        /// </summary>
        /// <param name="ev">The <see cref="TeleportingEventArgs"/> instance.</param>
        internal static void OnTeleporting(TeleportingEventArgs ev) => Teleporting.InvokeSafely(ev);
    }
}
