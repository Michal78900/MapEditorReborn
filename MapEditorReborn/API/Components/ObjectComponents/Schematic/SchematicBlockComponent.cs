namespace MapEditorReborn.API
{
    using System.Collections.Generic;
    using MEC;
    using Mirror;
    using UnityEngine;

    public class SchematicBlockComponent : MapEditorObject
    {
        public SchematicBlockComponent Init(SchematicObjectComponent parentSchematic, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            AttachedSchematic = parentSchematic;

            originalPosition = position;
            originalRotation = rotation;
            originalScale = scale;

            primitive = GetComponent<PrimitiveObjectComponent>();

            return this;
        }

        public SchematicObjectComponent AttachedSchematic;

        public override void UpdateObject()
        {
            if (first)
            {
                prevScale = originalScale;
                NetworkServer.Spawn(gameObject);

                if (primitive != null)
                    Timing.RunCoroutine(UpdateAnimation(primitive.Base.AnimationFrames));

                first = false;
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

            if (prevScale != transform.localScale)
            {
                prevScale = transform.localScale;
                base.UpdateObject();
            }
        }

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

                yield return Timing.WaitForSeconds(frame.Delay);

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

            playingAnimation = false;
            transform.parent = null;
        }

        private PrimitiveObjectComponent primitive;

        private Vector3 originalPosition;
        private Vector3 originalRotation;
        private Vector3 originalScale;

        private Vector3 prevScale;
        private bool first = true;
        private bool playingAnimation = false;
    }
}
