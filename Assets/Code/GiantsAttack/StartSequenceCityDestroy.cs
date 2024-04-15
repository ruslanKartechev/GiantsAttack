using System;
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
            _ui = Resources.Load<CityDestroyUI>("Prefabs/UI/city_destroy_ui");
            var ui = Instantiate(_ui);
            var count = _stage.GetTotalCount();
            ui.SetCount(count, count);
            ui.Show(() => { });
            _stage.CityUI = ui;
            if (_otherSequence == this)
                return;
            _otherSequence.Enemy = Enemy;
            _otherSequence.Begin(onEnd);
        }
    }
}