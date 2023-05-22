// -----------------------------------------------------------------------
// <copyright file="IPosition.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using UnityEngine;

namespace MapEditorReborn.Exiled.Interfaces;

/// <summary>
/// Represents an object with a <see cref="Vector3"/> position.
/// </summary>
public interface IPosition
{
    /// <summary>
    /// Gets the position of this object.
    /// </summary>
    public Vector3 Position { get; }
}