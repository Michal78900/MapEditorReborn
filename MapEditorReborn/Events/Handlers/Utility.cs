// -----------------------------------------------------------------------
// <copyright file="Utility.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Events.Handlers
{
    using EventArgs;
    using Exiled.Events.Features;

    /// <summary>
    /// The utility commands related events.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Invoked before picking up the ToolGun.
        /// </summary>
        public static Event<PickingUpToolGunEventArgs> PickingUpToolGun { get; set; } = new();

        /// <summary>
        /// Invoked before dropping the ToolGun.
        /// </summary>
        public static Event<DroppingToolGunEventArgs> DroppingToolGun { get; set; } = new();

        /// <summary>
        /// Called before picking up the ToolGun.
        /// </summary>
        /// <param name="ev">The <see cref="PickingUpToolGunEventArgs"/> instance.</param>
        internal static void OnPickingUpToolGun(PickingUpToolGunEventArgs ev) => PickingUpToolGun.InvokeSafely(ev);

        /// <summary>
        /// Called before dropping the ToolGun.
        /// </summary>
        /// <param name="ev">The <see cref="DroppingToolGunEventArgs"/> instance.</param>
        internal static void OnDroppingToolGun(DroppingToolGunEventArgs ev) => DroppingToolGun.InvokeSafely(ev);
    }
}
