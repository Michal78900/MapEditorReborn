namespace MapEditorReborn.API
{
    using Exiled.API.Features;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Used for handling RagdollSpawnPoint's spawning ragdolls.
    /// </summary>
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
            AttachedRagdoll = Ragdoll.Spawn(RagdollRoleType, RagdollDamageType.ConvertToDamageType(), RagdollName, gameObject.transform.position, gameObject.transform.rotation);
        }

        private void OnDestroy()
        {
            if (AttachedRagdoll != null)
                NetworkServer.Destroy(AttachedRagdoll.GameObject);
        }
    }
}
