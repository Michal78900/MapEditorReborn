// -----------------------------------------------------------------------
// <copyright file="Modify.cs" company="MapEditorReborn">
// Copyright (c) MapEditorReborn. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace MapEditorReborn.Commands.ModifyingCommands
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using API.Extensions;
    using API.Features.Objects;
    using API.Features.Serializable;
    using CommandSystem;
    using Events.Handlers.Internal;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using YamlDotNet.Serialization;
    using static API.API;

    /// <summary>
    /// Command used for modifying the objects.
    /// </summary>
    public class Modify : ICommand
    {
        /// <inheritdoc/>
        public string Command => "modify";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "mod" };

        /// <inheritdoc/>
        public string Description => "Allows modifying properties of the selected object.";

        /// <inheritdoc/>
        public bool SanitizeResponse => false;

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            Player player = Player.Get(sender);
            if (!player.TryGetSessionVariable(SelectedObjectSessionVarName, out MapEditorObject mapObject) || mapObject == null)
            {
                if (!ToolGunHandler.TryGetMapObject(player, out mapObject))
                {
                    response = "You haven't selected any object!";
                    return false;
                }

                ToolGunHandler.SelectObject(player, mapObject);
            }

            object instance = mapObject.GetType().GetField("Base").GetValue(mapObject);
            List<PropertyInfo> properties = instance.GetType().GetProperties().Where(x => (Type.GetTypeCode(x.PropertyType) != TypeCode.Object || x.PropertyType == typeof(float?)) && !x.Name.Contains("RoomType")).ToList();

            if (arguments.Count == 0)
            {
                response = "\nObject properties:\n\n";

                foreach (PropertyInfo property in properties)
                {
                    if (Attribute.IsDefined(property, typeof(YamlIgnoreAttribute)))
                        continue;

                    if (property.PropertyType == typeof(bool))
                        response += $"{property.Name}: {((bool)property.GetValue(instance) ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}\n";
                    else
                        response += $"{property.Name}: <color=yellow><b>{property.GetValue(instance) ?? "NULL"}</b></color>\n";
                }

                // Show extra teleport properties.
                if (mapObject is TeleportObject teleport)
                    HandleTeleport(teleport, arguments, player, ref response);

                return true;
            }

            PropertyInfo foundProperty = properties.FirstOrDefault(x => x.Name.ToLower().Contains(arguments.At(0).ToLower()));

            if (foundProperty == null)
            {
                // Handle extra teleport properties.
                if (mapObject is TeleportObject teleport)
                {
                    response = string.Empty;
                    return HandleTeleport(teleport, arguments, player, ref response);
                }

                response = $"There isn't any object property that contains \"{arguments.At(0)}\" in it's name!";
                return false;
            }

            try
            {
                if (foundProperty.PropertyType != typeof(string))
                {
                    object value;
                    try
                    {
                        value = TypeDescriptor.GetConverter(foundProperty.PropertyType).ConvertFromInvariantString(arguments.At(1));
                    }
                    catch (Exception)
                    {
                        if (arguments.At(1).ToLower().Contains("null") && foundProperty.PropertyType == typeof(float?))
                            value = null;
                        else
                            throw new Exception();
                    }

                    foundProperty.SetValue(instance, value);
                }
                else
                {
                    string spacedString = arguments.At(1);
                    for (int i = 1; i < arguments.Count - 1; i++)
                    {
                        spacedString += $" {arguments.At(1 + i)}";
                    }

                    foundProperty.SetValue(instance, TypeDescriptor.GetConverter(foundProperty.PropertyType).ConvertFromInvariantString(spacedString));
                }
            }
            catch (Exception)
            {
                response = $"\"{arguments.At(1)}\" is not a valid argument! The value should be a {foundProperty.PropertyType} type.";
                return false;
            }

            mapObject.UpdateObject();
            mapObject.UpdateIndicator();
            player.ShowGameObjectHint(mapObject);

            response = "You've successfully modified the object!";
            return true;
        }

        private bool HandleTeleport(TeleportObject teleport, ArraySegment<string> arguments, Player player, ref string response)
        {
            if (arguments.Count == 0)
            {
                response += "\n<b>Target Teleporters:</b>";
                foreach (TargetTeleporter targetTeleporter in teleport.Base.TargetTeleporters)
                {
                    response += $"\n- Teleporter Id: <color=yellow><b>{targetTeleporter.Id}</b></color>\n" +
                                $"  Chance: <color=yellow><b>{targetTeleporter.Chance}</b></color>";
                }

                return true;
            }

            if (arguments.Count < 2)
            {
                response = "You need to specify at least 2 arguments!";
                return false;
            }

            switch (arguments.At(0).ToLower())
            {
                case "add":
                    {
                        if (!int.TryParse(arguments.At(1), out int id) || id < 0)
                        {
                            response = "You need to provide a valid teleport id!";
                            return false;
                        }

                        if (teleport.Base.TargetTeleporters.Select(x => x.Id).Contains(id))
                        {
                            response = "This teleport id is already in use!";
                            return false;
                        }

                        teleport.Base.TargetTeleporters.Add(new TargetTeleporter(id, 100f));

                        response = "You've successfully added a new target teleport!";
                        if (SpawnedObjects.FirstOrDefault(x => x is TeleportObject teleportObject && teleportObject.Base.ObjectId == id) is null)
                            response = $"<i>Teleporter with {id} is currently not present on the map.</i>\n" + response;

                        break;
                    }

                case "delete":
                case "remove":
                    {
                        if (!int.TryParse(arguments.At(1), out int id) || id < 0)
                        {
                            response = "You need to provide a valid teleport id!";
                            return false;
                        }

                        if (!teleport.Base.TargetTeleporters.Select(x => x.Id).Contains(id))
                        {
                            response = "This teleport id doesn't exist!";
                            return false;
                        }

                        teleport.Base.TargetTeleporters.Remove(teleport.Base.TargetTeleporters.First(x => x.Id == id));

                        response = "You've successfully removed the target teleport!";
                        break;
                    }

                default:
                    {
                        if (!int.TryParse(arguments.At(0), out int id) || id < 0)
                        {
                            response = "You need to provide a valid teleport id!";
                            return false;
                        }

                        if (!teleport.Base.TargetTeleporters.Select(x => x.Id).Contains(id))
                        {
                            response = "This teleport id doesn't exist!";
                            return false;
                        }

                        // Change if I add more properties in the future.
                        if (!arguments.At(1).ToLower().Contains("chance"))
                        {
                            response = "You need to provide a valid property name!";
                            return false;
                        }

                        if (!arguments.At(2).TryParseToFloat(out float chance) || chance <= 0)
                        {
                            response = "You need to provide a valid chance value!";
                            return false;
                        }

                        teleport.Base.TargetTeleporters.First(x => x.Id == id).Chance = chance;

                        response = "You've successfully modified the target teleport!";
                        break;
                    }
            }

            teleport.UpdateObject();
            teleport.UpdateIndicator();
            player.ShowGameObjectHint(teleport);

            return true;
        }
    }
}
