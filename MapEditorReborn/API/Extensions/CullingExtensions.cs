// -----------------------------------------------------------------------
// <copyright file="CullingExtensions.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Extensions
{
    using Exiled.API.Features;
    using Features.Objects;
    using Mirror;

    /// <summary>
    /// A set of useful extensions to easily interact with culling features.
    /// </summary>
    public static class CullingExtensions
    {
        /// <summary>
        /// Spawns the given <paramref name="schematic"/> for the specified <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The target.</param>
        /// <param name="schematic">The schematic to spawn.</param>
        public static void SpawnSchematic(this Player player, SchematicObject schematic)
        {
            foreach (NetworkIdentity networkIdentity in schematic.NetworkIdentities)
                player.SpawnNetworkIdentity(networkIdentity);
        }

        /// <summary>
        /// Destroys the given <paramref name="schematic"/> for the specified <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The target.</param>
        /// <param name="schematic">The schematic to destroy.</param>
        public static void DestroySchematic(this Player player, SchematicObject schematic)
        {
            foreach (NetworkIdentity networkIdentity in schematic.NetworkIdentities)
                player.DestroyNetworkIdentity(networkIdentity);
        }

        /// <summary>
        /// Spawns the given <paramref name="networkIdentity"/> for the specified <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The target.</param>
        /// <param name="networkIdentity">The network identity to spawn.</param>
        public static void SpawnNetworkIdentity(this Player player, NetworkIdentity networkIdentity) =>
            Server.SendSpawnMessage.Invoke(null, new object[] { networkIdentity, player.Connection });

        /// <summary>
        /// Destroys the given <paramref name="networkIdentity"/> for the specified <paramref name="player"/>.
        /// </summary>
        /// <param name="player">The target.</param>
        /// <param name="networkIdentity">The network identity to destroy.</param>
        public static void DestroyNetworkIdentity(this Player player, NetworkIdentity networkIdentity) =>
            player.Connection.Send(new ObjectDestroyMessage
                { netId = networkIdentity.netId });
    }
}
