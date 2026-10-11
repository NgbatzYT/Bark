// Stolen from https://github.com/Graicc/PracticeMod/blob/617a9f758077ea06cf0407a776580d6b021bcc35/PracticeMod/Patches/PlayerTeleportPatch.cs#L61
// Used without permission, but what are you gonna do, sue me?

using Bark.Modules.Physics;
using Bark.Tools;
using GorillaLocomotion;
using HarmonyLib;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Player = GorillaLocomotion.GTPlayer;

namespace Bark.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.AntiTeleportTechnology))]
internal class PlayerAntiAntiTeleport
{
    private static bool Prefix()
    {
        return true;
    }
}

[HarmonyPatch(typeof(Player))]
[HarmonyPatch(nameof(Player.LateUpdate), MethodType.Normal)]
internal class TeleportPatch
{
    private static bool _isTeleporting,
        _rotate;

    private static Vector3 _teleportPosition;
    private static float _teleportRotation;
    private static bool _killVelocity;

    [HarmonyPostfix]
    public static void Postfix(Player __instance)
    {
        try
        {


            if (_isTeleporting)
            {
                __instance.TeleportTo(_teleportPosition, Quaternion.Euler(0, _teleportRotation, 0), !_killVelocity);
                /*Player.Instance.locomotionEnabledLayers = NoClip.layerMask;
                Player.Instance.bodyCollider.isTrigger = true;
                Player.Instance.headCollider.isTrigger = true;
                var playerRigidBody = __instance.GetComponent<Rigidbody>();
                if (playerRigidBody != null)
                {
                    var correctedPosition = _teleportPosition - __instance.bodyCollider.transform.position + __instance.transform.position;

                    if (_killVelocity)
                        playerRigidBody.linearVelocity = Vector3.zero;

                    __instance.transform.position = correctedPosition;
                    if (_rotate)
                        __instance.transform.rotation = Quaternion.Euler(0, _teleportRotation, 0);


                    Traverse.Create(__instance).Field("LastLeftHandPosition").SetValue(__instance.leftHand.handFollower.transform.position);
                    Traverse.Create(__instance).Field("LastRightHandPosition").SetValue(__instance.rightHand.handFollower.transform.position);
                    Traverse.Create(__instance).Field("lastPosition").SetValue(correctedPosition);
                    Traverse.Create(__instance).Field("LastPosition").SetValue(correctedPosition);
                    Traverse.Create(__instance).Field("lastOpenHeadPosition").SetValue(__instance.headCollider.transform.position);

                    GorillaTagger.Instance.offlineVRRig.transform.position = correctedPosition;
                }

                Player.Instance.headCollider.isTrigger = NoClip.Instance.baseHeadIsTrigger;
                Player.Instance.bodyCollider.isTrigger = NoClip.Instance.baseBodyIsTrigger;
                Task.Delay(250).ContinueWith(delegate
                {
                    Player.Instance.locomotionEnabledLayers = NoClip.Instance.baseMask;
                });*/
                _isTeleporting = false;
            }
        }
        catch (Exception e)
        {
            Logging.Exception(e);
        }
    }

    internal static void TeleportPlayer(Vector3 destinationPosition, float destinationRotation, bool killVelocity = true)
    {
        if (_isTeleporting)
            return;
        _killVelocity = killVelocity;
        _teleportPosition = destinationPosition;
        _teleportRotation = destinationRotation;
        _isTeleporting = true;
        _rotate = true;
    }

    internal static void TeleportPlayer(Vector3 destinationPosition, bool killVelocity = true)
    {
        if (_isTeleporting)
            return;

        _killVelocity = killVelocity;
        _teleportPosition = destinationPosition;
        _isTeleporting = true;
        _rotate = false;
    }
}