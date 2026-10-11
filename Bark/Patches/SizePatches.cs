using System;
using Bark.Modules.Physics;
using Bark.Tools;
using Bark.Extensions;
using HarmonyLib;
using UnityEngine;

namespace Bark.Patches;

[HarmonyPatch(typeof(SizeManager))]
[HarmonyPatch(nameof(SizeManager.ControllingChanger), MethodType.Normal)]
public class SizeChangePatch
{
    private static void Postfix(ref SizeChanger __result, Transform t)
    {
        try
        {
            if (Potions.active && t == GorillaTagger.Instance.offlineVRRig.transform)
                __result = Potions.sizeChanger;
            else if
            (
                !(Potions.ShowNetworkedSizes is null) &&
                Potions.ShowNetworkedSizes.Value &&
                t.GetComponentInParent<VRRig>() is VRRig rig &&
                rig.ModuleEnabled(Potions.DisplayName)
            )
                Potions.TryGetSizeChangerForRig(rig, out __result);
        }
        catch (Exception e)
        {
            Logging.Exception(e);
        }
    }
}