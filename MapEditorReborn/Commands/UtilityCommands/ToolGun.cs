namespace MapEditorReborn.Commands
{
    using System;
    using System.Linq;
    using API.Enums;
    using CommandSystem;
    using Events.EventArgs;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Permissions.Extensions;

    using static API.API;

    /// <summary>
    /// Command which gives a ToolGun to a sender.
    /// </summary>
    public class ToolGun : ICommand
    {
        /// <inheritdoc/>
        public string Command => "toolgun";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "tg" };

        /// <inheritdoc/>
        public string Description => "Tool gun for spawning and editing objects.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            Player player = Player.Get(sender);

            foreach (var item in player.Items.ToList())
            {
                if (ToolGuns.ContainsKey(item.Serial))
                {
                    DroppingToolGunEventArgs droppingEv = new DroppingToolGunEventArgs(player, true);
                    Events.Handlers.Utility.OnDroppingToolGun(droppingEv);

                    if (!droppingEv.IsAllowed)
                    {
                        response = droppingEv.Response;
                        return true;
                    }

                    ToolGuns.Remove(item.Serial);
                    player.RemoveItem(item);

                    response = "You no longer have a Tool Gun!";
                    return true;
                }
            }

            if (player.Items.Count >= 8)
            {
                response = "You have a full inventory!";
                return false;
            }

            PickingUpToolGunEventArgs ev = new PickingUpToolGunEventArgs(player, true);
            Events.Handlers.Utility.OnPickingUpToolGun(ev);

            if (!ev.IsAllowed)
            {
                response = ev.Response;
                return true;
            }

            Item toolgun = player.AddItem(ItemType.GunCOM15);
            Firearm firearm = toolgun as Firearm;

            firearm.Base.Status = new InventorySystem.Items.Firearms.FirearmStatus((byte)(firearm.MaxAmmo + 1), (InventorySystem.Items.Firearms.FirearmStatusFlags)28, 77);

            ToolGuns.Add(toolgun.Serial, ObjectType.LczDoor);

            response = "You now have the Tool Gun!\n\n";
            return true;
        }
    }
}
