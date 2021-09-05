namespace MapEditorReborn.API
{
    using UnityEngine;

    /// <summary>
    /// Component added to a ShootingTargetObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class ShootingTargetComponent : MapEditorObject
    {
        /// <summary>
        /// The target type of the ShootingTargetObject.
        /// </summary>
        public string TargetType;

        /// <summary>
        /// Instantiates <see cref="ShootingTargetComponent"/>.
        /// </summary>
        /// <param name="shootingTargetObject"><see cref="ShootingTargetComponent"/> used for instantiating the object. May be <see langword="null"/>.</param>
        public void Init(ShootingTargetObject shootingTargetObject = null)
        {
            if (shootingTargetObject != null)
            {
                TargetType = shootingTargetObject.TargetType;
            }
            else
            {
                switch (gameObject.name)
                {
                    case "sportTargetPrefab(Clone)":
                        {
                            TargetType = "Sport";
                            break;
                        }

                    case "dboyTargetPrefab(Clone)":
                        {
                            TargetType = "Dboy";
                            break;
                        }

                    case "binaryTargetPrefab(Clone)":
                        {
                            TargetType = "Binary";
                            break;
                        }
                }
            }

            // gameObject.GetComponent<InventorySystem.Items.Firearms.Utilities.ShootingTarget>().Network_syncMode = true;
        }
    }
}
