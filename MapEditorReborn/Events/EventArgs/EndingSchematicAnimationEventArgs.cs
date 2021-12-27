namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Enums;
    using API.Features.Components.ObjectComponents;

    /// <summary>
    /// Contains all information before ending a schematic animation.
    /// </summary>
    public class EndingSchematicAnimationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndingSchematicAnimationEventArgs"/> class.
        /// </summary>
        /// <param name="schematic"><inheritdoc cref="Schematic"/></param>
        /// <param name="animationEndAction"><inheritdoc cref="AnimationEndAction"/></param>
        public EndingSchematicAnimationEventArgs(SchematicObjectComponent schematic, AnimationEndAction animationEndAction)
        {
            Schematic = schematic;
            AnimationEndAction = animationEndAction;
        }

        /// <summary>
        /// Gets the <see cref="SchematicObjectComponent"/> which is ending the animation.
        /// </summary>
        public SchematicObjectComponent Schematic { get; }

        /// <summary>
        /// Gets or sets the <see cref="API.Enums.AnimationEndAction"/>.
        /// </summary>
        public AnimationEndAction AnimationEndAction { get; set; }
    }
}
