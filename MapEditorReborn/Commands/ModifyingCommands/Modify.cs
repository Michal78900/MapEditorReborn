namespace MapEditorReborn.Commands
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using API;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;

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
            if (!player.TryGetSessionVariable(Handler.SelectedObjectSessionVarName, out MapEditorObject mapObject) || mapObject == null)
            {
                if (!Handler.TryGetMapObject(player, out mapObject))
                {
                    response = "You haven't selected any object!";
                    return false;
                }
                else
                {
                    Handler.SelectObject(player, mapObject);
                }
            }

            object instance = mapObject.GetType().GetField("Base").GetValue(mapObject);
            List<PropertyInfo> properties = instance.GetType().GetProperties().Where(x => Type.GetTypeCode(x.PropertyType) != TypeCode.Object && x.Name != "RoomType").ToList();

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

                return true;
            }

            PropertyInfo foundProperty = properties.FirstOrDefault(x => x.Name.ToLower().Contains(arguments.At(0).ToLower()));

            if (foundProperty == null)
            {
                response = $"There isn't any object property that contains \"{arguments.At(0)}\" in it's name!";
                return false;
            }

            try
            {
                foundProperty.SetValue(instance, TypeDescriptor.GetConverter(foundProperty.PropertyType).ConvertFromString(arguments.At(1)));
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
