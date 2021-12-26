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
        public static event CustomEventHandler<EndingSchematicAnimationEventArgs> EndingSchematicAnimation;

        internal static void OnEndingSchematicAnimation(EndingSchematicAnimationEventArgs ev) => EndingSchematicAnimation.InvokeSafely(ev);
    }
}
