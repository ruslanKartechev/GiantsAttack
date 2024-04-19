﻿using UnityEngine;

namespace GameCore.UI
{
    public interface IDamageHitsUI
    {
        void ShowHit(Vector3 worldPos, float damage, DamageIndicationType type);
    }
}