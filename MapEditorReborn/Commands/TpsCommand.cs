namespace MapEditorReborn.Commands
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class TpsCommand : ICommand
    {
        /// <inheritdoc/>
        public string Command => "tps";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description => "Shows the current TPS.";

        /// <inheritdoc/>
        public bool SanitizeResponse => false;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            double diff = Server.Tps / ServerStatic.ServerTickrate;
            string color = diff switch
            {
                > 0.9 => "green",
                > 0.5 => "yellow",
                _ => "red"
            };

            response = $"<color={color}>{Server.Tps}/{ServerStatic.ServerTickrate}</color>";
            return true;
        }
    }
}