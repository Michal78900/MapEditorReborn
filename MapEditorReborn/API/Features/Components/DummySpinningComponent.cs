// -----------------------------------------------------------------------
// <copyright file="DummySpinningComponent.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Components
{
    using PlayerRoles.FirstPersonControl;
    using UnityEngine;

    /// <summary>
    /// Handles rotating a dummy indicator.
    /// </summary>
    public class DummySpinningComponent : MonoBehaviour
    {
        /// <summary>
        /// Initializes the <see cref="DummySpinningComponent"/>.
        /// </summary>
        /// <param name="referenceHub">The <see cref="ReferenceHub"/> of the dummy.</param>
        /// <param name="speed">The rotation speed.</param>
        /// <returns>Instance of this component.</returns>
        public DummySpinningComponent Init(ReferenceHub referenceHub, float speed = 3f)
        {
            hub = referenceHub;
            Speed = speed;

            return this;
        }

        /// <summary>
        /// The spinning speed.
        /// </summary>
        public float Speed = 3f;

        private void Update()
        {
            hub.TryOverridePosition(hub.transform.position, Vector3.up * i);
            //hub.playerMovementSync.RotationSync = new Vector2(0, i);

            i += Speed;
            if (i > 360)
                i = 0;
        }

        private ReferenceHub hub;

        private float i;
    }
}