using GorillaLocomotion;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Bark.Helpers
{
    public class GravityHelper
    {

        static Vector3 dir;
        public static void SetGravity(Vector3 direction)
        {
            dir = direction;
            GTPlayer.Instance.SetGravityOverride(Plugin.Instance, GravityOverride);
        }
        static void GravityOverride(GTPlayer player)
        {
            player.AddForce(dir, ForceMode.Acceleration);
        }

        public static void SetPlayerDirection(Vector3 upDirection)
        {
            Vector3 up = upDirection.normalized;

            Vector3 forward = Vector3.Cross(Vector3.right, up);
            Quaternion rotation = Quaternion.LookRotation(forward, up);

            GTPlayerTransform.ApplyRotationOverride(rotation, Time.frameCount);
        }
    }
}
