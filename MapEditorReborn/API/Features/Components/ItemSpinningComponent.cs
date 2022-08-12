// -----------------------------------------------------------------------
// <copyright file="ItemSpinningComponent.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Components
{
    using UnityEngine;

    /// <summary>
    /// Handles rotating a pickup indicator.
    /// </summary>
    public class ItemSpinningComponent : MonoBehaviour
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
