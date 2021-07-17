namespace MapEditorReborn.API
{
    using UnityEngine;

    public class SpiningComponent : MonoBehaviour
    {
        public float Speed = 100f;

        /// <inheritdoc/>
        private void Update()
        {
            transform.Rotate(Vector3.up, Time.deltaTime * Speed);
        }
    }
}
