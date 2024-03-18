using System;
using SleepDev;

namespace RaftsWar.UI
{
    public class TowerProgressCalculator
    {
        private TowerProgressUI _ui;
        private Action _callback;
        private int _actionCode;
        private float _val1;
        private float _val2;
        private SpriteFillIcon _icon;

        public TowerProgressCalculator(float addedAmount, TowerProgressUI ui, Action callback)
        {
            _ui = ui;
            _callback = callback;
            var repo = GCon.TowerRepository;
            _icon = repo.NextTowerSprite();
            if (repo.CanUpgrade() == false)
            {
                CLog.Log($"[TowerProgress] Max upgrade reached");
                ui.SetCurrentIconAndFill(_icon, 1f);
                _actionCode = 0;
                return;
            }

            _val1 = repo.Progress;
            _val2 = _val1 + addedAmount;
            if (_val2 >= 1f)
            {
                _val2 = 1f;
                _actionCode = 2;
                repo.Upgrade();
            }
            else
            {
                _actionCode = 1;
                repo.Progress = _val2;
            }
            ui.SetCurrentIconAndFill(_icon, repo.Progress);
        }

        public void Play()
        {
            switch (_actionCode)
            {
                case 0:
                    Callback();
                    break;
                case 1:
                    _ui.AnimateProgress(_val1, _val2, _icon, Callback);
                    break;
                case 2:
                    _ui.AnimateProgress(_val1, _val2, _icon, UpgradeAnimate);
                    break;
            }
        }

        private void UpgradeAnimate()
        {
            _ui.AnimateUpgrade();
            Callback();
        }

        private void Callback()
        {
            _callback?.Invoke();
        }
        
        /// <summary>
        /// Returns false if no more upgrades are available
        /// </summary>
        public static bool AddProgressAndUI(float amount, TowerProgressUI ui, Action callback)
        {
            var repo = GCon.TowerRepository;
            if (repo.CanUpgrade() == false)
            {
                ui.SetCurrentIconAndFill(repo.NextTowerSprite(), 1f);
                return false;
            }
            var val1 = repo.Progress;
            var val2 = repo.Progress + amount;
            var sprite = repo.NextTowerSprite();
            if (val2 >= 1f)
            {
                CLog.LogBlue($"[TowerUpgradeCalculator] progress >= 1 calling upgrade");
                repo.Upgrade();
            }
            ui.AnimateProgress(val1, val2, sprite, callback);
            return true;
        }

        /// <summary>
        /// returns 0 if no upgrade possible
        /// returns 1 if fill is possible
        /// returns 2 if after progress tower WILL be upgraded
        /// </summary>
        /// <param name="ui"></param>
        /// <returns></returns>
        public static int SetupProgressUI(float addedProgress, TowerProgressUI ui)
        {
            var repo = GCon.TowerRepository;
            if (repo.CanUpgrade() == false)
            {
                ui.SetCurrentIconAndFill(repo.NextTowerSprite(), 1f);
                return 0;
            }
            ui.SetCurrentIconAndFill(repo.NextTowerSprite(), repo.Progress);
            return 1;
            
        }
    }
}