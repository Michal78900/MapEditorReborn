using System.Globalization;
using MapEditorReborn.Exiled.Features;
using UnityEngine;

namespace MapEditorReborn.Commands;

using System;
using CommandSystem;

[CommandHandler(typeof(ClientCommandHandler))]
public class TpsCommand : ICommand
{
    /// <inheritdoc/>
    public string Command
    {
        get => "tps";
    }

    /// <inheritdoc/>
    public string[] Aliases { get; } = Array.Empty<string>();

    /// <inheritdoc/>
    public string Description
    {
        get => "Shows the current TPS.";
    }

    /// <inheritdoc/>
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        double diff = Math.Round(1.0 / Time.smoothDeltaTime) / ServerStatic.ServerTickrate;
        var color = diff switch
        {
            > 0.9 => "green",
            > 0.5 => "yellow",
            _ => "red"
        };

        response = $"<color={color}>{Math.Round(1.0 / Time.smoothDeltaTime).ToString(CultureInfo.InvariantCulture)}/{ServerStatic.ServerTickrate}</color>";
        return true;
    }
}