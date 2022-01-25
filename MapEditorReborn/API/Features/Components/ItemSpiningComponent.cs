namespace MapEditorReborn.API.Features.Components
{
    using UnityEngine;

    /// <summary>
    /// Handles rotating a pickup indicator.
    /// </summary>
    public class ItemSpiningComponent : MonoBehaviour
    {
        /// <summary>
        /// The spinning speed.
        /// </summary>
        public float Speed = 100f;

        /// <inheritdoc/>
        private void Update()
        {
            transform.Rotate(Vector3.up, Time.deltaTime * Speed);
        }
    }
}
