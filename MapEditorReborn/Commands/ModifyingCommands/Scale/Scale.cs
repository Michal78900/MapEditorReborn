// -----------------------------------------------------------------------
// <copyright file="Scale.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.ModifyingCommands.Scale
{
    using System;
    using API.Features.Objects;
    using CommandSystem;
    using Exiled.API.Features;
    using SubCommands;
    using static API.API;

    /// <summary>
    /// Command used for modifying object's scale.
    /// </summary>
    public class Scale : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scale"/> class.
        /// </summary>
        public Scale() => LoadGeneratedCommands();

        /// <inheritdoc/>
        public override string Command => "scale";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = { "scl" };

        /// <inheritdoc/>
        public bool SanitizeResponse => false;

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
