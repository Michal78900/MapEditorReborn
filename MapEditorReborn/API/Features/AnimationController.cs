namespace MapEditorReborn.API.Features
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using Objects;
    using UnityEngine;

    public class AnimationController
    {
        internal AnimationController(SchematicObject schematic)
        {
            AttachedSchematic = schematic;

            List<Animator> list = new ();
            foreach (GameObject gameObject in schematic.AttachedBlocks)
            {
                if (gameObject.TryGetComponent(out Animator animator))
                    list.Add(animator);
            }

            Animators = list.AsReadOnly();
            Dictionary.Add(schematic, this);
        }

        public SchematicObject AttachedSchematic { get; }

        public IReadOnlyList<Animator> Animators { get; }

        public void Play(string stateName, int animatorIndex = 0) => Animators[animatorIndex].Play(stateName);

        public void Play(string stateName, string animatorName) => Animators.FirstOrDefault(x => x.name == animatorName)?.Play(stateName);

        public void Stop(int animatorIndex = 0) => Animators[animatorIndex].StopPlayback();

        public void Stop(string animatorName) => Animators.FirstOrDefault(x => x.name == animatorName)?.StopPlayback();

        public static AnimationController Get(SchematicObject schematic)
        {
            if (schematic == null || !schematic.IsBuilt)
                return null;

            return Dictionary.TryGetValue(schematic, out AnimationController animationController) ? animationController : new AnimationController(schematic);
        }

        internal static readonly Dictionary<SchematicObject, AnimationController> Dictionary = new Dictionary<SchematicObject, AnimationController>();
    }
}
