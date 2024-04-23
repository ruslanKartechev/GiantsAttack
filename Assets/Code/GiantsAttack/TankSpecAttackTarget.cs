using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public enum SpecAttackStage {
        None, StageStart, AttackStart, Attack, End
    }
    
    public class TankSpecAttackTarget : MonoExtended, ISpecAttackTarget
    {
        [SerializeField] private SpecAttackStage _shootStage;
        [SerializeField] private float _shootDelay;
        [Space(10)]
        [SerializeField] private SpecAttackStage _explodeStage;
        [SerializeField] private float _explodeDelay;
        [Space(10)]
        [SerializeField] private SpecAttackStage _moveStage;
        [SerializeField] private float _moveDelay;
        [Space(10)]
        [SerializeField] private TankShooter _tankShooter;
        [SerializeField] private ExplosiveVehicle _explosive;
        [SerializeField] private SimpleForwardMover _mover;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(_mover == null)
                _mover = GetComponent<SimpleForwardMover>();
            if(_explosive == null)
                _explosive = GetComponent<ExplosiveVehicle>();
            if(_tankShooter == null)
                _tankShooter = GetComponent<TankShooter>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
        
        public void OnStageBegan()
        {
            TryMove(SpecAttackStage.StageStart);
            TryShoot(SpecAttackStage.StageStart);
            TryExplode(SpecAttackStage.StageStart);
        }

        public void OnAttackBegan()
        {
            TryMove(SpecAttackStage.AttackStart);
            TryShoot(SpecAttackStage.AttackStart);
            TryExplode(SpecAttackStage.AttackStart);
        }

        public void OnAttack()
        {   
            TryMove(SpecAttackStage.Attack);
            TryShoot(SpecAttackStage.Attack);
            TryExplode(SpecAttackStage.Attack);
        }

        public void OnCompleted()
        {
            TryMove(SpecAttackStage.End);
            TryShoot(SpecAttackStage.End);
            TryExplode(SpecAttackStage.End);
        }

        private void TryMove(SpecAttackStage stage)
        {
            if(_moveStage == stage)
                Delay(() => { _mover.Move(() => { });}, _moveDelay);
        }
        
        private void TryShoot(SpecAttackStage stage)
        {
            if(_shootStage == stage)
                Delay(_tankShooter.ShootOnce, _shootDelay);
        }
        
        private void TryExplode(SpecAttackStage stage)
        {
            if(_explodeStage == stage)
                Delay(_explosive.Explode, _explodeDelay);
        }
    }
}