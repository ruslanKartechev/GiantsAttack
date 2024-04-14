using System;
using System.Collections.Generic;
using GameCore.UI;
using SleepDev;
using SleepDev.SlowMotion;
using UnityEngine;

namespace GiantsAttack
{    
    public enum ActionType
    {
        LegKick, BreakBuilding, Toss,
        EvadeThrown, ShootDownThrown, AOE
    }
        
    public enum AnimationType { Move, Animate, None }
    
    [System.Serializable]
    public class SubStage : MonoBehaviour
    {
        [Header("Start Delay")]
        public float delayBeforeStart;
        [Header("Action Type")]
        public ActionType actionType;
        public int targetsCount = 1;
        public bool fromTop;
        [Header("Enemy Movement")]
        public bool doMoveEnemy = true;
        public float enemyMoveTime = 1f;
        public Transform enemyMoveToPoint;
        [Header("Interaction Target")]
        public string enemyAnimation = "-";
        public GameObject enemyTarget;
        [Header("Force, Toss distance, FlyTime")]
        public float forceVal = 100f;
        [Header("Rotation before action")] 
        public bool doRotateBeforeAction;
        public float rotationTime;
        public Transform rotationSource;
        [Header("Target Animation")]
        public bool doAnimateTarget;
        public float delayBeforeAnimateTarget;
        public AnimationType targetAnimationType;
        [Header("Evasion")] 
        public CorrectSwipeChecker swipeChecker;
        public float evadeDistance;
        [Header("Evasion")] 
        public bool doSlowMo;
        public SlowMotionEffectSO slowMotionEffect;
        [Header("StageListener")] 
        public List<StageListener> stageListeners;

        public SubStageExecutor GetExecutor(IMonster enemy, IHelicopter player, IPlayerMover playerMover,
            IGameplayMenu menu, IDestroyedTargetsCounter counter, 
            Action<Action, float> delayDelegate, Action callback, Action failCallback)
        {
            CLog.Log($"[SubStage] action type {actionType.ToString()}");
            switch (actionType)
            {
                case ActionType.LegKick:
                    return new SubStageExecutorLegKick(this, enemy, player, playerMover, menu,counter, delayDelegate, callback, failCallback);
                case ActionType.Toss:
                    return new SubStageExecutorToss(this, enemy, player, playerMover, menu,counter, delayDelegate, callback, failCallback);
                case ActionType.BreakBuilding:
                    return new SubStageExecutorBreakBuilding(this, enemy, player, playerMover, menu,counter, delayDelegate, callback, failCallback);
                case ActionType.EvadeThrown:
                    return new SubStageExecutorEvade(this, enemy, player, playerMover, menu,counter, delayDelegate, callback, failCallback);
                case ActionType.ShootDownThrown:
                    return new SubStageExecutorShootDown(this, enemy, player, playerMover, menu, counter, delayDelegate, callback, failCallback);
                case ActionType.AOE:
                    return new SubStageExecutorAOE(this, enemy, player, playerMover, menu, counter, delayDelegate, callback, failCallback);
            }
            return null;
        }
    }
}