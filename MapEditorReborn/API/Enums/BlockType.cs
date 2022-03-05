namespace MapEditorReborn.API.Enums
{
    /// <summary>
    /// All available blocks for schematic object.
    /// </summary>
    public enum BlockType
    {
        /// <summary>
        /// Represents an empty transform.
        /// </summary>
        Empty = 0,

        /// <summary>
        /// Represents a primitve.
        /// </summary>
        Primitive = 1,

        /// <summary>
        /// Represents a light.
        /// </summary>
        Light = 2,

        /// <summary>
        /// Represents a pickup.
        /// </summary>
        Pickup = 3,

        /// <summary>
        /// Represents a workstation.
        /// </summary>
        Workstation = 4,

        /// <summary>
        /// Represents a sub-schematic.
        /// </summary>
        Schematic = 5,
    }
}
