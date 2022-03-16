namespace MapEditorReborn.API.Features.Components
{
    using System.Collections.Generic;
    using Enums;
    using Exiled.API.Features;
    using Extensions;
    using MEC;
    using Mirror;
    using Objects;
    using UnityEngine;

    public class CullingComponent : MonoBehaviour
    {
        public static List<Collider> CullingColliders = new List<Collider>();

        public BoxCollider BoxCollider;

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