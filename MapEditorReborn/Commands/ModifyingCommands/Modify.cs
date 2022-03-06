namespace MapEditorReborn.Commands
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using API.Extensions;
    using API.Features.Objects;
    using CommandSystem;
    using Events.Handlers.Internal;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using UnityEngine;

    using static API.API;

    /// <summary>
    /// Command used for modifing the objects.
    /// </summary>
    public class Modify : ICommand
    {
        /// <inheritdoc/>
        public string Command => "modify";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "mod" };

        /// <inheritdoc/>
        public string Description => "Allows modifying properties of the selected object.";

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
                else
                {
                    ToolGunHandler.SelectObject(player, mapObject);
                }
            }

            TeleportObject teleport = mapObject as TeleportObject;

            object instance = teleport == null ? mapObject.GetType().GetField("Base").GetValue(mapObject) : mapObject.GetType().GetField("Controller").GetValue(teleport).GetType().GetField("Base").GetValue(teleport.Controller);
            List<PropertyInfo> properties = instance.GetType().GetProperties().Where(x => Type.GetTypeCode(x.PropertyType) != TypeCode.Object && !x.Name.Contains("RoomType")).ToList();

            if (arguments.Count == 0)
            {
                response = "\nObject properties:\n\n";

                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType == typeof(bool))
                        response += $"{property.Name}: {((bool)property.GetValue(instance) ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}\n";
                    else
                        response += $"{property.Name}: <color=yellow><b>{property.GetValue(instance)}</b></color>\n";
                }

                if (teleport != null)
                {
                    if (!teleport.IsEntrance)
                        response += $"Chance: <color=yellow><b>{teleport.GetType().GetField("Chance").GetValue(teleport)}</b></color>\n";

                    response += $"\nTo spawn another teleport exit use <color=yellow><b>add</b></color> as an argument";
                }

                return true;
            }

            PropertyInfo foundProperty = properties.FirstOrDefault(x => x.Name.ToLower().Contains(arguments.At(0).ToLower()));

            if (foundProperty == null)
            {
                if (teleport != null)
                {
                    if (arguments.At(0).ToLower() == "add")
                    {
                        teleport.Controller.ExitTeleports.Add(teleport.Controller.CreateTeleporter(player.Position, Vector3.one, Exiled.API.Enums.RoomType.Surface, 100, true));
                        response = $"Teleport exit have been successfully created!";
                        return true;
                    }

                    if (!teleport.IsEntrance && "chance".Contains(arguments.At(0).ToLower()))
                    {
                        teleport.GetType().GetField("Chance").SetValue(teleport, TypeDescriptor.GetConverter(typeof(float)).ConvertFromString(null, CultureInfo.GetCultureInfo("en-US"), arguments.At(1)));
                        response = "You've successfully modified the object!";
                        player.ShowGameObjectHint(mapObject);
                        return true;
                    }
                }

                response = $"There isn't any object property that contains \"{arguments.At(0)}\" in it's name!";
                return false;
            }

            try
            {
                if (foundProperty.PropertyType != typeof(string))
                {
                    foundProperty.SetValue(instance, TypeDescriptor.GetConverter(foundProperty.PropertyType).ConvertFromString(null, CultureInfo.GetCultureInfo("en-US"), arguments.At(1)));
                }
                else
                {
                    string spacedString = arguments.At(1);
                    for (int i = 1; i < arguments.Count - 1; i++)
                    {
                        spacedString += $" {arguments.At(1 + i)}";
                    }

                    foundProperty.SetValue(instance, TypeDescriptor.GetConverter(foundProperty.PropertyType).ConvertFromString(null, CultureInfo.GetCultureInfo("en-US"), spacedString));
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
    }
}
