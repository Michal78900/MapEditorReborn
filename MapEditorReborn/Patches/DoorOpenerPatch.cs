namespace MapEditorReborn.Patches
{
#pragma warning disable SA1313

    using API;
    using HarmonyLib;
    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// Pathes the <see cref="DoorEventOpenerExtension.Trigger(DoorEventOpenerExtension.OpenerEventType)"/> to prevent selected doors from opening, when the Alpha Warhead is activated.
    /// </summary>
    [HarmonyPatch(typeof(DoorEventOpenerExtension), nameof(DoorEventOpenerExtension.Trigger))]
    internal static class DoorOpenerPatch
    {
        private static void Postfix(DoorEventOpenerExtension __instance, ref DoorEventOpenerExtension.OpenerEventType eventType)
        {
            if (eventType == DoorEventOpenerExtension.OpenerEventType.WarheadStart && __instance.TargetDoor.TryGetComponent(out DoorObjectComponent doorObjectComponent) && !doorObjectComponent.Base.OpenOnWarheadActivation)
            {
                __instance.TargetDoor.NetworkTargetState = false;
                __instance.TargetDoor.ServerChangeLock(DoorLockReason.Warhead, false);
            }
        }
    }
}
