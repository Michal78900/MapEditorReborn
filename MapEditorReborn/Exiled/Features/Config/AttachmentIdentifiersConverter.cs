// -----------------------------------------------------------------------
// <copyright file="AttachmentIdentifiersConverter.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using InventorySystem.Items.Firearms.Attachments;
using MapEditorReborn.Exiled.Structs;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MapEditorReborn.Exiled.Features.Config
{

    /// <summary>
    /// Converts a <see cref="IEnumerable{T}"/> of <see cref="AttachmentName"/> to Yaml configs and vice versa.
    /// </summary>
    public sealed class AttachmentIdentifiersConverter : IYamlTypeConverter
    {
        /// <inheritdoc/>
        public bool Accepts(Type type) => type == typeof(AttachmentName);

        /// <inheritdoc/>
        public object ReadYaml(IParser parser, Type type)
        {
            if (!parser.TryConsume(out Scalar scalar) || !AttachmentIdentifier.TryParse(scalar.Value, out AttachmentName name))
                throw new InvalidDataException($"Invalid AttachmentNameTranslation value: {scalar.Value}.");

            return Enum.Parse(type, name.ToString());
        }

        /// <inheritdoc/>
        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            AttachmentName name = default;

            if (value is AttachmentName locAttachment)
                name = locAttachment;

            emitter.Emit(new Scalar(name.ToString()));
        }
    }
}