// -----------------------------------------------------------------------
// <copyright file="IndicatorObject.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Objects
{
    using System.Collections.Generic;
    using AdminToys;
    using Exiled.API.Features.Toys;
    using MEC;
    using UnityEngine;

    /// <summary>
    /// Component added to spawned IndicatorObject.
    /// </summary>
    public class IndicatorObject : MapEditorObject
    {
        /// <summary>
        /// Initializes the <see cref="IndicatorObject"/>.
        /// </summary>
        /// <param name="mapEditorObject">The <see cref="MapEditorObject"/> which this indicator will indicate.</param>
        /// <returns>Instance of this component.</returns>
        public IndicatorObject Init(MapEditorObject mapEditorObject)
        {
            AttachedMapEditorObject = mapEditorObject;
            mapEditorObject.AttachedIndicator = this;

            if (TryGetComponent(out PrimitiveObjectToy primitive))
            {
                Timing.RunCoroutine(BlinkingIndicator(Primitive.Get(primitive)).CancelWith(gameObject));
                primitive.enabled = true;
            }

            return this;
        }

        /// <summary>
        /// <see cref="MapEditorObject"/> that is attached to this object.
        /// </summary>
        public MapEditorObject AttachedMapEditorObject;

        public override bool IsRotatable => false;

        public override bool IsScalable => false;

        private IEnumerator<float> BlinkingIndicator(Primitive primitive)
        {
            while (true)
            {
                while (primitive.Color.a > 0f)
                {
                    primitive.Color = new Color(primitive.Color.r, primitive.Color.g, primitive.Color.b, primitive.Color.a - 0.1f);
                    yield return Timing.WaitForSeconds(0.1f);
                }

                while (primitive.Color.a < 0.9f)
                {
                    primitive.Color = new Color(primitive.Color.r, primitive.Color.g, primitive.Color.b, primitive.Color.a + 0.1f);
                    yield return Timing.WaitForSeconds(0.1f);
                }
            }
        }
    }
}
