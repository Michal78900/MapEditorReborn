namespace MapEditorReborn.Events.Handlers.Internal
{
    using System.Linq;
    using API.Features.Serializable.Vanilla;
    using Exiled.API.Enums;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Handlers;
    using InventorySystem.Items;
    using static API.API;
    using Item = Exiled.API.Features.Items.Item;

    public class VanillaTeslaHandler
    {
        private static VanillaTeslaProperties Properties => CurrentLoadedMap.VanillaTeslaProperties;

        internal static void RegisterEvents()
        {
            Player.TriggeringTesla += OnTriggeringTesla;
            Player.Hurting += OnHurting;
        }

        internal static void UnRegisterEvents()
        {
            Player.TriggeringTesla -= OnTriggeringTesla;
            Player.Hurting -= OnHurting;
        }

        private static void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (Properties.IgnoredRoles.Contains(ev.Player.Role.Type.ToString()))
            {
                ev.IsInIdleRange = false;
                ev.IsAllowed = false;
                return;
            }

            ItemBase? itemBase = null;
            foreach (ItemType itemType in CurrentLoadedMap.VanillaTeslaProperties.IgnoredItems)
            {
                itemBase = ev.Player.Inventory.UserInventory.Items.Values.FirstOrDefault(x => x.ItemTypeId == itemType);
                if (itemBase is not null)
                    break;
            }

            if (itemBase is not null && (Properties.InventoryItem || Item.Get(itemBase) == ev.Player.CurrentItem))
            {
                ev.IsInIdleRange = false;
                ev.IsAllowed = false;
            }
        }

        private static void OnHurting(HurtingEventArgs ev)
        {
            if (ev.DamageHandler.Type == DamageType.Tesla)
                ev.Amount *= Properties.DamageMultiplier;
        }
    }
}