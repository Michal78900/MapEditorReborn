namespace MapEditorReborn.Events.Handlers
{
    using EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    /// <summary>
    /// Schematic related events.
    /// </summary>
    public static class Schematic
    {
        /// <summary>
        /// Invoked before ending a schematic animation.
        /// </summary>
        public static event CustomEventHandler<EndingSchematicAnimationEventArgs> EndingSchematicAnimation;

        /// <summary>
        /// Called before ending a schematic animation.
        /// </summary>
        /// <param name="ev">The <see cref="EndingSchematicAnimationEventArgs"/> instance.</param>
        internal static void OnEndingSchematicAnimation(EndingSchematicAnimationEventArgs ev) => EndingSchematicAnimation.InvokeSafely(ev);
    }
}
