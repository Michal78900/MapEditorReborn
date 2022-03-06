namespace MapEditorReborn.Commands
{
    using System;
    using API.Features.Objects;
    using CommandSystem;
    using Events.EventArgs;
    using Events.Handlers.Internal;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Command used for copying the objects.
    /// </summary>
    public class CopyObject : ICommand
    {
        /// <inheritdoc/>
        public string Command => "copy";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "cp" };

        /// <inheritdoc/>
        public string Description => "Copies the object which you are looking at.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            Player player = Player.Get(sender);

            if (ToolGunHandler.TryGetMapObject(player, out MapEditorObject mapObject))
            {
                CopyingObjectEventArgs ev = new CopyingObjectEventArgs(player, mapObject, true);
                Events.Handlers.MapEditorObject.OnCopyingObject(ev);

                if (!ev.IsAllowed)
                {
                    response = ev.Response;
                    return true;
                }

                ToolGunHandler.CopyObject(player, ev.Object);
                response = "You've successfully copied the object!";
                return true;
            }
            else
            {
                response = "You aren't looking at any Map Editor object!";
                return false;
            }
        }
    }
}
