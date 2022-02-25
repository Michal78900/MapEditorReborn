namespace MapEditorReborn.API.Features
{
    using System.Collections.Generic;
    using Components.ObjectComponents;
    using UnityEngine;

    public class AnimationController
    {
        internal AnimationController(SchematicObjectComponent schematic)
        {
            AttachedSchematic = schematic;

            List<Animator> list = new List<Animator>();
            foreach (GameObject gameObject in schematic.AttachedBlocks)
            {
                if (gameObject.TryGetComponent(out Animator animator))
                    list.Add(animator);
            }

            Animators = list.AsReadOnly();
            Dictionary.Add(schematic, this);
        }

        public SchematicObjectComponent AttachedSchematic { get; }

        public IReadOnlyList<Animator> Animators { get; }

        public void Play(string stateName) => Animators[0].Play(stateName);

        public void Play(string stateName, int animatorIndex) => Animators[animatorIndex].Play(stateName, animatorIndex);

        public void Stop() => Animators[0].StopPlayback();

        public void Stop(int animatorIndex) => Animators[animatorIndex].StopPlayback();

        public static AnimationController Get(SchematicObjectComponent schematic)
        {
            if (schematic == null || !schematic.IsBuilt)
                return null;

            return Dictionary.TryGetValue(schematic, out AnimationController animationController) ? animationController : new AnimationController(schematic);
        }

        internal static readonly Dictionary<SchematicObjectComponent, AnimationController> Dictionary = new Dictionary<SchematicObjectComponent, AnimationController>();
    }
}
