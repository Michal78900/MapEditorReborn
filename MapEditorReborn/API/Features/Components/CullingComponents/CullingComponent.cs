namespace MapEditorReborn.API.Features.Components.CullingComponents
{
    using System.Collections.Generic;
    using Enums;
    using Exiled.API.Features;
    using Extensions;
    using MEC;
    using Mirror;
    using ObjectComponents;
    using UnityEngine;

    public class CullingComponent : MonoBehaviour
    {
        public static List<Collider> CullingColliders = new List<Collider>();

        public BoxCollider BoxCollider;

        public void Init(Player player)
        {
            this.player = player;

            BoxCollider = gameObject.AddComponent<BoxCollider>();
            CullingColliders.Add(BoxCollider);

            BoxCollider.center = Vector3.forward * 27.5f;
            BoxCollider.isTrigger = true;

            RefreshSize();
        }

        public void RefreshSize()
        {
            bool godmode = player.IsGodModeEnabled;
            player.IsGodModeEnabled = true;

            BoxCollider.size = Vector3.one * 100000f;
            Timing.CallDelayed(0.25f, () =>
            {
                BoxCollider.size = new Vector3(80f, 45f, 65f);
                player.IsGodModeEnabled = godmode;
            });
        }

        private void OnTriggerEnter(Collider collider)
        {
            SchematicObjectComponent schematicObjectComponent = collider.GetComponentInParent<SchematicObjectComponent>();
            if (schematicObjectComponent == null || schematicObjectComponent.Base.CullingType != CullingType.Distance)
                return;

            NetworkIdentity networkIdentity = collider.gameObject.GetComponentInParent<NetworkIdentity>();
            if (networkIdentity == null)
                return;

            player.SpawnNetworkIdentity(networkIdentity);
        }

        private void OnTriggerExit(Collider collider)
        {
            SchematicObjectComponent schematicObjectComponent = collider.GetComponentInParent<SchematicObjectComponent>();
            if (schematicObjectComponent == null || schematicObjectComponent.Base.CullingType != CullingType.Distance)
                return;

            NetworkIdentity networkIdentity = collider.GetComponentInParent<NetworkIdentity>();
            if (networkIdentity == null)
                return;

            player.DestroyNetworkIdentity(networkIdentity);
        }

        private void OnDestroy() => CullingColliders.Remove(BoxCollider);

        private Player player;
    }
}
