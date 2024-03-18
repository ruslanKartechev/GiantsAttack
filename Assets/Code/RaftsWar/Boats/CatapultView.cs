using DG.Tweening;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class CatapultView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private SpinAnimator _spinAnimator;
        [SerializeField] private CatapultAnimator _animator;
        [SerializeField] private Transform _orientation;
        [SerializeField] private Transform _shootFrom;
        [SerializeField] private CatapultMagazine _catapultMagazine;

        public CatapultMagazine CatapultMagazine => _catapultMagazine;
        public Transform ShootFrom => _shootFrom;
        public Transform Orientation => _orientation;
        public CatapultAnimator Animator => _animator;
        
        
        public void Init(Team team)
        {
            _renderer.sharedMaterials = team.CatapultViewSettings.materials.ToArray();
        }

        public void AnimatePop()
        {
            var tr = transform;
            tr.localScale = Vector3.zero;
            tr.DOScale(Vector3.one, .33f);
            _spinAnimator.Spin();
        }
        
    }
}