namespace MapEditorReborn.Events.Handlers
{
    using EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// <see cref="API.Features.Components.MapEditorObject"/> related events.
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
        /// Invoked before changing a <see cref="API.Features.Components.MapEditorObject.RelativePosition"/>.
        /// </summary>
        public static event CustomEventHandler<ChangingObjectPositionEventArgs> ChangingObjectPosition;

        /// <summary>
        /// Invoked before changing a <see cref="API.Features.Components.MapEditorObject.RelativeRotation"/>.
        /// </summary>
        public static event CustomEventHandler<ChangingObjectRotationEventArgs> ChangingObjectRotation;

        /// <summary>
        /// Invoked before changing a <see cref="API.Features.Components.MapEditorObject.Scale"/>.
        /// </summary>
        public static event CustomEventHandler<ChangingObjectScaleEventArgs> ChangingObjectScale;

        /// <summary>
        /// Invoked before grabbing a <see cref="API.Features.Components.MapEditorObject"/>.
        /// </summary>
        public static event CustomEventHandler<GrabbingObjectEventArgs> GrabbingObject;

        /// <summary>
        /// Invoked before releasing a <see cref="API.Features.Components.MapEditorObject"/>.
        /// </summary>
        public static event CustomEventHandler<ReleasingObjectEventArgs> ReleasingObject;

        /// <summary>
        /// Invoked before bringing a <see cref="API.Features.Components.MapEditorObject"/>.
        /// </summary>
        public static event CustomEventHandler<BringingObjectEventArgs> BringingObject;

        /// <summary>
        /// Invoked before showing a <see cref="API.Features.Components.MapEditorObject"/>'s hint.
        /// </summary>
        public static event CustomEventHandler<ShowingObjectHintEventArgs> ShowingObjectHint;

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

        /// <summary>
        /// Called before changing a <see cref="API.Features.Components.MapEditorObject.RelativePosition"/>.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingObjectPositionEventArgs"/> instance.</param>
        internal static void OnChangingObjectPosition(ChangingObjectPositionEventArgs ev) => ChangingObjectPosition.InvokeSafely(ev);

        /// <summary>
        /// Called before changing a <see cref="API.Features.Components.MapEditorObject.RelativeRotation"/>.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingObjectRotationEventArgs"/> instance.</param>
        internal static void OnChangingObjectRotation(ChangingObjectRotationEventArgs ev) => ChangingObjectRotation.InvokeSafely(ev);

        /// <summary>
        /// Called before changing a <see cref="API.Features.Components.MapEditorObject.Scale"/>.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingObjectScaleEventArgs"/> instance.</param>
        internal static void OnChangingObjectScale(ChangingObjectScaleEventArgs ev) => ChangingObjectScale.InvokeSafely(ev);

        /// <summary>
        /// Called before grabbing a <see cref="API.Features.Components.MapEditorObject"/>.
        /// </summary>
        /// <param name="ev">The <see cref="GrabbingObjectEventArgs"/> instance.</param>
        internal static void OnGrabbingObject(GrabbingObjectEventArgs ev) => GrabbingObject.InvokeSafely(ev);

        /// <summary>
        /// Called before releasing a <see cref="API.Features.Components.MapEditorObject"/>.
        /// </summary>
        /// <param name="ev">The <see cref="ReleasingObjectEventArgs"/> instance.</param>
        internal static void OnReleasingObject(ReleasingObjectEventArgs ev) => ReleasingObject.InvokeSafely(ev);

        /// <summary>
        /// Called before bringing a <see cref="API.Features.Components.MapEditorObject"/>.
        /// </summary>
        /// <param name="ev">The <see cref="BringingObjectEventArgs"/> instance.</param>
        internal static void OnBringingObject(BringingObjectEventArgs ev) => BringingObject.InvokeSafely(ev);

        /// <summary>
        /// Called before showing a <see cref="API.Features.Components.MapEditorObject"/>'s hint.
        /// </summary>
        /// <param name="ev">The <see cref="BringingObjectEventArgs"/> instance.</param>
        internal static void OnShowingObjectHint(ShowingObjectHintEventArgs ev) => ShowingObjectHint.InvokeSafely(ev);
    }
}
