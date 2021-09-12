namespace MapEditorReborn.API
{
    using UnityEditor;

    /// <summary>
    /// Component added to spawned IndicatorObject.
    /// </summary>
    public class IndicatorObjectComponent : MapEditorObject
    {
        /// <summary>
        /// <see cref="MapEditorObject"/> that is attached to this object.
        /// </summary>
        public MapEditorObject AttachedMapEditorObject;

        /// <summary>
        /// Initializes the <see cref="IndicatorObjectComponent"/>.
        /// </summary>
        /// <param name="mapEditorObject">The <see cref="MapEditorObject"/> which this indicator will indicate.</param>
        public void Init(MapEditorObject mapEditorObject)
        {
            AttachedMapEditorObject = mapEditorObject;
        }
    }
}
