namespace MapEditorReborn.API
{
    /// <summary>
    /// Component added to spawned IndicatorObject.
    /// </summary>
    public class IndicatorObjectComponent : MapEditorObject
    {
        /// <summary>
        /// Initializes the <see cref="IndicatorObjectComponent"/>.
        /// </summary>
        /// <param name="mapEditorObject">The <see cref="MapEditorObject"/> which this indicator will indicate.</param>
        /// <returns>Instance of this compoment.</returns>
        public IndicatorObjectComponent Init(MapEditorObject mapEditorObject)
        {
            AttachedMapEditorObject = mapEditorObject;

            return this;
        }

        /// <summary>
        /// <see cref="MapEditorObject"/> that is attached to this object.
        /// </summary>
        public MapEditorObject AttachedMapEditorObject;
    }
}
