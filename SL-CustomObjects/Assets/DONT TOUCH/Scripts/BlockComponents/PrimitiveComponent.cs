using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimitiveComponent : MonoBehaviour
{
    public Color Color;
    public bool Collidable;
    public PrimitiveType Type { get; private set; }

    public void Awake()
    {
        Type = (PrimitiveType)Enum.Parse(typeof(PrimitiveType), tag);

        GetComponent<Renderer>().material.color = Color;

        if (!Collidable)
            transform.localScale *= -1;
    }
}
