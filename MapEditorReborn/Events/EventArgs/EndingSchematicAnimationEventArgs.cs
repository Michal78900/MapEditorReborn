namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Enums;
    using API.Features.Components.ObjectComponents;

    public class EndingSchematicAnimationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndingSchematicAnimationEventArgs"/> class.
        /// </summary>
        /// <param name="schematic"></param>
        /// <param name="animationEndAction"></param>
        public EndingSchematicAnimationEventArgs(SchematicObjectComponent schematic, AnimationEndAction animationEndAction)
        {
            Schematic = schematic;
            AnimationEndAction = animationEndAction;
        }

        public SchematicObjectComponent Schematic { get; }

        public AnimationEndAction AnimationEndAction { get; set; }
    }
}
