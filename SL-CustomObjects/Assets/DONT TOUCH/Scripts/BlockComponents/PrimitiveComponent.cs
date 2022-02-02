using System;
using UnityEditor;
using UnityEngine;

public class PrimitiveComponent : SchematicBlock
{
    public Color Color;
    public bool Collidable;

    public override BlockType BlockType => BlockType.Primitive;

    private void Awake()
    {
        if (Collidable)
            gameObject.AddComponent<MeshCollider>().convex = true;
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (EditorUtility.IsPersistent(gameObject))
            return;
#endif

        if (_renderer == null && !TryGetComponent(out _renderer))
            return;

        if (_sharedRegular == null)
            _sharedRegular = new Material((Material)Resources.Load("Materials/Regular"));

        if (_sharedTransparent == null)
            _sharedTransparent = new Material((Material)Resources.Load("Materials/Transparent"));

        _renderer.sharedMaterial = Color.a >= 1f ? _sharedRegular : _sharedTransparent;
        _renderer.sharedMaterial.color = Color;
    }

    private MeshRenderer _renderer;
    private Material _sharedRegular;
    private Material _sharedTransparent;
}
