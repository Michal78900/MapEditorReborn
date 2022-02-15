namespace MapEditorReborn.API.Extensions
{
    using Enums;

    public static class EnumExtensions
    {
        public static bool HasFlagFast(this TeleportFlags value, TeleportFlags flag) => (value & flag) == flag;

        public static bool HasFlagFast(this LockOnEvent value, LockOnEvent flag) => (value & flag) == flag;
    }
}
