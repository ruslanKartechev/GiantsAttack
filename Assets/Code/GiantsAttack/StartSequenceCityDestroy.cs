using System;
using GameCore.Core;
using GameCore.UI;
using UnityEngine;

namespace GiantsAttack
{
    public class StartSequenceCityDestroy : LevelStartSequence
    {
        [SerializeField] private LevelStartSequence _otherSequence;
        [SerializeField] private LevelStageHavok _stage;
        private CityDestroyUI _ui;

#if UNITY_EDITOR
        public override void E_Init()
        { }
#endif
        
        public override void Begin(Action onEnd)
        {
            _ui = ((IGameplayMenu)GCon.UIFactory.GetGameplayMenu()).CityDestroyUI;
            _stage.CityUI = _ui;
            if (_otherSequence == this)
                return;
            _otherSequence.Enemy = Enemy;
            _otherSequence.Begin(onEnd);
        }
    }
}