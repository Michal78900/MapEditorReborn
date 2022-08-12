﻿// -----------------------------------------------------------------------
// <copyright file="ObjectRotationComponent.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using UnityEngine;

namespace MapEditorReborn.API.Features.Components
{
    /// <summary>
    /// Used for tracking objects with random rotation.
    /// </summary>
    public class ObjectRotationComponent : MonoBehaviour
    {
        /// <summary>
        /// If the X axis of the object is random.
        /// </summary>
        public bool XisRandom;

        /// <summary>
        /// If the Y axis of the object is random.
        /// </summary>
        public bool YisRandom;

        /// <summary>
        /// If the Z axis of the object is random.
        /// </summary>
        public bool ZisRandom;

        /// <summary>
        /// Initializes the <see cref="ObjectRotationComponent"/>.
        /// </summary>
        /// <param name="initialRotation">The initial object rotation.</param>
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
