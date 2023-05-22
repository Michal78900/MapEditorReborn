using System;

namespace MapEditorReborn.Events;

public class Events
{
    public event EventHandler<System.EventArgs> Handler;

    public virtual void Invoke(System.EventArgs t) => Handler?.Invoke(this, t);

    public delegate void CustomEventHandler<in T>(T ev)
        where T : System.EventArgs;
}