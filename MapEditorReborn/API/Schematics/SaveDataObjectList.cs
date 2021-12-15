namespace MapEditorReborn.API
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class SaveDataObjectList
    {
        public List<PrimitiveObject> Primitives = new List<PrimitiveObject>();
        public List<LightSourceObject> LightSources = new List<LightSourceObject>();
        public List<ItemSpawnPointObject> Items = new List<ItemSpawnPointObject>();
        public List<WorkStationObject> WorkStations = new List<WorkStationObject>();

        public List<AnimationFrame> ParentAnimationFrames = new List<AnimationFrame>();
    }
}
