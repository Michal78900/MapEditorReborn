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
        public static event CustomEventHandler<SchematicSpawnedEventArgs> SchematicSpawned;

        internal static void OnSchematicSpawned(SchematicSpawnedEventArgs ev) => SchematicSpawned.InvokeSafely(ev);

        /*
        /// <summary>
        /// Invoked before starting a schematic animation.
        /// </summary>
        public static event CustomEventHandler<StartingSchematicAnimationEventArgs> StartingSchematicAnimation;

        /// <summary>
        /// Invoked before ending a schematic animation.
        /// </summary>
        public static event CustomEventHandler<EndingSchematicAnimationEventArgs> EndingSchematicAnimation;

        /// <summary>
        /// Called before starting a schematic animation.
        /// </summary>
        /// <param name="ev">The <see cref="StartingSchematicAnimationEventArgs"/> instance.</param>
        internal static void OnStartingSchematicAnimation(StartingSchematicAnimationEventArgs ev) => StartingSchematicAnimation.InvokeSafely(ev);

        /// <summary>
        /// Called before ending a schematic animation.
        /// </summary>
        /// <param name="ev">The <see cref="EndingSchematicAnimationEventArgs"/> instance.</param>
        internal static void OnEndingSchematicAnimation(EndingSchematicAnimationEventArgs ev) => EndingSchematicAnimation.InvokeSafely(ev);
        */
    }
}
