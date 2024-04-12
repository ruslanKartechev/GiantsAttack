using System;
using UnityEngine;

namespace GiantsAttack
{
    public class StartSequenceCityDestroy : LevelStartSequence
    {
        [SerializeField] private LevelStartSequence _otherSequence;
        [SerializeField] private LevelStageHavok _stage;
        [SerializeField] private CityDestroyUI _ui;

#if UNITY_EDITOR
        public override void E_Init()
        { }
#endif
        
        public override void Begin(Action onEnd)
        {
            var ui = Instantiate(_ui);
            var count = _stage.GetTotalCount();
            ui.SetCount(count, count);
            ui.Show(() => { });
            _stage.CityUI = ui;
            _otherSequence.Enemy = Enemy;
            _otherSequence.Begin(onEnd);
        }
    }
}