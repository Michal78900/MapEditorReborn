namespace MapEditorReborn.API.Enums
{
    using System;

    [Flags]
    public enum LockOnEvent
    {
        None = 0,
        LightDecontaminated = 1,
        WarheadDetonated = 2,
    }
}
