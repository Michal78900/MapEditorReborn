namespace MapEditorReborn.API
{
    using UnityEngine;

    public class SchematicBlockComponent : MapEditorObject
    {
        public SchematicBlockComponent Init(SchematicObjectComponent parentSchematic, Vector3 position, Vector3 rotation, Vector3 scale, bool requiresRespawning)
        {
            AttachedSchematic = parentSchematic;

            originalPosition = position;
            originalRotation = rotation;
            originalScale = scale;

            this.requiresRespawning = requiresRespawning;

            return this;
        }

        public SchematicObjectComponent AttachedSchematic;

        public override void UpdateObject()
        {
            transform.position = AttachedSchematic.transform.TransformPoint(originalPosition);
            transform.rotation = AttachedSchematic.transform.rotation * Quaternion.Euler(originalRotation);
            transform.localScale = Vector3.Scale(AttachedSchematic.transform.localScale, originalScale);

            if (prevScale != transform.localScale || requiresRespawning)
            {
                prevScale = transform.localScale;
                base.UpdateObject();
            }
        }

        private Vector3 originalPosition;
        private Vector3 originalRotation;
        private Vector3 originalScale;

        private Vector3? prevScale = null;
        private bool requiresRespawning;
    }
}
