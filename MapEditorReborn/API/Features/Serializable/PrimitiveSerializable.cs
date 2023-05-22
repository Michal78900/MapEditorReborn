// -----------------------------------------------------------------------
// <copyright file="PrimitiveSerializable.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.API.Features.Serializable;

using System;
using UnityEngine;

/// <summary>
/// A tool used to easily handle primitives.
/// </summary>
[Serializable]
public class PrimitiveSerializable : SerializableObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrimitiveSerializable"/> class.
    /// </summary>
    public PrimitiveSerializable()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrimitiveSerializable"/> class.
    /// </summary>
    /// <param name="primitiveType"></param>
    /// <param name="color"></param>
    public PrimitiveSerializable(PrimitiveType primitiveType, string color)
    {
        PrimitiveType = primitiveType;
        Color = color;
    }

    public PrimitiveSerializable(SchematicBlockData block)
    {
        PrimitiveType = (PrimitiveType)Enum.Parse(typeof(PrimitiveType), block.Properties["PrimitiveType"].ToString());
        Color = block.Properties["Color"].ToString();
    }

    /// <summary>
    /// Gets or sets the <see cref="UnityEngine.PrimitiveType"/>.
    /// </summary>
    public PrimitiveType PrimitiveType { get; set; } = PrimitiveType.Cube;

    /// <summary>
    /// Gets or sets the <see cref="PrimitiveSerializable"/>'s color.
    /// </summary>
    public string Color { get; set; } = "red";
}