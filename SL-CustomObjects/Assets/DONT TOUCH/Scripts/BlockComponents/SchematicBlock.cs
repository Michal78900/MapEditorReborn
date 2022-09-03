using UnityEngine;

public abstract class SchematicBlock : MonoBehaviour
{
    public abstract BlockType BlockType { get; }

    public virtual bool Compile(SchematicBlockData block, Schematic schematic) => true;
}
