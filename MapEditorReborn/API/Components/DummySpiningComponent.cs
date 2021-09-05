namespace MapEditorReborn.API
{
    using UnityEngine;

    /// <summary>
    /// Handles rotating a dummy indicator.
    /// </summary>
    public class DummySpiningComponent : MapEditorObject
    {
        /// <summary>
        /// The spinning speed.
        /// </summary>
        public float Speed = 3f;

        /// <summary>
        /// The <see cref="ReferenceHub"/> of the dummy object.
        /// </summary>
        public ReferenceHub Hub;

        private float i;

        private void Update()
        {
            Hub.playerMovementSync.RotationSync = new Vector2(0, i);

            i += Speed;
            if (i > 360)
                i = 0;
        }
    }
}
