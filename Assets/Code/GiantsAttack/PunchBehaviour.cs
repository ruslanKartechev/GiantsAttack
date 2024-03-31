using System;
using UnityEngine;

namespace GiantsAttack
{
    public class PunchBehaviour
    {
        private IMonster _monster;
        private Action _callback;
        private Action _punchStartCallback;
        
        
        public PunchBehaviour(IMonster monster, Animator animator, string key, Action punchStartCallback, Action callback)
        {
            _monster = monster;
            _callback = callback;
            _punchStartCallback = punchStartCallback;
            _monster.AnimEventReceiver.OnPunch += OnPunch;
            _monster.AnimEventReceiver.OnPunchBegan += OnPunchBegan;
            animator.SetTrigger(key);
        }

        private void OnPunchBegan()
        {
            _punchStartCallback.Invoke();
        }

        private void OnPunch()
        {
            _callback.Invoke();
        }
    }
}