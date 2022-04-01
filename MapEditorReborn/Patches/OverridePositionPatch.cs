namespace MapEditorReborn.Patches
{
    using System.Collections.Generic;
    using API.Enums;
    using API.Extensions;
    using API.Features.Objects;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using HarmonyLib;
    using MEC;
    using Mirror;
    using UnityEngine;

    using static API.API;

    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OverridePosition))]
    internal static class OverridePositionPatch
    {
        internal static void ResetValues()
        {
            SurfaceSchematics.Clear();
            FacilitySchematics.Clear();
        }

        private static void Prefix(PlayerMovementSync __instance, Vector3 pos)
        {
            Player player = Player.Get(__instance.gameObject);

            if (player.TryGetSessionVariable("MapEditorReborn_LastYPos", out float lastY))
            {
                if ((lastY > 900f && pos.y < 900f) || (lastY < 900f && pos.y > 900f))
                {
                    player.SessionVariables["MapEditorReborn_LastYPos"] = pos.y;
                }
                else
                {
                    return;
                }
            }
            else
            {
                player.SessionVariables.Add("MapEditorReborn_LastYPos", pos.y);
            }

            if (SurfaceSchematics.Count == 0 || FacilitySchematics.Count == 0)
            {
                foreach (MapEditorObject mapEditorObject in SpawnedObjects)
                {
                    if (mapEditorObject is SchematicObject schematic && schematic.Base.CullingType == CullingType.Zone)
                    {
                        if (mapEditorObject.RoomType == RoomType.Surface)
                        {
                            SurfaceSchematics.Add(schematic);
                        }
                        else
                        {
                            FacilitySchematics.Add(schematic);
                        }
                    }
                }
            }

            if (SurfaceSchematics.Count == 0 || FacilitySchematics.Count == 0)
                return;

            if (pos.y > 900f)
            {
                foreach (SchematicObject schematic in FacilitySchematics)
                {
                    player.DestroySchematic(schematic);
                }

                foreach (SchematicObject schematic in SurfaceSchematics)
                {
                    uint i = 0;

                    foreach (NetworkIdentity networkIdentity in schematic.NetworkIdentities)
                    {
                        if (Config.SchematicBlockSpawnDelay >= 0)
                        {
                            Timing.CallDelayed(Config.SchematicBlockSpawnDelay * i, () => player.SpawnNetworkIdentity(networkIdentity));
                            i++;
                        }
                        else
                        {
                            player.SpawnNetworkIdentity(networkIdentity);
                        }
                    }
                }
            }
            else
            {
                foreach (SchematicObject schematic in SurfaceSchematics)
                {
                    player.DestroySchematic(schematic);
                }

                foreach (SchematicObject schematic in FacilitySchematics)
                {
                    uint i = 0;

                    foreach (NetworkIdentity networkIdentity in schematic.NetworkIdentities)
                    {
                        if (Config.SchematicBlockSpawnDelay >= 0)
                        {
                            Timing.CallDelayed(Config.SchematicBlockSpawnDelay * i, () => player.SpawnNetworkIdentity(networkIdentity));
                            i++;
                        }
                        else
                        {
                            player.SpawnNetworkIdentity(networkIdentity);
                        }
                    }
                }
            }
        }

        private static readonly List<SchematicObject> SurfaceSchematics = new List<SchematicObject>();
        private static readonly List<SchematicObject> FacilitySchematics = new List<SchematicObject>();
        private static readonly Config Config = MapEditorReborn.Singleton.Config;
    }
}
