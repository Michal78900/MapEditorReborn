namespace MapEditorReborn.API.Features.Serializable
{
    using System.Collections.Generic;
    using Enums;

    public class LockerSerializable : SerializableObject
    {
        public LockerType LockerType { get; set; }

        public List<LockerChamberSerializable> Chambers { get; set; } = new List<LockerChamberSerializable>()
        {
            new LockerChamberSerializable(),
        };
    }
}
