namespace MapEditorReborn.Commands
{
    using System;
    using System.Linq;
    using API;
    using CommandSystem;
    using Exiled.API.Features;
    using RemoteAdmin;

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
            Player player = Player.Get((sender as PlayerCommandSender).ReferenceHub);

            foreach (var item in player.Items.ToList())
            {
                if (Handler.ToolGuns.ContainsKey(item.uniq))
                {
                    Handler.ToolGuns.Remove(item.uniq);
                    player.RemoveItem(item);
                    player.Ammo[2]--;

                    response = "You no longer have a Tool Gun!";
                    return true;
                }
            }

            if (player.Items.Count >= 8)
            {
                response = "You have full inventory!";
                return false;
            }

            Inventory.SyncItemInfo toolGun = new Inventory.SyncItemInfo()
            {
                id = ItemType.GunCOM15,
                durability = 1,
                modSight = 0,
                modBarrel = 1,
                modOther = 1,
                uniq = Inventory._uniqId++,
            };

            player.AddItem(toolGun);

            Handler.ToolGuns.Add(toolGun.uniq + 2, ToolGunMode.LczDoor);

            player.Ammo[2]++;

            response = "You now have the Tool Gun!\n\n";
            return true;
        }
    }
}
