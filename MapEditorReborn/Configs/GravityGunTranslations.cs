// -----------------------------------------------------------------------
// <copyright file="Translation.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Configs
{
    using Exiled.API.Interfaces;

    /// <summary>
    /// Config with translations for the Gravity Gun.
    /// </summary>
    public sealed class GravityGunTranslations : ITranslation
    {
        /// <summary>
        /// Gets a text which represents the moving object mode.
        /// </summary>
        public string ModeMoveOnly { get; private set; } =
            "<color=yellow>Mode:</color> <color=green>Move Object Only</color>";

        /// <summary>
        /// Gets a text which represents the moving and rotating object mode.
        /// </summary>
        public string ModeMoveAndRotate { get; private set; } =
            "<color=yellow>Mode:</color> <color=green>Move and Rotate Object</color>";

        /// <summary>
        /// Gets a text which represents the rotating object mode.
        /// </summary>
        public string ModeRotateOnly { get; private set; } =
            "<color=yellow>Mode:</color> <color=green>Rotate Object Only</color>";

        /// <summary>
        /// Gets a text which represents the Gravity object mode.
        /// </summary>
        public string ModeNoGravity { get; private set; } =
            "<color=yellow>Mode:</color> <color=green>Gravity and toppling enabled.</color>";

        /// <summary>
        /// Gets a text which represents the No gravity mode object mode.
        /// </summary>
        public string ModeGravity { get; private set; } =
            "<color=yellow>Mode:</color> <color=green>Gravity enabled. No toppling.</color>";

    }
}
