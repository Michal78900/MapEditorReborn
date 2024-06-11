﻿// -----------------------------------------------------------------------
// <copyright file="OpenDirectory.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.UtilityCommands
{
    using System;
    using System.Diagnostics;
    using CommandSystem;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Command used for opening folder in which maps are stored.
    /// </summary>
    public class OpenDirectory : ICommand
    {
        /// <inheritdoc/>
        public string Command => "opendirectory";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "od", "openfolder" };

        /// <inheritdoc/>
        public string Description => "Opens the MapEditorParent directory.";

        /// <inheritdoc/>
        public bool SanitizeResponse => false;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = "У вас недостаточно прав на выполнения этой команды.";
                return false;
            }

            Process.Start(MapEditorReborn.PluginDir);

            response = "Папка успешно открыта!";
            return true;
        }
    }
}
