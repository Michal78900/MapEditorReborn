namespace MapEditorReborn.Commands
{
    using System;
    using API;
    using CommandSystem;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

    public class SetRoomType : ICommand
    {
        /// <inheritdoc/>
        public string Command => "setroomtype";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "setroom", "resetroom", "rr" };

        /// <inheritdoc/>
        public string Description => "Sets the object's room type.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            Player player = Player.Get(sender);
            if (!player.TryGetSessionVariable(Handler.SelectedObjectSessionVarName, out MapEditorObject mapObject) || mapObject == null)
            {
                if (!Handler.TryGetMapObject(player, out mapObject))
                {
                    response = "You haven't selected any object!";
                    return false;
                }
                else
                {
                    Handler.SelectObject(player, mapObject);
                }
            }

            if (arguments.Count == 0)
            {
                mapObject.ForcedRoomType = RoomType.Unknown;
                mapObject.ForcedRoomType = mapObject.FindRoom().Type;
                player.ShowGameObjectHint(mapObject);

                response = $"Object's RoomType has been reseted! It is now \"{mapObject.ForcedRoomType}\"";
                return true;
            }

            if (Enum.TryParse(arguments.At(0), true, out RoomType roomType))
            {
                if (roomType == RoomType.Unknown)
                    roomType = RoomType.Surface;

                mapObject.ForcedRoomType = roomType;
                player.ShowGameObjectHint(mapObject);

                response = $"Object's RoomType has been set to \"{roomType}\"!";
                return true;
            }

            response = $"\"{arguments.At(0)}\" is an invalid room type!";
            return false;
        }
    }
}
