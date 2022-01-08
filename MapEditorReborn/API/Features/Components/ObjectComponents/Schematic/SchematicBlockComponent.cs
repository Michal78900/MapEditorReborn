namespace MapEditorReborn.API.Features.Components.ObjectComponents
{
    using AdminToys;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// The component to be used with <see cref="SchematicObjectComponent"/>.
    /// </summary>
    public class SchematicBlockComponent : MapEditorObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchematicObjectComponent"/> class.
        /// </summary>
        /// <param name="parentSchematic">The requried <see cref="SchematicObjectComponent"/>.</param>
        /// <param name="position">The specified position.</param>
        /// <param name="rotation">The specified rotation.</param>
        /// <param name="scale">The specified scale.</param>
        /// <returns>The initialized <see cref="SchematicObjectComponent"/> instance.</returns>
        public SchematicBlockComponent Init(SchematicObjectComponent parentSchematic, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            AttachedSchematic = parentSchematic;

            OriginalPosition = position;
            OriginalRotation = rotation;
            OriginalScale = scale;

            if (TryGetComponent(out AdminToyBase adminToyBase))
            {
                AdminToyBase = adminToyBase;

                if (TryGetComponent(out PrimitiveObjectComponent primitiveObjectComponent))
                    primitive = primitiveObjectComponent;
            }

            if (name.Contains("Door") || name.Contains("Workstation"))
                requiresRespawning = true;

            return this;
        }

        /// <summary>
        /// The attached schematic.
        /// </summary>
        public SchematicObjectComponent AttachedSchematic;

        /// <summary>
        /// Gets the <see cref="AdminToys.AdminToyBase"/>. May be null.
        /// </summary>
        public AdminToyBase AdminToyBase { get; private set; }

        /// <summary>
        /// Gets the original position of this block.
        /// </summary>
        public Vector3 OriginalPosition { get; private set; }

        /// <summary>
        /// Gets the original rotation of this block.
        /// </summary>
        public Vector3 OriginalRotation { get; private set; }

        /// <summary>
        /// Gets the original scale of this block.
        /// </summary>
        public Vector3 OriginalScale { get; private set; }

        /// <inheritdoc cref="MapEditorObject.UpdateObject"/>
        public override void UpdateObject()
        {
            if (prevScale == Vector3.zero)
            {
                prevScale = OriginalScale;
                NetworkServer.Spawn(gameObject);
            }

            if (primitive != null)
            {
                primitive.UpdateObject();
                return;
            }

            if (prevScale != transform.localScale || requiresRespawning)
            {
                prevScale = transform.localScale;
                base.UpdateObject();
            }
        }

        private Vector3 prevScale = Vector3.zero;

        private bool requiresRespawning = false;

        private PrimitiveObjectComponent primitive;
    }
}
