using System;
using System.Collections.Generic;
using UnityEngine;

namespace GiantsAttack
{
    public interface IHelicopterGun
    {
        List<HelicopterGunBarrel> Barrels { get; }
        Transform Rotatable { get; }
        void PlayGunsInstallAnimation();
        void PlayReload(Action OnReloaded);
        void StopAnimations();
    }
}