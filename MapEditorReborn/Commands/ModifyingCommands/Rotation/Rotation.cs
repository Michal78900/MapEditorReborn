namespace MapEditorReborn.Commands.Rotation
{
    using System;
    using API.Features.Objects;
    using CommandSystem;
    using Exiled.API.Features;
    using SubCommands;

    using static API.API;

    /// <summary>
    /// Command used for modifing object's rotation.
    /// </summary>
    public class Rotation : ParentCommand
    {
        /// <inheritdoc/>
        public Rotation() => LoadGeneratedCommands();

        /// <inheritdoc/>
        public override string Command => "rotation";

        /// <inheritdoc/>
        public override string[] Aliases => new string[] { "rot" };

        /// <inheritdoc/>
        public override string Description => "Modifies object's rotation.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Add());
            RegisterCommand(new Set());
            RegisterCommand(new Rotate());
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            if (player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) && mapObject != null)
            {
                response = $"Object current rotation: {mapObject.RelativeRotation}\n";
                return true;
            }

            response = "\nUsage:\n";
            response += "mp rotation set (x) (y) (z)\n";
            response += "mp rotation add (x) (y) (z)\n";
            return false;
        }
    }
}
