namespace MapEditorReborn.API
{
    using UnityEngine;

    public class ObjectRotationComponent : MonoBehaviour
    {
        public bool XisRandom = false;

        public bool YisRandom = false;

        public bool ZisRandom = false;

        public void Init(Vector3 initialRotation)
        {
            if (initialRotation.x == -1f)
                XisRandom = true;

            if (initialRotation.y == -1f)
                YisRandom = true;

            if (initialRotation.z == -1f)
                ZisRandom = true;
        }
    }
}
