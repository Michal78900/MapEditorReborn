namespace MapEditorReborn.API.Extensions
{
    using Exiled.API.Features;
    using Features.Objects;
    using Mirror;

    public static class CullingExtenstions
    {
        public static void SpawnSchematic(this Player player, SchematicObject schematic)
        {
            foreach (NetworkIdentity networkIdentity in schematic.NetworkIdentities)
            {
                player.SpawnNetworkIdentity(networkIdentity);
            }
        }

        public static void DestroySchematic(this Player player, SchematicObject schematic)
        {
            foreach (NetworkIdentity networkIdentity in schematic.NetworkIdentities)
            {
                player.DestroyNetworkIdentity(networkIdentity);
            }
        }

        public static void SpawnNetworkIdentity(this Player player, NetworkIdentity networkIdentity) =>
            Server.SendSpawnMessage.Invoke(null, new object[2] { networkIdentity, player.Connection });

        public static void DestroyNetworkIdentity(this Player player, NetworkIdentity networkIdentity) =>
            player.Connection.Send(new ObjectDestroyMessage() { netId = networkIdentity.netId });
    }
}
