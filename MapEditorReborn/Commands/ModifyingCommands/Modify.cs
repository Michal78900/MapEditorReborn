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

    public class Modify : ICommand
    {
        public string Command => "modify";

        public string[] Aliases => new string[] { "mod" };

        public string Description => "Allows modifying properties of the selected object.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission($"mpr.{Command}"))
            {
                response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
                return false;
            }

            // Player player = Player.Get(sender);
            Player player = Player.Get(sender as CommandSender);

            if (!player.TryGetSessionVariable(Handler.SelectedObjectSessionVarName, out MapEditorObject mapEditorObject) || mapEditorObject == null)
            {
                response = "You haven't selected any object!";
                return false;
            }

            object instance = mapEditorObject.GetType().GetField("Base").GetValue(mapEditorObject);
            List<PropertyInfo> properties = instance.GetType().GetProperties().Where(x => Type.GetTypeCode(x.PropertyType) != TypeCode.Object && x.Name != "RoomType").ToList();

            if (arguments.Count == 0)
            {
                response = "\nObject properties:\n\n";

                foreach (PropertyInfo property in properties)
                {
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

            foundProperty.SetValue(instance, TypeDescriptor.GetConverter(foundProperty.PropertyType).ConvertFromString(arguments.At(1)));
            mapEditorObject.UpdateObject();

            response = "funi";
            return true;
        }
    }
}
