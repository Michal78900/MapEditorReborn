namespace MapEditorReborn.Events.Handlers.Internal
{
    using System.Linq;
    using API.Features.Serializable.Vanilla;
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs;
    using static API.API;

    public class VanillaTeslaHandler
    {
        private static VanillaTeslaProperties Properties => CurrentLoadedMap.VanillaTeslaProperties;

        internal static void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.TriggeringTesla += OnTriggeringTesla;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        internal static void UnRegisterEvents()
        {
            Exiled.Events.Handlers.Player.TriggeringTesla -= OnTriggeringTesla;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
        }

        private static void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (Properties.IgnoredRoles.Contains(ev.Player.Role.Type.ToString()))
            {
                ev.IsInIdleRange = false;
                ev.IsTriggerable = false;
                return;
            }

            InventorySystem.Items.ItemBase itemBase = null;
            foreach (ItemType itemType in CurrentLoadedMap.VanillaTeslaProperties.IgnoredItems)
            {
                itemBase = ev.Player.Inventory.UserInventory.Items.Values.FirstOrDefault(x => x.ItemTypeId == itemType);
                if (itemBase is not null)
                    break;
            }

            if (itemBase is not null && (Properties.InventoryItem || Item.Get(itemBase) == ev.Player.CurrentItem))
            {
                ev.IsInIdleRange = false;
                ev.IsTriggerable = false;
                return;
            }
        }

        private static void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Handler.Type == DamageType.Tesla)
                ev.Amount *= Properties.DamageMultiplier;
        }
    }
}