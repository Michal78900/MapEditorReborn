namespace MapEditorReborn.API
{
    using UnityEngine;

    /// <summary>
    /// Component added to a spawned PlayerSpawnPoint object. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class PlayerSpawnPointComponent : MapEditorObject
    {
        /// <summary>
        /// Initializes the <see cref="PlayerSpawnPointComponent"/>.
        /// </summary>
        /// <param name="playerSpawnPointObject">The <see cref="PlayerSpawnPointObject"/> used for instantiating the object.</param>
        /// <returns>Instance of this compoment.</returns>
        public PlayerSpawnPointComponent Init(PlayerSpawnPointObject playerSpawnPointObject)
        {
            Base = playerSpawnPointObject;

            UpdateObject();

            return this;
        }

        /// <summary>
        /// The config-base of the object containing all of it's properties.
        /// </summary>
        public PlayerSpawnPointObject Base;

        /// <inheritdoc cref="MapEditorObject.UpdateObject()"/>
        public override void UpdateObject()
        {
            gameObject.tag = Base.RoleType.ConvertToSpawnPointTag();
        }
    }
}
