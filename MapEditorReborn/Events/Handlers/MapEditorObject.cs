// -----------------------------------------------------------------------
// <copyright file="MapEditorObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.Handlers
{
    using Commands.ModifyingCommands.Scale;
    using EventArgs;
    using Exiled.Events.Features;

    /// <summary>
    /// <see cref="API.Features.Objects.MapEditorObject"/> related events.
    /// </summary>
    public static class MapEditorObject
    {
        /// <summary>
        /// Invoked before deleting a <see cref="API.Features.Objects.MapEditorObject"/>.
        /// </summary>
        public static Event<DeletingObjectEventArgs> DeletingObject { get; set; } = new();

        /// <summary>
        /// Invoked before spawning a <see cref="API.Features.Objects.MapEditorObject"/>.
        /// </summary>
        public static Event<SpawningObjectEventArgs> SpawningObject { get; set; } = new ();

        /// <summary>
        /// Invoked before selecting a <see cref="API.Features.Objects.MapEditorObject"/>.
        /// </summary>
        public static Event<SelectingObjectEventArgs> SelectingObject { get; set; } = new ();

        /// <summary>
        /// Invoked before copying a <see cref="API.Features.Objects.MapEditorObject"/>.
        /// </summary>
        public static Event<CopyingObjectEventArgs> CopyingObject { get; set; } = new ();

        /// <summary>
        /// Invoked before changing a <see cref="API.Features.Objects.MapEditorObject.RelativePosition"/>.
        /// </summary>
        public static Event<ChangingObjectPositionEventArgs> ChangingObjectPosition { get; set; } = new ();

        /// <summary>
        /// Invoked before changing a <see cref="API.Features.Objects.MapEditorObject.RelativeRotation"/>.
        /// </summary>
        public static Event<ChangingObjectRotationEventArgs> ChangingObjectRotation { get; set; } = new ();

        /// <summary>
        /// Invoked before changing a <see cref="Scale"/>.
        /// </summary>
        public static Event<ChangingObjectScaleEventArgs> ChangingObjectScale { get; set; } = new ();

        /// <summary>
        /// Invoked before grabbing a <see cref="API.Features.Objects.MapEditorObject"/>.
        /// </summary>
        public static Event<GrabbingObjectEventArgs> GrabbingObject { get; set; } = new ();

        /// <summary>
        /// Invoked before releasing a <see cref="API.Features.Objects.MapEditorObject"/>.
        /// </summary>
        public static Event<ReleasingObjectEventArgs> ReleasingObject { get; set; } = new ();

        /// <summary>
        /// Invoked before bringing a <see cref="API.Features.Objects.MapEditorObject"/>.
        /// </summary>
        public static Event<BringingObjectEventArgs> BringingObject { get; set; } = new ();

        /// <summary>
        /// Invoked before showing a <see cref="API.Features.Objects.MapEditorObject"/>'s hint.
        /// </summary>
        public static Event<ShowingObjectHintEventArgs> ShowingObjectHint { get; set; } = new ();

        /// <summary>
        /// Called before deleting a <see cref="API.Features.Objects.MapEditorObject"/>.
        /// </summary>
        /// <param name="ev">The <see cref="DeletingObjectEventArgs"/> instance.</param>
        internal static void OnDeletingObject(DeletingObjectEventArgs ev) => DeletingObject.InvokeSafely(ev);

        /// <summary>
        /// Called before spawning a <see cref="API.Features.Objects.MapEditorObject"/>.
        /// </summary>
        /// <param name="ev">The <see cref="SpawningObjectEventArgs"/> instance.</param>
        internal static void OnSpawningObject(SpawningObjectEventArgs ev) => SpawningObject.InvokeSafely(ev);

        /// <summary>
        /// Called before selecting a <see cref="API.Features.Objects.MapEditorObject"/>.
        /// </summary>
        /// <param name="ev">The <see cref="SelectingObjectEventArgs"/> instance.</param>
        internal static void OnSelectingObject(SelectingObjectEventArgs ev) => SelectingObject.InvokeSafely(ev);

        /// <summary>
        /// Called before copying a <see cref="API.Features.Objects.MapEditorObject"/>.
        /// </summary>
        /// <param name="ev">The <see cref="SelectingObjectEventArgs"/> instance.</param>
        internal static void OnCopyingObject(CopyingObjectEventArgs ev) => CopyingObject.InvokeSafely(ev);

        /// <summary>
        /// Called before changing a <see cref="API.Features.Objects.MapEditorObject.RelativePosition"/>.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingObjectPositionEventArgs"/> instance.</param>
        internal static void OnChangingObjectPosition(ChangingObjectPositionEventArgs ev) => ChangingObjectPosition.InvokeSafely(ev);

        /// <summary>
        /// Called before changing a <see cref="API.Features.Objects.MapEditorObject.RelativeRotation"/>.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingObjectRotationEventArgs"/> instance.</param>
        internal static void OnChangingObjectRotation(ChangingObjectRotationEventArgs ev) => ChangingObjectRotation.InvokeSafely(ev);

        /// <summary>
        /// Called before changing a <see cref="Scale"/>.
        /// </summary>
        /// <param name="ev">The <see cref="ChangingObjectScaleEventArgs"/> instance.</param>
        internal static void OnChangingObjectScale(ChangingObjectScaleEventArgs ev) => ChangingObjectScale.InvokeSafely(ev);

        /// <summary>
        /// Called before grabbing a <see cref="API.Features.Objects.MapEditorObject"/>.
        /// </summary>
        /// <param name="ev">The <see cref="GrabbingObjectEventArgs"/> instance.</param>
        internal static void OnGrabbingObject(GrabbingObjectEventArgs ev) => GrabbingObject.InvokeSafely(ev);

        /// <summary>
        /// Called before releasing a <see cref="API.Features.Objects.MapEditorObject"/>.
        /// </summary>
        /// <param name="ev">The <see cref="ReleasingObjectEventArgs"/> instance.</param>
        internal static void OnReleasingObject(ReleasingObjectEventArgs ev) => ReleasingObject.InvokeSafely(ev);

        /// <summary>
        /// Called before bringing a <see cref="API.Features.Objects.MapEditorObject"/>.
        /// </summary>
        /// <param name="ev">The <see cref="BringingObjectEventArgs"/> instance.</param>
        internal static void OnBringingObject(BringingObjectEventArgs ev) => BringingObject.InvokeSafely(ev);

        /// <summary>
        /// Called before showing a <see cref="API.Features.Objects.MapEditorObject"/>'s hint.
        /// </summary>
        /// <param name="ev">The <see cref="BringingObjectEventArgs"/> instance.</param>
        internal static void OnShowingObjectHint(ShowingObjectHintEventArgs ev) => ShowingObjectHint.InvokeSafely(ev);
    }
}
