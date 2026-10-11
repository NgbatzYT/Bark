using Bark.GUI;
using Bark.Networking;
using Bark.Patches;
using GorillaLocomotion;
using Bark.Extensions;
using UnityEngine;
using GorillaTag.Gravity;
using Bark.Helpers;

namespace Bark.Modules.Physics;

public class UpsideDown : BarkModule
{
    private Vector3 baseGravity;
    private Quaternion baseRotation;
    
    
    protected override void Cleanup()
    {
        GravityHelper.SetGravity(baseGravity);
        GravityHelper.SetPlayerDirection(Vector3.up);
        
        Plugin.MenuController?.GetComponent<LowGravity>().button.RemoveBlocker(ButtonController.Blocker.MOD_INCOMPAT);
    }

    private void Awake()
    {
        baseGravity = new Vector3(0, -9.81f, 0);
    }

    protected override void OnEnable()
    {
        if (!MenuController.Instance.Built)
            return;
        
        base.OnEnable();

        GravityHelper.SetGravity(-baseGravity);
        GravityHelper.SetPlayerDirection(Vector3.down);

        Plugin.MenuController?.GetComponent<LowGravity>().button.AddBlocker(ButtonController.Blocker.MOD_INCOMPAT);
    }

    public override string GetDisplayName() => "Upside Down";
    public override string Tutorial() => " - Puts you upside down.\n - There is no safeguard! If you fall out of the map, you are done for!";
}