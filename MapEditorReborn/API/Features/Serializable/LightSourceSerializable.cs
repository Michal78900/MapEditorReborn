namespace MapEditorReborn.API.Features.Serializable
{
    using System;
    using UnityEngine;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A tool used to easily handle light sources.
    /// </summary>
    [Serializable]
    public class LightSourceSerializable : SerializableObject
    {
        public LightSourceSerializable()
        {
        }

        public LightSourceSerializable(string color, float intensity, float range, bool shadows)
        {
            Color = color;
            Intensity = intensity;
            Range = range;
            Shadows = shadows;
        }

        /// <summary>
        /// Gets or sets the <see cref="LightSourceSerializable"/>'s color.
        /// </summary>
        public string Color { get; set; } = "white";

        /// <summary>
        /// Gets or sets the <see cref="LightSourceSerializable"/>'s intensity.
        /// </summary>
        public float Intensity { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the <see cref="LightSourceSerializable"/>'s range.
        /// </summary>
        public float Range { get; set; } = 1f;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="LightSourceSerializable"/>'s shadows are enabled.
        /// </summary>
        public bool Shadows { get; set; } = true;

        [YamlIgnore]
        public override Vector3 Rotation { get; set; }

        [YamlIgnore]
        public override Vector3 Scale { get; set; }
    }
}
