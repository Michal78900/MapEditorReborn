namespace MapEditorReborn.API
{
    using Exiled.API.Features;
    using Mirror;
    using UnityEngine;

    public class RagdollObjectComponent : MonoBehaviour
    {
        public string RagdollName = string.Empty;

        public RoleType RagdollRoleType = RoleType.ClassD;

        public Ragdoll AttachedRagdoll = null;

        private void Start()
        {
            AttachedRagdoll = Map.SpawnRagdoll(RagdollRoleType, DamageTypes.None, RagdollName, gameObject.transform.position, gameObject.transform.rotation);
        }

        private void OnDestroy()
        {
            if (AttachedRagdoll != null)
                NetworkServer.Destroy(AttachedRagdoll.gameObject);
        }
    }
}
