namespace MapEditorReborn.Events.Handlers
{
    using EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// <see cref="MapSchematic"/> related events.
    /// </summary>
    public static class MapEditorObject
    {
        /// <summary>
        /// Invoked before deleting a <see cref="API.Features.Components.MapEditorObject"/>.
        /// </summary>
        public static event CustomEventHandler<DeletingObjectEventArgs> DeletingObject;

        /// <summary>
        /// Called before deleting a <see cref="API.Features.Components.MapEditorObject"/>.
        /// </summary>
        /// <param name="ev">The <see cref="DeletingObjectEventArgs"/> instance.</param>
        internal static void OnDeletingObject(DeletingObjectEventArgs ev) => DeletingObject.InvokeSafely(ev);
    }
}
