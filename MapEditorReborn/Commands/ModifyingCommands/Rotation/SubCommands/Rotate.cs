namespace MapEditorReborn.Commands.Rotation
{
    using System;
    using System.Collections.Generic;
    using API;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using MEC;
    using UnityEngine;

    public class Rotate : ICommand
    {
        public string Command => "rotate";

        public string[] Aliases => Array.Empty<string>();

        public string Description => string.Empty;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mpr.rotation"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.rotation";
                return false;
            }

            Player player = Player.Get(sender);
            if (!player.TryGetSessionVariable(Methods.SelectedObjectSessionVarName, out MapEditorObject mapObject) || mapObject == null)
            {
                if (!Methods.TryGetMapObject(player, out mapObject))
                {
                    response = "You haven't selected any object!";
                    return false;
                }
                else
                {
                    Methods.SelectObject(player, mapObject);
                }
            }

            if (RotatingPlayers.ContainsKey(player))
            {
                Timing.KillCoroutines(RotatingPlayers[player]);
                RotatingPlayers.Remove(player);
                response = "Ungrabbed";
                return true;
            }

            RotatingPlayers.Add(player, Timing.RunCoroutine(RotatingCoroutine(player, mapObject)));

            response = "Grabbed";
            return true;
        }

        private IEnumerator<float> RotatingCoroutine(Player player, MapEditorObject mapObject)
        {
            Vector3 playerStartPos = player.Position;
            int i = 0;

            while (!RoundSummary.singleton.RoundEnded)
            {
                yield return Timing.WaitForOneFrame;

                i++;
                if (i == 60)
                {
                    i = 0;
                    player.ShowGameObjectHint(mapObject);
                }

                if (playerStartPos == player.Position)
                    continue;

                mapObject.transform.eulerAngles += Round((playerStartPos - player.Position) * 10f);
                mapObject.UpdateObject();
                mapObject.UpdateIndicator();
                player.Position = playerStartPos;
            }

            RotatingPlayers.Remove(player);
        }

        private Vector3 Round(Vector3 vector)
        {
            vector.x = Mathf.Round(vector.x);
            vector.y = Mathf.Round(vector.y);
            vector.z = Mathf.Round(vector.z);

            return vector;
        }

        public static Dictionary<Player, CoroutineHandle> RotatingPlayers = new Dictionary<Player, CoroutineHandle>();

    }
}
