namespace MapEditorReborn.API.Features.Objects
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Toys;
    using Extensions;
    using Features.Serializable;

    using static API;

    /// <summary>
    /// Component added to a ShootingTargetObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class ShootingTargetObject : MapEditorObject
    {
        /// <summary>
        /// Initializes the <see cref="ShootingTargetObject"/>.
        /// </summary>
        /// <param name="shootingTargetSerializable">The <see cref="ShootingTargetSerializable"/> to instantiate.</param>
        /// <returns>Instance of this compoment.</returns>
        public ShootingTargetObject Init(ShootingTargetSerializable shootingTargetSerializable)
        {
            Base = shootingTargetSerializable;

            if (TryGetComponent(out AdminToys.ShootingTarget shootingTargetObj))
            {
                shootingTargetToy = ShootingTargetToy.Get(shootingTargetObj);

                shootingTargetToy.MovementSmoothing = 60;
                Base.TargetType = shootingTargetToy.Type;
                prevBase.CopyProperties(Base);

                ForcedRoomType = shootingTargetSerializable.RoomType != RoomType.Unknown ? shootingTargetSerializable.RoomType : FindRoom().Type;
                UpdateObject();

                return this;
            }

            return null;
        }

        /// <inheritdoc cref="MapEditorObject.UpdateObject"/>
        public override void UpdateObject()
        {
            if (prevBase.TargetType != Base.TargetType)
            {
                SpawnedObjects[SpawnedObjects.IndexOf(this)] = ObjectSpawner.SpawnShootingTarget(Base, transform.position, transform.rotation);
                shootingTargetToy.Destroy();
                return;
            }

            prevBase.CopyProperties(Base);

            base.UpdateObject();
        }

        /// <summary>s
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public ShootingTargetSerializable Base;

        private ShootingTargetToy shootingTargetToy;
        private ShootingTargetSerializable prevBase = new ShootingTargetSerializable();
    }
}
