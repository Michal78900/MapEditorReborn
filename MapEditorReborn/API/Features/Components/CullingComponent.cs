namespace MapEditorReborn.API.Features.Components
{
    using System.Collections.Generic;
    using Enums;
    using Exiled.API.Features;
    using Extensions;
    using Mirror;
    using Objects;
    using UnityEngine;

    /// <summary>
    /// Handles all culling related features.
    /// </summary>
    public class CullingComponent : MonoBehaviour
    {
        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="Collider"/> containing all the existing culling colliders.
        /// </summary>
        public static List<Collider> CullingColliders { get; } = new();

        /// <summary>
        /// Gets the <see cref="UnityEngine.BoxCollider"/>.
        /// </summary>
        public BoxCollider BoxCollider { get; private set; }

        /// <summary>
        /// Initializes the a new instances of the <see cref="CullingComponent"/> class.
        /// </summary>
        /// <param name="player">The owner of the component.</param>
        public void Init(Player player)
        {
            this.player = player;

            BoxCollider = gameObject.AddComponent<BoxCollider>();
            BoxCollider.center = Config.CullingOffset;
            BoxCollider.size = Config.CullingSize;
            gameObject.transform.localEulerAngles = Config.CullingRotation;

            CullingColliders.Add(BoxCollider);
            BoxCollider.isTrigger = true;
        }

        /// <summary>
        /// Refreshes the specified schematic.
        /// </summary>
        /// <param name="schematic">The schematic to refresh.</param>
        public void RefreshForSchematic(SchematicObject schematic)
        {
            foreach (NetworkIdentity networkIdentity in schematic.NetworkIdentities)
            {
                if (!BoxCollider.bounds.Contains(networkIdentity.transform.position))
                    player.DestroyNetworkIdentity(networkIdentity);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            SchematicObject schematicObject = collider.GetComponentInParent<SchematicObject>();
            if (schematicObject == null || schematicObject.Base.CullingType != CullingType.Distance)
                return;

            NetworkIdentity networkIdentity = collider.gameObject.GetComponentInParent<NetworkIdentity>();
            if (networkIdentity == null)
                return;

            player.SpawnNetworkIdentity(networkIdentity);
        }

        private void OnTriggerExit(Collider collider)
        {
            SchematicObject schematicObject = collider.GetComponentInParent<SchematicObject>();
            if (schematicObject == null || schematicObject.Base.CullingType != CullingType.Distance)
                return;

            NetworkIdentity networkIdentity = collider.gameObject.GetComponentInParent<NetworkIdentity>();
            if (networkIdentity == null)
                return;

            player.DestroyNetworkIdentity(networkIdentity);
        }

        private void OnDestroy() => CullingColliders.Remove(BoxCollider);

        private Player player;
        private static readonly Config Config = MapEditorReborn.Singleton.Config;
    }
}