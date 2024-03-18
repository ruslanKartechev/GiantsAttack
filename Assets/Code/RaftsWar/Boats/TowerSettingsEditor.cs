using System;
using System.Collections.Generic;
using UnityEngine;

namespace RaftsWar.Boats
{
    [CreateAssetMenu(menuName = "SO/TowerSettingsEditor", fileName = "TowerSettingsEditor", order = 0)]
    public class TowerSettingsEditor : ScriptableObject
    {
        #if UNITY_EDITOR
        public TowerSettings commonSettings;
        public List<TowerSettingsSo> settings;

         
        [ContextMenu("CopyAll")]
        public void SetAll()
        {
            CallPerLevel((target, common) =>
            {
                target.radius = common.radius;
                target.fireRate = common.fireRate;
                target.unitDamage = common.unitDamage;
                target.buildingSettings = common.buildingSettings;

            });
        }
        
        [ContextMenu("SetRadiusAll")]
        public void SetRadiusAll()
        {
            CallPerLevel((target, common) =>
            {
                target.radius = common.radius;
            });
        }
        
        [ContextMenu("SetFireRateAll")]
        public void SetFireRateAll()
        {
            CallPerLevel((target, common) =>
            {
                target.fireRate = common.fireRate;
            });
        }
        
        [ContextMenu("SetDamageAll")]
        public void SetDamageAll()
        {
            CallPerLevel((target, common) =>
            {
                target.unitDamage = common.unitDamage;
            });
        }
        
        [ContextMenu("SetTowerHealthAll")]
        public void SetTowerHealthAll()
        {
            CallPerLevel((target, common) =>
            {
                target.towerHealth = common.towerHealth;
            });
        }

        
        [ContextMenu("SetUpgradePointsAll")]
        public void SetUpgradePointsAll()
        {
            CallPerLevel((target, common) =>
            {
                target.buildingSettings = common.buildingSettings;
            });
        }


        /// <summary>
        ///  First arg = target settings, second = my common settings
        /// </summary>
        private void CallPerLevel(Action<TowerLevelSettings, TowerLevelSettings> callback)
        {
            foreach (var so in settings)
            {
                var target = so.settings.levelSettings;
                var common = commonSettings.levelSettings;
                if (common.Count > target.Count)
                {
                    var diff = common.Count - target.Count;
                    for (var i = 0; i < diff; i++)
                        target.Add(new TowerLevelSettings());
                }
                else if (common.Count < target.Count)
                {
                    var diff = target.Count - common.Count;
                    for (var i = 0; i < diff && target.Count >= 0; i++)
                    {
                        var ind = target.Count - 1;
                        target.RemoveAt(ind);
                    }
                }

                for (var i = 0; i < common.Count; i++)
                {
                    callback.Invoke(target[i], common[i]);
                }

                UnityEditor.EditorUtility.SetDirty(so);
            }
        }
#endif
    }
}