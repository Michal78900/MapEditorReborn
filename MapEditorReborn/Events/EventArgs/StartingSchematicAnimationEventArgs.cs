namespace MapEditorReborn.Events.EventArgs
{
    using System;
    using API.Enums;
    using API.Features.Components.ObjectComponents;

    /// <summary>
    /// Contains all information before starting a schematic animation.
    /// </summary>
    public class StartingSchematicAnimationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartingSchematicAnimationEventArgs"/> class.
        /// </summary>
        /// <param name="schematic"><inheritdoc cref="Schematic"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public StartingSchematicAnimationEventArgs(SchematicObjectComponent schematic, bool isAllowed = true)
        {
            Schematic = schematic;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="SchematicObjectComponent"/> which is starting the animation.
        /// </summary>
        public SchematicObjectComponent Schematic { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the animation can start.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
