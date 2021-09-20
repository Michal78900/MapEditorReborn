namespace MapEditorReborn.API
{
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Component added to RagdollSpawnPointObject. Is is used for easier idendification of the object and it's variables.
    /// </summary>
    public class RagdollSpawnPointComponent : MapEditorObject
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
        /// Initializes the <see cref="RagdollSpawnPointComponent"/>.
        /// </summary>
        /// <param name="ragdollSpawnPoint">The <see cref="RagdollSpawnPointObject"/> to instantiate.</param>
        public void Init(RagdollSpawnPointObject ragdollSpawnPoint = null)
        {
            if (ragdollSpawnPoint != null)
            {
                if (string.IsNullOrEmpty(RagdollName) && Handler.CurrentLoadedMap.RagdollRoleNames.TryGetValue(ragdollSpawnPoint.RoleType, out List<string> ragdollNames))
                {
                    RagdollName = ragdollNames[Random.Range(0, ragdollNames.Count)];
                }
                else
                {
                    RagdollName = ragdollSpawnPoint.Name;
                }

                RagdollRoleType = ragdollSpawnPoint.RoleType;
                RagdollDamageType = ragdollSpawnPoint.DamageType;
            }

            attachedRagdoll = Ragdoll.Spawn(RagdollRoleType, RagdollDamageType.ConvertToDamageType(), RagdollName, transform.position, transform.rotation);
        }

        private void OnDestroy()
        {
            if (attachedRagdoll != null)
                attachedRagdoll.Delete();
        }

        private Ragdoll attachedRagdoll = null;
    }
}
