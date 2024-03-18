using System;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class TowerViewByLevels : MonoExtended
    {
        [SerializeField] private List<TowerViewPart> _viewParts;
        /// <summary>
        /// Callback used to spawn units after the spawning animation played
        /// </summary>
        public Action OnUpdatedCallback { get; set; }
       
        
        public List<TowerViewPart> Views { get; private set; } = new List<TowerViewPart>(5);

        /// <summary>
        /// SpawnPoints that need to spawn a unit upon upgrade
        /// </summary>
        public List<Transform> ActiveSpawnPoints { get; private set; } = new List<Transform>(5);

        /// <summary>
        /// Points to which a unit can be moved from a raft
        /// </summary>
        public List<Transform> OptionalSpawnPoints { get; private set; } = new List<Transform>(30);
        public int ViewPartIndex { get; private set; }


        public void Init(Team team, IDamageable damageable, 
            IArrowStuckTarget arrowStuckTarget, 
            IDamagePointsProvider damagePointsProvider)
        {
            ViewPartIndex = 0;
            foreach (var view in _viewParts)
            {
                if(view == null)
                    continue;
                view.Team = team;
                view.DamagePointsProvider = damagePointsProvider;
                view.ArrowStuckTarget = arrowStuckTarget;
                view.Damageable = damageable;
            }
        }
        
        public void UpdateToLevel(int level)
        {
            if (level == 0)
                return;
            ViewPartIndex = level-1;
            var current = _viewParts[ViewPartIndex];
            current.OnBuiltCallback = Callback;
            Views.Add(current);
            ActiveSpawnPoints = (current.SpawnPoints);
            OptionalSpawnPoints.AddRange(current.OptionalSpawnPoints);
            current.Show();
        }
        
        
        /// <summary>
        /// Spawn all views up to level immediately
        /// </summary>
        public void SetLevel(int level)
        {
            if (level == 0)
                return;
            ViewPartIndex = level-1;
            for (var i = 0; i <= ViewPartIndex; i++)
            {
                _viewParts[i].ShowNow();
                Views.Add(_viewParts[i]);
                ActiveSpawnPoints.AddRange(_viewParts[i].SpawnPoints);
                OptionalSpawnPoints.AddRange(_viewParts[i].OptionalSpawnPoints);
            }
        }

        public void BreakTower()
        {
            foreach (var part in Views)
                part.Destroy();
        }
        
        private void Callback()
        {
            // CLog.LogYellow($"[Tower {gameObject.name}] View On Animation callback....");
            OnUpdatedCallback.Invoke();
        }


#if UNITY_EDITOR
        public void E_Spawn(int level)
        {
            if (level == 0)
            {
                for (var i = 0; i < 4; i++)
                    _viewParts[i].HideNow();
                return;
            }
            ViewPartIndex = level-1;
            for (var i = 0; i <= ViewPartIndex; i++)
                _viewParts[i].ShowNow();
            for (var i = ViewPartIndex+1; i < 4; i++)
                _viewParts[i].HideNow();
        }
        #endif
    }
}