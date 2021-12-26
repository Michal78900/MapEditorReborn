namespace MapEditorReborn.API
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Extensions;

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
            shootingTarget = ShootingTarget.Get(GetComponent<AdminToys.ShootingTarget>());
            shootingTarget.Base.NetworkMovementSmoothing = 60;
            Base.TargetType = shootingTarget.Type;
            prevBase.CopyProperties(Base);

            ForcedRoomType = shootingTargetObject.RoomType != RoomType.Unknown ? shootingTargetObject.RoomType : FindRoom().Type;
            UpdateObject();

            return this;
        }

        /// <inheritdoc cref="MapEditorObject.UpdateObject"/>
        public override void UpdateObject()
        {
            if (prevBase.TargetType != Base.TargetType)
            {
                Methods.SpawnedObjects[Methods.SpawnedObjects.FindIndex(x => x == this)] = Methods.SpawnShootingTarget(Base, transform.position, transform.rotation);
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
