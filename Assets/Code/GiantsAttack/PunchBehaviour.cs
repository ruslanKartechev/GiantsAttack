using System;
using UnityEngine;

namespace GiantsAttack
{
    public class PunchBehaviour
    {
        private IMonster _monster;
        private Action _punchMadeCallback;
        private Action _punchStartCallback;
        private Action _onAnimationOver;
        
        public PunchBehaviour(IMonster monster, Animator animator, string key, Action punchStartCallback, Action punchMadeCallback, Action onAnimationOver)
        {
            _monster = monster;
            _punchMadeCallback = punchMadeCallback;
            _punchStartCallback = punchStartCallback;
            _onAnimationOver = onAnimationOver;
            _monster.AnimEventReceiver.EOnPunch += OnPunch;
            _monster.AnimEventReceiver.EOnPunchBegan += OnPunchBegan;
            _monster.AnimEventReceiver.EOnAnimationOver += OnAnimationOver;
            animator.SetTrigger(key);
        }

        private void OnPunchBegan()
        {
            _monster.AnimEventReceiver.EOnPunchBegan -= OnPunchBegan;
            _punchStartCallback.Invoke();
        }

        private void OnPunch()
        {
            _monster.AnimEventReceiver.EOnPunch -= OnPunch;
            _punchMadeCallback.Invoke();
        }

        private void OnAnimationOver()
        {
            _monster.AnimEventReceiver.EOnAnimationOver -= OnAnimationOver;
            _onAnimationOver.Invoke();
        }
    }
}