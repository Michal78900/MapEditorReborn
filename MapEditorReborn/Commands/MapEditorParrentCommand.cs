namespace MapEditorReborn.Commands
{
    using System;
    using System.Linq;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;

    /// <summary>
    /// The base parent command.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class MapEditorParrentCommand : ParentCommand
    {
        /// <inheritdoc/>
        public MapEditorParrentCommand() => LoadGeneratedCommands();

        /// <inheritdoc/>
        public override string Command => "mapeditor";

        /// <inheritdoc/>
        public override string[] Aliases => new string[] { "mp" };

        /// <inheritdoc/>
        public override string Description => "The MapEditorReborn parent command";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new ToolGun());
            RegisterCommand(new Save());
            RegisterCommand(new Load());
            RegisterCommand(new ShowIndicators());
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get((sender as PlayerCommandSender).ReferenceHub);

            string message = "\nPlease enter a valid subcommand:\n\n";

            foreach (var command in AllCommands.ToList())
            {
                if (player.CheckPermission($"mpr.{command.Command}"))
                {
                    message += $"- {command.Command} ({command.Aliases[0]})\n{command.Description}\n\n";
                }
            }

            response = message;
            return false;
        }
    }
}
