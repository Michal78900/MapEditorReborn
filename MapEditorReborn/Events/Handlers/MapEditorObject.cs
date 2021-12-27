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
        /// Invoked before spawning a <see cref="API.Features.Components.MapEditorObject"/>.
        /// </summary>
        public static event CustomEventHandler<SpawningObjectEventArgs> SpawningObject;

        /// <summary>
        /// Invoked before selecting a <see cref="API.Features.Components.MapEditorObject"/>.
        /// </summary>
        public static event CustomEventHandler<SelectingObjectEventArgs> SelectingObject;

        /// <summary>
        /// Invoked before copying a <see cref="API.Features.Components.MapEditorObject"/>.
        /// </summary>
        public static event CustomEventHandler<CopyingObjectEventArgs> CopyingObject;

        /// <summary>
        /// Called before deleting a <see cref="API.Features.Components.MapEditorObject"/>.
        /// </summary>
        /// <param name="ev">The <see cref="DeletingObjectEventArgs"/> instance.</param>
        internal static void OnDeletingObject(DeletingObjectEventArgs ev) => DeletingObject.InvokeSafely(ev);

        /// <summary>
        /// Called before spawning a <see cref="API.Features.Components.MapEditorObject"/>.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningObjectEventArgs"/> instance.</param>
        internal static void OnSpawningObject(SpawningObjectEventArgs ev) => SpawningObject.InvokeSafely(ev);

        /// <summary>
        /// Called before selecting a <see cref="API.Features.Components.MapEditorObject"/>.
        /// </summary>
        /// <param name="ev">The <see cref="SelectingObjectEventArgs"/> instance.</param>
        internal static void OnSelectingObject(SelectingObjectEventArgs ev) => SelectingObject.InvokeSafely(ev);

        /// <summary>
        /// Called before copying a <see cref="API.Features.Components.MapEditorObject"/>.
        /// </summary>
        /// <param name="ev">The <see cref="SelectingObjectEventArgs"/> instance.</param>
        internal static void OnCopyingObject(CopyingObjectEventArgs ev) => CopyingObject.InvokeSafely(ev);
    }
}
