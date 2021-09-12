namespace MapEditorReborn.Commands.Rotation
{
    using System;
    using API;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using Mirror;
    using RemoteAdmin;
    using SubCommands;
    using UnityEngine;

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
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "\nUsage:\n";
            response += "mp rotation set (x) (y) (z)\n";
            response += "mp rotation add (x) (y) (z)\n";
            return false;
        }
    }
}
