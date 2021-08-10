namespace MapEditorReborn.API
{
    using Exiled.API.Features;
    using Mirror;
    using UnityEngine;

    public class RagdollObjectComponent : MonoBehaviour
    {
        /// <summary>
        /// Ragdoll's nickname.
        /// </summary>
        public string RagdollName = string.Empty;

        /// <summary>
        /// Ragdoll's role type.
        /// </summary>
        public RoleType RagdollRoleType = RoleType.ClassD;

        /// <summary>
        /// Ragdoll's damage type.
        /// </summary>
        public string RagdollDamageType = "None";

        /// <summary>
        /// Ragdoll that is attached to this object.
        /// </summary>
        public Ragdoll AttachedRagdoll = null;

        private void Start()
        {
            AttachedRagdoll = Map.SpawnRagdoll(RagdollRoleType, RagdollDamageType.ConvertToDamageType(), RagdollName, gameObject.transform.position, gameObject.transform.rotation);
        }

        private void OnDestroy()
        {
            if (AttachedRagdoll != null)
                NetworkServer.Destroy(AttachedRagdoll.gameObject);
        }
    }
}
