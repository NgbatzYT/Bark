using GorillaLocomotion;
using UnityEngine;

namespace Bark.Patches;

internal class TeleportPatch
{
    internal static void TeleportPlayer(Vector3 destinationPosition, float destinationRotation, bool killVelocity = true)
        => GTPlayer.Instance.TeleportTo(destinationPosition, Quaternion.Euler(0, destinationRotation, 0), !killVelocity);

    internal static void TeleportPlayer(Vector3 destinationPosition, bool killVelocity = true)
        => GTPlayer.Instance.TeleportTo(destinationPosition, Quaternion.Euler(0, GTPlayer.Instance.headCollider.transform.rotation.eulerAngles.y, 0), !killVelocity);
}