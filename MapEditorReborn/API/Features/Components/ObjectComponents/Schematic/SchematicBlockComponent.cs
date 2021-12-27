﻿namespace MapEditorReborn.API.Features.Components.ObjectComponents
{
    using System.Collections.Generic;
    using Enums;
    using Features.Objects.Schematics;
    using MEC;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// The component to be used with <see cref="SchematicObjectComponent"/>.
    /// </summary>
    public class SchematicBlockComponent : MapEditorObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchematicObjectComponent"/> class.
        /// </summary>
        /// <param name="parentSchematic">The requried <see cref="SchematicObjectComponent"/>.</param>
        /// <param name="position">The specified position.</param>
        /// <param name="rotation">The specified rotation.</param>
        /// <param name="scale">The specified scale.</param>
        /// <returns>The initialized <see cref="SchematicObjectComponent"/> instance.</returns>
        public SchematicBlockComponent Init(SchematicObjectComponent parentSchematic, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            AttachedSchematic = parentSchematic;

            originalPosition = position;
            originalRotation = rotation;
            originalScale = scale;

            if (TryGetComponent(out PrimitiveObjectComponent primitiveObjectComponent))
                primitive = primitiveObjectComponent;

            if (name.Contains("Door") || name.Contains("Workstation"))
                requiresRespawning = true;

            return this;
        }

        /// <summary>
        /// The attached schematic.
        /// </summary>
        public SchematicObjectComponent AttachedSchematic;

        /// <inheritdoc cref="MapEditorObject.UpdateObject"/>
        public override void UpdateObject()
        {
            if (prevScale == Vector3.zero)
            {
                prevScale = originalScale;
                NetworkServer.Spawn(gameObject);

                if (primitive != null)
                    Timing.RunCoroutine(UpdateAnimation(primitive.Base.AnimationFrames));
            }

            if (!playingAnimation)
            {
                transform.position = AttachedSchematic.transform.TransformPoint(originalPosition);
                transform.rotation = AttachedSchematic.transform.rotation * Quaternion.Euler(originalRotation);
                transform.localScale = Vector3.Scale(AttachedSchematic.transform.localScale, originalScale);
            }

            if (primitive != null)
            {
                primitive.UpdateObject();
                return;
            }

            if (prevScale != transform.localScale || requiresRespawning)
            {
                prevScale = transform.localScale;
                base.UpdateObject();
            }
        }

        /// <summary>
        /// Plays one frame.
        /// </summary>
        public void PlayOneFrame() => playOneFrame = true;

        private IEnumerator<float> UpdateAnimation(List<AnimationFrame> frames)
        {
            if (frames.Count == 0)
                yield break;

            playingAnimation = true;
            transform.parent = AttachedSchematic.transform;

            foreach (AnimationFrame frame in frames)
            {
                Vector3 remainingPosition = frame.PositionAdded;
                Vector3 remainingRotation = frame.RotationAdded;
                Vector3 deltaPosition = remainingPosition / Mathf.Abs(frame.PositionRate);
                Vector3 deltaRotation = remainingRotation / Mathf.Abs(frame.RotationRate);

                if (frame.Delay >= 0f)
                {
                    yield return Timing.WaitForSeconds(frame.Delay);
                }
                else
                {
                    yield return Timing.WaitUntilTrue(() => playOneFrame);
                }

                playOneFrame = false;

                while (true)
                {
                    if (remainingPosition != Vector3.zero)
                    {
                        transform.position += deltaPosition;
                        remainingPosition -= deltaPosition;
                    }

                    if (remainingRotation != Vector3.zero)
                    {
                        transform.rotation *= Quaternion.Euler(deltaRotation);
                        remainingRotation -= deltaRotation;
                    }

                    primitive.UpdateObject();

                    if (remainingPosition.sqrMagnitude <= 1 && remainingRotation.sqrMagnitude <= 1)
                        break;

                    yield return Timing.WaitForSeconds(frame.FrameLength);
                }
            }

            if (primitive.Base.AnimationEndAction == AnimationEndAction.Destroy)
            {
                Destroy();
            }
            else if (primitive.Base.AnimationEndAction == AnimationEndAction.Loop)
            {
                Timing.RunCoroutine(UpdateAnimation(frames));
                yield break;
            }

            playingAnimation = false;
            transform.parent = null;
        }

        private PrimitiveObjectComponent primitive;

        private Vector3 originalPosition;
        private Vector3 originalRotation;
        private Vector3 originalScale;
        private Vector3 prevScale = Vector3.zero;
        private bool requiresRespawning = false;

        private bool playingAnimation = false;
        private bool playOneFrame = false;
    }
}