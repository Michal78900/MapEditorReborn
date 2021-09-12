namespace MapEditorReborn.API
{
    using Exiled.API.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// Component added to a ShootingTargetObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class ShootingTargetComponent : MapEditorObject
    {
        /// <summary>
        /// Gets <see cref="ShootingTargetType"/> of the <see cref="ShootingTargets"/>.
        /// </summary>
        public ShootingTargetType TargetType => shootingTarget.Type;

        private void Awake() => shootingTarget = ShootingTarget.Get(gameObject.GetComponent<InventorySystem.Items.Firearms.Utilities.ShootingTarget>());

        private ShootingTarget shootingTarget;
    }
}
