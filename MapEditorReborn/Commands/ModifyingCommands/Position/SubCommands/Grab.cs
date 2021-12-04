namespace MapEditorReborn.Commands.Position.SubCommands
{
    using System;
    using System.Collections.Generic;
    using API;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using MEC;
    using UnityEngine;

    public class Grab : ICommand
    {
        public string Command => "grab";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Grabs an object.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
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

            if (GrabbingPlayers.ContainsKey(player))
            {
                Timing.KillCoroutines(GrabbingPlayers[player]);
                GrabbingPlayers.Remove(player);
                response = "Ungrabbed";
                return true;
            }

            GrabbingPlayers.Add(player, Timing.RunCoroutine(GrabbingCoroutine(player, mapObject)));

            response = "Grabbed";
            return true;
        }

        private IEnumerator<float> GrabbingCoroutine(Player player, MapEditorObject mapObject)
        {
            float multiplier = Vector3.Distance(player.CameraTransform.position, mapObject.transform.position);
            Vector3 prevPos = player.CameraTransform.position + (player.CameraTransform.forward * multiplier);
            Vector3 newPos = Vector3.zero;
            int i = 0;

            while (!RoundSummary.singleton.RoundEnded)
            {
                yield return Timing.WaitForOneFrame;
                newPos = mapObject.transform.position = player.CameraTransform.position + (player.CameraTransform.forward * multiplier);

                i++;
                if (i == 60)
                {
                    i = 0;
                    player.ShowGameObjectHint(mapObject);
                }

                if (prevPos == newPos)
                    continue;

                prevPos = newPos;
                mapObject.transform.position = prevPos;
                mapObject.UpdateObject();
                mapObject.UpdateIndicator();
            }

            GrabbingPlayers.Remove(player);
        }

        public static Dictionary<Player, CoroutineHandle> GrabbingPlayers = new Dictionary<Player, CoroutineHandle>();
    }
}
