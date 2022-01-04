using System;
using System.Collections.Generic;
using UnityEngine;

public class PrimitiveComponent : MonoBehaviour
{
    public Color Color;
    public bool Collidable;
    public List<AnimationFrame> AnimationFrames = new List<AnimationFrame>();
    public AnimationEndAction AnimationEndAction;

    public PrimitiveType Type { get; private set; }

    private void Awake()
    {
        Type = (PrimitiveType)Enum.Parse(typeof(PrimitiveType), tag);

        if (!Collidable)
            transform.localScale *= -1;

        if (AnimationFrames.Count > 0)
        {
            StartCoroutine(UpdateAnimation());
        }
        else
        {
            AnimationFrames = null;
        }
    }

    private IEnumerator<YieldInstruction> UpdateAnimation()
    {
        foreach (AnimationFrame frame in AnimationFrames)
        {
            Vector3 remainingPosition = frame.PositionAdded;
            Vector3 remainingRotation = frame.RotationAdded;
            Vector3 deltaPosition = remainingPosition / Mathf.Abs(frame.PositionRate);
            Vector3 deltaRotation = remainingRotation / Mathf.Abs(frame.RotationRate);

            yield return new WaitForSeconds(frame.Delay);

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

                if (remainingPosition.sqrMagnitude <= 1 && remainingRotation.sqrMagnitude <= 1)
                    break;

                yield return new WaitForSeconds(frame.FrameLength);
            }
        }

        if (AnimationEndAction == AnimationEndAction.Destroy)
        {
            Destroy(this);
        }
        else if (AnimationEndAction == AnimationEndAction.Loop)
        {
            StartCoroutine(UpdateAnimation());
        }
    }

    private void OnValidate() => GetComponent<Renderer>().material.color = Color;
}
