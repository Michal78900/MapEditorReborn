namespace MapEditorReborn.API.Features.Objects
{
    /// <summary>
    /// Component added to spawned IndicatorObject.
    /// </summary>
    public class IndicatorObject : MapEditorObject
    {
        /// <summary>
        /// Initializes the <see cref="IndicatorObject"/>.
        /// </summary>
        /// <param name="mapEditorObject">The <see cref="MapEditorObject"/> which this indicator will indicate.</param>
        /// <returns>Instance of this compoment.</returns>
        public IndicatorObject Init(MapEditorObject mapEditorObject)
        {
            AttachedMapEditorObject = mapEditorObject;
            mapEditorObject.AttachedIndicator = this;

            return this;
        }

        /// <summary>
        /// <see cref="MapEditorObject"/> that is attached to this object.
        /// </summary>
        public MapEditorObject AttachedMapEditorObject;

        public override bool IsRotatable => false;

        public override bool IsScalable => false;
    }
}
