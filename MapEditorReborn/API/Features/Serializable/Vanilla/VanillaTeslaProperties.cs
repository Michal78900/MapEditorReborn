namespace MapEditorReborn.API.Features.Serializable.Vanilla
{
    using System.Collections.Generic;

    public class VanillaTeslaProperties
    {
        public List<string> IgnoredRoles { get; set; } = new();

        public List<ItemType> IgnoredItems { get; set; } = new();

        public bool InventoryItem { get; set; } = true;

        public float DamageMultiplier { get; set; } = 1f;
    }
}