namespace MapEditorReborn.API
{
    using System.Linq;
    using Exiled.API.Extensions;
    using Exiled.API.Features;

    /// <summary>
    /// Component added to a ShootingTargetObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class ShootingTargetComponent : MapEditorObject
    {
        /// <summary>
        /// Initializes the <see cref="ShootingTargetComponent"/>.
        /// </summary>
        /// <param name="shootingTargetObject">The <see cref="ShootingTargetObject"/> to instantiate.</param>
        /// <returns>Instance of this compoment.</returns>
        public ShootingTargetComponent Init(ShootingTargetObject shootingTargetObject)
        {
            Base = shootingTargetObject;
            shootingTarget = ShootingTarget.Get(GetComponent<InventorySystem.Items.Firearms.Utilities.ShootingTarget>());
            Base.TargetType = shootingTarget.Type;
            prevBase.CopyProperties(Base);

            UpdateObject();

            return this;
        }

        /// <inheritdoc cref="MapEditorObject.UpdateObject"/>
        public override void UpdateObject()
        {
            if (prevBase.TargetType != Base.TargetType)
            {
                Handler.SpawnedObjects[Handler.SpawnedObjects.FindIndex(x => x == this)] = Handler.SpawnShootingTarget(Base, transform.position, transform.rotation);
                Destroy();

                return;
            }

            prevBase.CopyProperties(Base);

            base.UpdateObject();
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public ShootingTargetObject Base;

        private ShootingTarget shootingTarget;
        private ShootingTargetObject prevBase = new ShootingTargetObject();
    }
}
