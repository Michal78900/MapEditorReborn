namespace MapEditorReborn.API
{
    using Exiled.API.Features;

    /// <summary>
    /// Component added to a ShootingTargetObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class ShootingTargetComponent : MapEditorObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShootingTargetComponent"/> class.
        /// </summary>
        /// <param name="shootingTargetObject">The <see cref="ShootingTargetObject"/> to instantiate.</param>
        /// <returns>Instance of this compoment.</returns>
        public ShootingTargetComponent Init(ShootingTargetObject shootingTargetObject)
        {
            Base = shootingTargetObject;
            shootingTarget = ShootingTarget.Get(GetComponent<InventorySystem.Items.Firearms.Utilities.ShootingTarget>());
            Base.TargetType = shootingTarget.Type;

            UpdateObject();

            return this;
        }

        public ShootingTargetObject Base;

        private ShootingTarget shootingTarget;
    }
}
