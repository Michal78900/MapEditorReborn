// -----------------------------------------------------------------------
// <copyright file="MapEditorObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using MapEditorReborn.API.Features;
using MapEditorReborn.Exiled;

namespace MapEditorReborn.Events.Handlers;

using Commands.ModifyingCommands.Scale;
using EventArgs;

/// <summary>
/// <see cref="API.Features.Objects.MapEditorObject"/> related events.
/// </summary>
public static class MapEditorObject
{
    /// <summary>
    /// Invoked before deleting a <see cref="API.Features.Objects.MapEditorObject"/>.
    /// </summary>
    public static event Events.CustomEventHandler<DeletingObjectEventArgs> DeletingObject;

    /// <summary>
    /// Invoked before spawning a <see cref="API.Features.Objects.MapEditorObject"/>.
    /// </summary>
    public static event Events.CustomEventHandler<SpawningObjectEventArgs> SpawningObject;

    /// <summary>
    /// Invoked before selecting a <see cref="API.Features.Objects.MapEditorObject"/>.
    /// </summary>
    public static event Events.CustomEventHandler<SelectingObjectEventArgs> SelectingObject;

    /// <summary>
    /// Invoked before copying a <see cref="API.Features.Objects.MapEditorObject"/>.
    /// </summary>
    public static event Events.CustomEventHandler<CopyingObjectEventArgs> CopyingObject;

    /// <summary>
    /// Invoked before changing a <see cref="API.Features.Objects.MapEditorObject.RelativePosition"/>.
    /// </summary>
    public static event Events.CustomEventHandler<ChangingObjectPositionEventArgs> ChangingObjectPosition;

    /// <summary>
    /// Invoked before changing a <see cref="API.Features.Objects.MapEditorObject.RelativeRotation"/>.
    /// </summary>
    public static event Events.CustomEventHandler<ChangingObjectRotationEventArgs> ChangingObjectRotation;

    /// <summary>
    /// Invoked before changing a <see cref="Scale"/>.
    /// </summary>
    public static event Events.CustomEventHandler<ChangingObjectScaleEventArgs> ChangingObjectScale;

    /// <summary>
    /// Invoked before grabbing a <see cref="API.Features.Objects.MapEditorObject"/>.
    /// </summary>
    public static event Events.CustomEventHandler<GrabbingObjectEventArgs> GrabbingObject;

    /// <summary>
    /// Invoked before releasing a <see cref="API.Features.Objects.MapEditorObject"/>.
    /// </summary>
    public static event Events.CustomEventHandler<ReleasingObjectEventArgs> ReleasingObject;

    /// <summary>
    /// Invoked before bringing a <see cref="API.Features.Objects.MapEditorObject"/>.
    /// </summary>
    public static event Events.CustomEventHandler<BringingObjectEventArgs> BringingObject;

    /// <summary>
    /// Invoked before showing a <see cref="API.Features.Objects.MapEditorObject"/>'s hint.
    /// </summary>
    public static event Events.CustomEventHandler<ShowingObjectHintEventArgs> ShowingObjectHint;

    /// <summary>
    /// Called before deleting a <see cref="API.Features.Objects.MapEditorObject"/>.
    /// </summary>
    /// <param name="ev">The <see cref="DeletingObjectEventArgs"/> instance.</param>
    internal static void OnDeletingObject(DeletingObjectEventArgs ev) => DeletingObject?.Invoke(ev);

    /// <summary>
    /// Called before spawning a <see cref="API.Features.Objects.MapEditorObject"/>.
    /// </summary>
    /// <param name="ev">The <see cref="SpawningObjectEventArgs"/> instance.</param>
    internal static void OnSpawningObject(SpawningObjectEventArgs ev) => SpawningObject?.Invoke(ev);

    /// <summary>
    /// Called before selecting a <see cref="API.Features.Objects.MapEditorObject"/>.
    /// </summary>
    /// <param name="ev">The <see cref="SelectingObjectEventArgs"/> instance.</param>
    internal static void OnSelectingObject(SelectingObjectEventArgs ev) => SelectingObject?.Invoke(ev);

    /// <summary>
    /// Called before copying a <see cref="API.Features.Objects.MapEditorObject"/>.
    /// </summary>
    /// <param name="ev">The <see cref="SelectingObjectEventArgs"/> instance.</param>
    internal static void OnCopyingObject(CopyingObjectEventArgs ev) => CopyingObject?.Invoke(ev);

    /// <summary>
    /// Called before changing a <see cref="API.Features.Objects.MapEditorObject.RelativePosition"/>.
    /// </summary>
    /// <param name="ev">The <see cref="ChangingObjectPositionEventArgs"/> instance.</param>
    internal static void OnChangingObjectPosition(ChangingObjectPositionEventArgs ev) => ChangingObjectPosition?.Invoke(ev);

    /// <summary>
    /// Called before changing a <see cref="API.Features.Objects.MapEditorObject.RelativeRotation"/>.
    /// </summary>
    /// <param name="ev">The <see cref="ChangingObjectRotationEventArgs"/> instance.</param>
    internal static void OnChangingObjectRotation(ChangingObjectRotationEventArgs ev) => ChangingObjectRotation?.Invoke(ev);

    /// <summary>
    /// Called before changing a <see cref="Scale"/>.
    /// </summary>
    /// <param name="ev">The <see cref="ChangingObjectScaleEventArgs"/> instance.</param>
    internal static void OnChangingObjectScale(ChangingObjectScaleEventArgs ev) => ChangingObjectScale?.Invoke(ev);

    /// <summary>
    /// Called before grabbing a <see cref="API.Features.Objects.MapEditorObject"/>.
    /// </summary>
    /// <param name="ev">The <see cref="GrabbingObjectEventArgs"/> instance.</param>
    internal static void OnGrabbingObject(GrabbingObjectEventArgs ev) => GrabbingObject?.Invoke(ev);

    /// <summary>
    /// Called before releasing a <see cref="API.Features.Objects.MapEditorObject"/>.
    /// </summary>
    /// <param name="ev">The <see cref="ReleasingObjectEventArgs"/> instance.</param>
    internal static void OnReleasingObject(ReleasingObjectEventArgs ev) => ReleasingObject?.Invoke(ev);

    /// <summary>
    /// Called before bringing a <see cref="API.Features.Objects.MapEditorObject"/>.
    /// </summary>
    /// <param name="ev">The <see cref="BringingObjectEventArgs"/> instance.</param>
    internal static void OnBringingObject(BringingObjectEventArgs ev) => BringingObject?.Invoke(ev);

    /// <summary>
    /// Called before showing a <see cref="API.Features.Objects.MapEditorObject"/>'s hint.
    /// </summary>
    /// <param name="ev">The <see cref="BringingObjectEventArgs"/> instance.</param>
    internal static void OnShowingObjectHint(ShowingObjectHintEventArgs ev) => ShowingObjectHint?.Invoke(ev);
}