using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveDataObjectList
{
    public SaveDataObjectList()
    {
    }

    public List<PrimitiveObject> Primitives { get; set; } = new List<PrimitiveObject>();
    public List<LightSourceObject> LightSources { get; set; } = new List<LightSourceObject>();
    public List<ItemObject> Items { get; set; } = new List<ItemObject>();
    public List<WorkStationObject> WorkStations { get; set; } = new List<WorkStationObject>();

    public List<SerializableAnimationFrame> ParentAnimationFrames { get; set; }
    public AnimationEndAction AnimationEndAction { get; set; }
}

public class SerializableVector3
{
    public SerializableVector3()
    {
    }

    public SerializableVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public float x { get; set; }

    public float y { get; set; }

    public float z { get; set; }

    public static implicit operator SerializableVector3(Vector3 vector) => new SerializableVector3(vector.x, vector.y, vector.z);

    public static implicit operator Vector3(SerializableVector3 vector) => new Vector3(vector.x, vector.y, vector.z);
}
