namespace MapEditorReborn.Commands.Position
{
    using System;
    using CommandSystem;
    using SubCommands;

    /// <summary>
    /// Command used for modifing object's position.
    /// </summary>
    public class Position : ParentCommand
    {
        /// <inheritdoc/>
        public Position() => LoadGeneratedCommands();

        /// <inheritdoc/>
        public override string Command => "position";

        /// <inheritdoc/>
        public override string[] Aliases => new string[] { "pos" };

        /// <inheritdoc/>
        public override string Description => "Modifies object's posistion.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Add());
            RegisterCommand(new Set());
            RegisterCommand(new Bring());
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "\nUsage:\n";
            response += "mp position set (x) (y) (z)\n";
            response += "mp position add (x) (y) (z)\n";
            response += "mp position bring";
            return false;
        }
    }
}
