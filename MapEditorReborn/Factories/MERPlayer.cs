using System.Collections.Generic;
using System.Linq;
using PluginAPI.Core;
using PluginAPI.Core.Interfaces;

namespace MapEditorReborn.Factories;

public class MERPlayer : Player
{
    public Dictionary<string, object> SessionVariables { get; }
    
    public MERPlayer(IGameComponent component) : base(component)
    {
        SessionVariables = new Dictionary<string, object>();
    }
    
    public bool TryGetSessionVariable<T>(string key, out T result)
    {
        if (SessionVariables.TryGetValue(key, out object value) && value is T type)
        {
            result = type;
            return true;
        }

        result = default;
        return false;
    }
    
    public new static MERPlayer Get(string userId)
        => (from hub in ReferenceHub.AllHubs
            where hub.characterClassManager.UserId == userId
            select Get<MERPlayer>(hub)).FirstOrDefault();

    public new static MERPlayer Get(int playerId)
        => (from hub in ReferenceHub.AllHubs
                where hub.PlayerId == playerId
                select Get<MERPlayer>(hub))
            .FirstOrDefault();
    
    public new static MERPlayer Get(ReferenceHub refHub) => Get<MERPlayer>(refHub);
}