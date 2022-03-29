namespace MapEditorReborn.API.Features.Objects
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Toys;
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
                ShootingTargetToy = ShootingTargetToy.Get(shootingTargetObj);

                ShootingTargetToy.MovementSmoothing = 60;
                Base.TargetType = ShootingTargetToy.Type;
                prevType = ShootingTargetToy.Type;

                ForcedRoomType = shootingTargetSerializable.RoomType != RoomType.Unknown ? shootingTargetSerializable.RoomType : FindRoom().Type;
                UpdateObject();

                return this;
            }

            return null;
        }

        /// <summary>s
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public ShootingTargetSerializable Base;

        public ShootingTargetToy ShootingTargetToy { get; private set; }

        /// <inheritdoc cref="MapEditorObject.UpdateObject"/>
        public override void UpdateObject()
        {
            if (prevType != Base.TargetType)
            {
                SpawnedObjects[SpawnedObjects.IndexOf(this)] = ObjectSpawner.SpawnObject<ShootingTargetObject>(Base, transform.position, transform.rotation);
                ShootingTargetToy.Destroy();
                return;
            }

            prevType = Base.TargetType;

            base.UpdateObject();
        }

        private ShootingTargetType prevType;
    }
}
