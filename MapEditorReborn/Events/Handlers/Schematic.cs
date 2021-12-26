namespace MapEditorReborn.Events.Handlers
{
    using EventArgs;
    using Exiled.Events.Extensions;

    using static Exiled.Events.Events;

    public static class Schematic
    {
        public static event CustomEventHandler<EndingSchematicAnimationEventArgs> EndingSchematicAnimation;

        public static void OnEndingSchematicAnimation(EndingSchematicAnimationEventArgs ev) => EndingSchematicAnimation.InvokeSafely(ev);
    }
}
