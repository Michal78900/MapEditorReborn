namespace MapEditorReborn.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Objects;
    using UnityEngine;

    public class AnimationController : IDisposable
    {
        internal AnimationController(SchematicObject schematic)
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

        public SchematicObject AttachedSchematic { get; private set; }

        public IReadOnlyList<Animator> Animators { get; private set; }

        public void Play(string stateName, int animatorIndex = 0) => Animators[animatorIndex].Play(stateName, animatorIndex);

        public void Stop(int animatorIndex = 0) => Animators[animatorIndex].StopPlayback();

        public static AnimationController Get(SchematicObject schematic)
        {
            if (schematic == null || !schematic.IsBuilt)
                return null;

            return Dictionary.TryGetValue(schematic, out AnimationController animationController) ? animationController : new AnimationController(schematic);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    AttachedSchematic = null;
                    Animators = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dictionary.Remove(Dictionary.First(x => x.Value == this).Key);
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool disposedValue;

        internal static readonly Dictionary<SchematicObject, AnimationController> Dictionary = new Dictionary<SchematicObject, AnimationController>();
    }
}
