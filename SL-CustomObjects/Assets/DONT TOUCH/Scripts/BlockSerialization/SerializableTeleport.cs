using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SerializableTeleport : SchematicBlockData
{
    public SerializableTeleport()
    {
    }
    
    public SerializableTeleport(SchematicBlockData block)
    {
        this.Name = block.Name;
        this.ObjectId = block.ObjectId;
        this.ParentId = block.ParentId;
        this.Position = block.Position;
        this.Rotation = block.Rotation;
        this.Scale = block.Scale;
    }

    public RoomType RoomType { get; set; }
    
    public List<TargetTeleporter> TargetTeleporters { get; set; }

    public List<string> AllowedRoles { get; set; }

    public float Cooldown { get; set; }

    public TeleportFlags TeleportFlags { get; set; }

    public LockOnEvent LockOnEvent { get; set; }

    public int TeleportSoundId { get; set; }

    public float? PlayerRotationX { get; set; } = null;

    public float? PlayerRotationY { get; set; } = null;

    [JsonIgnore]
    public override BlockType BlockType { get; set; }

    [JsonIgnore]
    public override string AnimatorName { get; set; }

    [JsonIgnore]
    public override Dictionary<string, object> Properties { get; set; }
}
