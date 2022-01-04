namespace MapEditorReborn.API.Features.Objects.Schematics
{
    using System;
    using System.Collections.Generic;
    using Enums;

    /// <summary>
    /// Saved data of the schematic. It is used for building and animating the schematic in game.
    /// </summary>
    [Serializable]
    public class SaveDataObjectList
    {
        /// <summary>
        /// List of <see cref="PrimitiveObject"/>s.
        /// </summary>
        public List<PrimitiveObject> Primitives = new List<PrimitiveObject>();

        /// <summary>
        /// List of <see cref="LightSourceObject"/>.
        /// </summary>
        public List<LightSourceObject> LightSources = new List<LightSourceObject>();

        /// <summary>
        /// List of <see cref="ItemSpawnPointObject"/>.
        /// </summary>
        public List<ItemSpawnPointObject> Items = new List<ItemSpawnPointObject>();

        /// <summary>
        /// List of <see cref="WorkStationObject"/>.
        /// </summary>
        public List<WorkStationObject> WorkStations = new List<WorkStationObject>();

        /// <summary>
        /// List of <see cref="AnimationFrame"/>s.
        /// </summary>
        public List<AnimationFrame> ParentAnimationFrames = new List<AnimationFrame>();

        /// <summary>
        /// Set <see cref="Enums.AnimationEndAction"/>.
        /// </summary>
        public AnimationEndAction AnimationEndAction;
    }
}
