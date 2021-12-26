namespace MapEditorReborn.Commands
{
    using System;
    using API.Features.Components;
    using CommandSystem;
    using Events.Handlers.Internal;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Command used for selecting the objects.
    /// </summary>
    public class SelectObject : ICommand
    {
        /// <inheritdoc/>
        public string Command => "select";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "sel", "choose" };

        /// <inheritdoc/>
        public string Description => "Selects the object which you are looking at.";

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
                response = "You've successfully selected the object!";
            }
            else
            {
                response = "You've unselected the object!";
            }

            ToolGunHandler.SelectObject(player, mapObject);
            return true;
        }
    }
}
