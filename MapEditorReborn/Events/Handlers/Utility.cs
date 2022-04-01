namespace MapEditorReborn.Events.Handlers
{
    using EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// The utility commands related events.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Invoked before picking up the ToolGun.
        /// </summary>
        public static event CustomEventHandler<PickingUpToolGunEventArgs> PickingUpToolGun;

        /// <summary>
        /// Invoked before dropping the ToolGun.
        /// </summary>
        public static event CustomEventHandler<DroppingToolGunEventArgs> DroppingToolGun;

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
