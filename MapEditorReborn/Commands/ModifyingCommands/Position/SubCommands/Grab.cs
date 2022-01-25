namespace MapEditorReborn.Commands.Position.SubCommands
{
    using System;
    using System.Collections.Generic;
    using API.Extensions;
    using API.Features.Components;
    using CommandSystem;
    using Events.EventArgs;
    using Events.Handlers.Internal;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using MEC;
    using UnityEngine;

    using static API.API;

    /// <summary>
    /// Grabs a specific <see cref="MapEditorObject"/>.
    /// </summary>
    public class Grab : ICommand
    {
        /// <inheritdoc/>
        public string Command => "grab";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => "Grabs an object.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            Player player = Player.Get(sender);
            if (!player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) || mapObject == null)
            {
                if (!ToolGunHandler.TryGetMapObject(player, out mapObject))
                {
                    response = "You haven't selected any object!";
                    return false;
                }
                else
                {
                    ToolGunHandler.SelectObject(player, mapObject);
                }
            }

            if (GrabbingPlayers.ContainsKey(player))
            {
                ReleasingObjectEventArgs releasingEv = new ReleasingObjectEventArgs(player, mapObject, true);
                Events.Handlers.MapEditorObject.OnReleasingObject(releasingEv);

                if (!releasingEv.IsAllowed)
                {
                    response = releasingEv.Response;
                    return true;
                }

                Timing.KillCoroutines(GrabbingPlayers[player]);
                GrabbingPlayers.Remove(player);
                response = "Ungrabbed";
                return true;
            }

            GrabbingObjectEventArgs grabbingEv = new GrabbingObjectEventArgs(player, mapObject, true);
            Events.Handlers.MapEditorObject.OnGrabbingObject(grabbingEv);

            if (!grabbingEv.IsAllowed)
            {
                response = grabbingEv.Response;
                return true;
            }

            GrabbingPlayers.Add(player, Timing.RunCoroutine(GrabbingCoroutine(player, grabbingEv.Object)));

            response = "Grabbed";
            return true;
        }

        private IEnumerator<float> GrabbingCoroutine(Player player, MapEditorObject mapObject)
        {
            float multiplier = Vector3.Distance(player.CameraTransform.position, mapObject.transform.position);
            Vector3 prevPos = player.CameraTransform.position + (player.CameraTransform.forward * multiplier);
            Vector3 newPos;
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

                ChangingObjectPositionEventArgs ev = new ChangingObjectPositionEventArgs(player, mapObject, prevPos, true);
                Events.Handlers.MapEditorObject.OnChangingObjectPosition(ev);

                if (!ev.IsAllowed)
                    break;

                mapObject.transform.position = prevPos;
                mapObject.UpdateObject();
                mapObject.UpdateIndicator();
            }

            GrabbingPlayers.Remove(player);
        }

        /// <summary>
        /// The <see cref="Dictionary{TKey, TValue}"/> which contains all <see cref="Player"/> and <see cref="CoroutineHandle"/> pairs.
        /// </summary>
        public static Dictionary<Player, CoroutineHandle> GrabbingPlayers = new Dictionary<Player, CoroutineHandle>();
    }
}
