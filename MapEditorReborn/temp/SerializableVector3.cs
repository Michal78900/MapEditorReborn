namespace MapEditorReborn
{
    using System;
    using UnityEngine;

    /// <summary>
    /// This will be deleted later.
    /// </summary>
    [Serializable]
    public class SerializableVector3
    {
        /// <inheritdoc cref="SerializableVector3"/>
        public SerializableVector3()
        {
        }

        /// <inheritdoc cref="SerializableVector3"/>
        public SerializableVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <inheritdoc cref="SerializableVector3"/>
        public static SerializableVector3 Zero => new SerializableVector3(0, 0, 0);

        /// <inheritdoc cref="SerializableVector3"/>
        public static SerializableVector3 One => new SerializableVector3(1, 1, 1);

        public static implicit operator SerializableVector3(Vector3 vector) => new SerializableVector3(vector.x, vector.y, vector.z);

        public static implicit operator Vector3(SerializableVector3 vector) => new Vector3(vector.X, vector.Y, vector.Z);

        /// <inheritdoc cref="SerializableVector3"/>
        public float X { get; set; }

        /// <inheritdoc cref="SerializableVector3"/>
        public float Y { get; set; }

        /// <inheritdoc cref="SerializableVector3"/>
        public float Z { get; set; }
    }
}
