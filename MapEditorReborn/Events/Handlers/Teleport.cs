namespace MapEditorReborn.Events.Handlers
{
    using EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// Teleport related events.
    /// </summary>
    public static class Teleport
    {
        /// <summary>
        /// Invoked before teleporting.
        /// </summary>
        public static event CustomEventHandler<TeleportingEventArgs> Teleporting;

        /// <summary>
        /// Called before teleporting.
        /// </summary>
        /// <param name="ev">The <see cref="TeleportingEventArgs"/> instance.</param>
        internal static void OnTeleporting(TeleportingEventArgs ev) => Teleporting.InvokeSafely(ev);
    }
}
