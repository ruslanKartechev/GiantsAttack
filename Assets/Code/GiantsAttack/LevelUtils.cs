﻿using System;
using GameCore.Core;
using GameCore.UI;
using RaftsWar.Core;
using SleepDev;

namespace GiantsAttack
{
    public class LevelUtils
    {
        private IHelicopter _helicopter;
        private IHitCounter _hitCounter;
        
        public LevelUtils(IHelicopter helicopter, IHitCounter hitCounter)
        {
            _helicopter = helicopter;
            _hitCounter = hitCounter;
        }
        
        public void CallWinScreen(int level, Action nextButtonCallback = null)
        {
            var screen = GCon.UIFactory.GetCompletedMenu() as IMenuWin;
            if (nextButtonCallback == null)
                nextButtonCallback = CallNextLevel;
            screen.Show(level, nextButtonCallback, () =>{});
            screen.ResultPrinter.Text = $"SHOTS:      {_hitCounter.ShotsCount}\n" +
                                        $"HEADSHOTS:  {_hitCounter.HeadShotsCount}\n" +
                                        $"BEST STREAK {_hitCounter.BestStreak}\n" +
                                        $"MISSES:     {_hitCounter.MissCount}";
            screen.ResultPrinter.PrintText();
        }

        public void CallFailScreen(int level, Action nextButtonCallback = null)
        {
            var screen = GCon.UIFactory.GetFailedMenu() as IMenuFail;
            if (nextButtonCallback == null)
                nextButtonCallback = CalLReplayLevel;
            screen.Show(level, nextButtonCallback, () =>{});
        }

        public void SendWinEvent(int level, float playTime, IHitCounter counter)
        {
            Analytics.OnWin(level, "default", playTime, counter.MissCount, counter.HitsCount);
        }

        public void SendFailEvent(int level, float playTime, IHitCounter counter)
        {
            Analytics.OnFailed(level, "default", "fail", playTime, counter.MissCount, counter.HitsCount);
        }

        private void CallNextLevel()
        {
            CLog.Log($"[LevelUtils] On Next level button");
            GameCore.Levels.LevelUtils.CallNextLevel();
        }

        private void CalLReplayLevel()
        {
            CLog.Log($"[LevelUtils] On Replay level button");
            GameCore.Levels.LevelUtils.CallReplay();
        }
    }
}