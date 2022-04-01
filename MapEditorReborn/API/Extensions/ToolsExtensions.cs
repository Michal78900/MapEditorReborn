namespace MapEditorReborn.API.Extensions
{
    using Exiled.API.Features.Items;

    using static API;

    public static class ToolsExtensions
    {
        /// <summary>
        /// Gets a value indicating whether the specified <see cref="Item"/> is a ToolGun.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to check.</param>
        /// <returns><see langword="true"/> if the <paramref name="item"/> is a ToolGun; otherwise, <see langword="false"/>.</returns>
        public static bool IsToolGun(this Item item) => item != null && ToolGuns.ContainsKey(item.Serial);

        /// <summary>
        /// Gets a value indicating whether the specified <see cref="Item"/> is a GravityGun.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to check.</param>
        /// <returns><see langword="true"/> if the <paramref name="item"/> is a GravityGun; otherwise, <see langword="false"/>.</returns>
        public static bool IsGravityGun(this Item item) => item != null && GravityGuns.Contains(item.Serial);
    }
}
