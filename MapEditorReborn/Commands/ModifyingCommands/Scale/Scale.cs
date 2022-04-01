namespace MapEditorReborn.Commands.Scale
{
    using System;
    using API.Features.Objects;
    using CommandSystem;
    using Exiled.API.Features;
    using SubCommands;

    using static API.API;

    /// <summary>
    /// Command used for modifing object's scale.
    /// </summary>
    public class Scale : ParentCommand
    {
        /// <inheritdoc/>
        public Scale() => LoadGeneratedCommands();

        /// <inheritdoc/>
        public override string Command => "scale";

        /// <inheritdoc/>
        public override string[] Aliases => new string[] { "scl" };

        /// <inheritdoc/>
        public override string Description => "Modifies object's scale.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Add());
            RegisterCommand(new Set());
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            if (player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) && mapObject != null)
            {
                response = $"Object current scale: {mapObject.Scale}\n";
                return true;
            }

            response = "\nUsage:\n";
            response += "mp scale set (x) (y) (z)\n";
            response += "mp scale add (x) (y) (z)\n";
            return false;
        }
    }
}
