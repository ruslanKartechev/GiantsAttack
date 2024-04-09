using System;
using System.Collections.Generic;
using GameCore.Core;
using GameCore.UI;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class RockerFinalSequence : LevelFinalSequence
    {
        public byte e_debugInd;
        [SerializeField] private List<FatalityData> _fatalityData;
        [SerializeField] private float _playerRotateTime = 1f;
        [SerializeField] private float _enemyRotTime = .33f;
        [SerializeField] private float _endcallbackDelay = .44f;
        [SerializeField] private float _afterEnemyAnimationDelay;
        [SerializeField] private float _lookAtEnemyUpOffset = 20;
        
        private Action _endCallback;
        private GameObject _camPointsParent;
        private bool _enemyAnimated;
        /// <summary>
        /// value = 2 => ready
        /// </summary>
        private byte _readyStage;

        [System.Serializable]
        private class FatalityData
        {
            public string uiId;
            public string prefabId;
            public FatalityType type;
        }
        
#if UNITY_EDITOR
        public override void E_Init()
        { }
#endif

        public override void Begin(Action callback)
        {
            Player.StopAll();
            PlayerMover.Pause(true);
            Player.Mover.RotateToLook(Enemy.Point.position + Vector3.up * _lookAtEnemyUpOffset, _playerRotateTime, () => {});
            _endCallback = callback;
            Enemy.PreKillState();
            Enemy.Mover.RotateToLookAt(Player.Point, _enemyRotTime, () => {});
            Delay(OnEnemyAnimated, _afterEnemyAnimationDelay);
            var ui = GCon.UIFactory.GetRouletteUI() as RouletteMenu;
            ui.Show(() => {});
            ui.RouletteUI.OnButtonCallback += OnTypeSelected; 
            ui.RouletteUI.Begin();
        }

        private void OnTypeSelected()
        {
            _readyStage += 1;
            var ui = ((RouletteMenu)GCon.UIFactory.GetRouletteUI());
            ui.Hide(() => {});
            if(_readyStage == 2)
                StartFatality();
        }
        
        private void OnEnemyAnimated()
        {
            _readyStage += 1;
            if(_readyStage == 2)
                StartFatality();
        }

        private void StartFatality()
        {
            var ui = ((RouletteMenu)GCon.UIFactory.GetRouletteUI()).RouletteUI;
            var data = _fatalityData.Find(t => t.uiId == ui.CurrentSectionGO.name);
            // ==========
            data = _fatalityData[e_debugInd];
            // ==========
            var prefab = Resources.Load($"Prefabs/Fatalities/{data.prefabId}") as GameObject;
            var inst = Instantiate(prefab, transform.parent);
            var fatality = inst.GetComponent<IFatality>();
            fatality.Init(Player, Enemy);
            fatality.Play(OnFatalityEnd);
        }
        
        private void OnFatalityEnd()
        {
            Invoke(nameof(RaiseCallback), _endcallbackDelay);
        }
        
        private void RaiseCallback()
        {
            _endCallback.Invoke();
        }

    }
}