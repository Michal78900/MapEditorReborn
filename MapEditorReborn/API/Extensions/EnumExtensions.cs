// -----------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Extensions
{
    using Enums;

    /// <summary>
    /// A set of useful extensions to easily interact with enums.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Compares two <see cref="TeleportFlags"/>.
        /// </summary>
        /// <param name="value">The first <see cref="TeleportFlags"/>.</param>
        /// <param name="flag">The second <see cref="TeleportFlags"/>.</param>
        /// <returns><see langword="true"/> if the <paramref name="value"/> has the <paramref name="flag"/>; otherwise, <see langword="false"/>.</returns>
        public static bool HasFlagFast(this TeleportFlags value, TeleportFlags flag) => (value & flag) == flag;

        /// <summary>
        /// Compares two <see cref="LockOnEvent"/>.
        /// </summary>
        /// <param name="value">The first <see cref="LockOnEvent"/>.</param>
        /// <param name="flag">The second <see cref="LockOnEvent"/>.</param>
        /// <returns><see langword="true"/> if the <paramref name="value"/> has the <paramref name="flag"/>; otherwise, <see langword="false"/>.</returns>
        public static bool HasFlagFast(this LockOnEvent value, LockOnEvent flag) => (value & flag) == flag;
    }
}
