// -----------------------------------------------------------------------
// <copyright file="ToolsExtensions.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Extensions
{
    using Exiled.API.Features.Items;
    using static API;

    /// <summary>
    /// A set of useful extensions to easily interact with ToolGun items.
    /// </summary>
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
        public static bool IsGravityGun(this Item item) => item != null && GravityGuns.ContainsKey(item.Serial);
    }
}
