namespace MapEditorReborn.Patches
{
#pragma warning disable SA1313
    using API.Enums;
    using API.Extensions;
    using API.Features.Objects;
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
            if (!__instance.TargetDoor.TryGetComponent(out DoorObject doorObjectComponent))
                return;

            if ((eventType != DoorEventOpenerExtension.OpenerEventType.DeconFinish || !doorObjectComponent.Base.LockOnEvent.HasFlagFast(LockOnEvent.LightDecontaminated)) &&
                (eventType != DoorEventOpenerExtension.OpenerEventType.WarheadStart || !doorObjectComponent.Base.LockOnEvent.HasFlagFast(LockOnEvent.WarheadDetonated)))
                return;

            __instance.TargetDoor.NetworkTargetState = false;
            __instance.TargetDoor.ServerChangeLock(eventType == DoorEventOpenerExtension.OpenerEventType.DeconFinish ? DoorLockReason.DecontLockdown : DoorLockReason.Warhead, false);
        }
    }
}
