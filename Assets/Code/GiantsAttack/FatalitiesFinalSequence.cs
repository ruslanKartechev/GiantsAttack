using System;
using System.Collections.Generic;
using GameCore.Core;
using GameCore.UI;
using SleepDev.Sound;
using UnityEngine;

namespace GiantsAttack
{
    public class FatalitiesFinalSequence : LevelFinalSequence
    {
#if UNITY_EDITOR
        [Header("Editor cheat")]
        public byte e_debugInd;
        public bool e_doCheatIndex;
        [Space(10)]        
#endif
        [SerializeField] private List<FatalityData> _fatalityData;
        [SerializeField] private float _playerRotateTime = 1f;
        [SerializeField] private float _playerMoveDelay = .5f;
        [SerializeField] private float _endcallbackDelay = .44f;
        [SerializeField] private float _afterEnemyAnimationDelay;
        [SerializeField] private SoundSo _winSound;
        [SerializeField] private SoundSo _finishHimSound;

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
            _endCallback = callback;
            Player.StopAll();
            PlayerMover.Pause(true);
            Delay(() => {
                    Player.Mover.CenterInternal();
                    Player.Mover.ParentAndMoveLocal(Enemy.KillPoint, _playerRotateTime, 
                    AnimationCurve.EaseInOut(0f, .5f, 1f, 1f), null);
            }, _playerMoveDelay);
            Enemy.PreKillState();
            Delay(OnEnemyAnimated, _afterEnemyAnimationDelay);
            var ui = GCon.UIFactory.GetRouletteUI() as RouletteMenu;
            ui.Show(() => {});
            ui.RouletteUI.OnButtonCallback += OnTypeSelected; 
            ui.RouletteUI.Begin();
            _finishHimSound.Play();
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
#if UNITY_EDITOR
            if (e_doCheatIndex)
                data = _fatalityData[e_debugInd];
#endif
            var prefab = Resources.Load($"Prefabs/Fatalities/{data.prefabId}") as GameObject;
            var inst = Instantiate(prefab, transform.parent);
            var fatality = inst.GetComponent<IFatality>();
            fatality.Init(Player, Enemy);
            fatality.Play(OnFatalityEnd);
        }
        
        private void OnFatalityEnd()
        {
            Invoke(nameof(RaiseCallback), _endcallbackDelay);
            _winSound.Play();
        }
        
        private void RaiseCallback()
        {
            _endCallback.Invoke();
        }

    }
}