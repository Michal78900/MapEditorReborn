namespace MapEditorReborn.Patches
{
#pragma warning disable SA1313
    using API.Enums;
    using API.Extensions;
    using API.Features.Components.ObjectComponents;
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
            switch (eventType)
            {
                case DoorEventOpenerExtension.OpenerEventType.DeconFinish:
                    {
                        break;
                    }

                case DoorEventOpenerExtension.OpenerEventType.WarheadStart:
                    {
                        if (__instance.TargetDoor.TryGetComponent(out DoorObjectComponent doorObjectComponent) && doorObjectComponent.Base.LockOnEvent.HasFlagFast(LockOnEvent.WarheadDetonated))
                        {
                            __instance.TargetDoor.NetworkTargetState = false;
                            __instance.TargetDoor.ServerChangeLock(DoorLockReason.Warhead, false);
                        }

                        break;
                    }
            }
        }
    }
}
