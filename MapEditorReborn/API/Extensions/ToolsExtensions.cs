// -----------------------------------------------------------------------
// <copyright file="ToolsExtensions.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using InventorySystem.Items;
using MapEditorReborn.Exiled.Features.Items;

namespace MapEditorReborn.API.Extensions;

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
    public static bool IsToolGun(this ItemBase item) => item != null && ToolGuns.ContainsKey(item.ItemSerial);

    /// <summary>
    /// Gets a value indicating whether the specified <see cref="Item"/> is a GravityGun.
    /// </summary>
    /// <param name="item">The <see cref="Item"/> to check.</param>
    /// <returns><see langword="true"/> if the <paramref name="item"/> is a GravityGun; otherwise, <see langword="false"/>.</returns>
    public static bool IsGravityGun(this ItemBase item) => item != null && GravityGuns.Contains(item.ItemSerial);
}