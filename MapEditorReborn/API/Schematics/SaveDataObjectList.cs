namespace MapEditorReborn.API
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SaveDataObjectList
    {
        public List<SchematicBlockData> Blocks = new List<SchematicBlockData>();
    }
}
