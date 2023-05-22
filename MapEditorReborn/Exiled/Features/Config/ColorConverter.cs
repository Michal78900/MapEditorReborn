﻿// -----------------------------------------------------------------------
// <copyright file="ColorConverter.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NorthwoodLib.Pools;
using UnityEngine;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

/// <summary>
/// Converts <see cref="Color"/> to Yaml configs and vice versa.
/// </summary>
public sealed class ColorConverter : IYamlTypeConverter
{
    /// <inheritdoc />
    public bool Accepts(Type type) => type == typeof(Color);

    /// <inheritdoc />
    public object ReadYaml(IParser parser, Type type)
    {
        if (!parser.TryConsume<MappingStart>(out _))
            throw new InvalidDataException($"Cannot deserialize object of type {type.FullName}");

        List<object> coordinates = ListPool<object>.Shared.Rent(4);
        int i = 0;

        while (!parser.TryConsume<MappingEnd>(out _))
        {
            if (i++ % 2 == 0)
            {
                parser.MoveNext();
                continue;
            }

            if (!parser.TryConsume(out Scalar scalar) || !float.TryParse(scalar.Value, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out float coordinate))
            {
                ListPool<object>.Shared.Return(coordinates);
                throw new InvalidDataException("Invalid float value.");
            }

            coordinates.Add(coordinate);
        }

        object color = Activator.CreateInstance(type, coordinates.ToArray());

        ListPool<object>.Shared.Return(coordinates);

        return color;
    }

    /// <inheritdoc />
    public void WriteYaml(IEmitter emitter, object value, Type type)
    {
        Dictionary<string, float> coordinates = new();

        if (value is Color color)
        {
            coordinates["r"] = color.r;
            coordinates["g"] = color.g;
            coordinates["b"] = color.b;
            coordinates["a"] = color.a;
        }

        emitter.Emit(new MappingStart());

        foreach (KeyValuePair<string, float> coordinate in coordinates)
        {
            emitter.Emit(new Scalar(coordinate.Key));
            emitter.Emit(new Scalar(coordinate.Value.ToString(CultureInfo.GetCultureInfo("en-US"))));
        }
        
        emitter.Emit(new MappingEnd());
    }
}