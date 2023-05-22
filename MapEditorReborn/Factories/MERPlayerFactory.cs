using System;
using PluginAPI.Core.Factories;
using PluginAPI.Core.Interfaces;

namespace MapEditorReborn.Factories;

public class MERPlayerFactory : PlayerFactory
{
    public override Type BaseType { get; } = typeof(MERPlayer);

    public override IPlayer Create(IGameComponent component)
    {
        return new MERPlayer(component);
    }
}