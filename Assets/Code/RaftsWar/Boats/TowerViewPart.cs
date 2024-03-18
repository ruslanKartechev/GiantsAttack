using System;
using System.Collections.Generic;
using DG.Tweening;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class TowerViewPart : MonoExtended, ITarget
    {
        private static bool _animNo2;
        
        [SerializeField] private bool _useAnimation = true;
        [SerializeField] private bool _simpleDestroy = false;
        [SerializeField] private float _callbackDelay = .5f;
        [SerializeField] private Animator _animator;
        [SerializeField] private List<Transform> _spawnPoints;
        [SerializeField] private List<Transform> _optionalSpawnPoints;
        [SerializeField] private List<Transform> _damagePoints;
        [SerializeField] private SinkingPartsController _sinkingPartsController;
        [SerializeField] private ByPartsDestroyer _destroyer;
        [Space(10)]
        [SerializeField] private List<MeshRenderer> _colorTargets;
        [SerializeField] private int _materialIndex;
        private Team _team;

        public Action OnBuiltCallback;
        public List<Transform> DamagePoints => _damagePoints;
        public List<Transform> SpawnPoints => _spawnPoints;
        public List<Transform> OptionalSpawnPoints => _optionalSpawnPoints;
        
        public Team Team
        {
            get => _team;
            set
            {
                _team = value;
                foreach (var target in _colorTargets)
                {
                    var mats = target.sharedMaterials;
                    mats[_materialIndex] = _team.BoatView.SideMaterial[0];
                    target.sharedMaterials = mats;
                }
            } 
        }
        
        public Transform Point => transform;
        public IDamageable Damageable { get; set; }
        public IArrowStuckTarget ArrowStuckTarget { get; set; }
        public IDamagePointsProvider DamagePointsProvider { get; set; }
        
        
        public void Show()
        {
            gameObject.SetActive(true);
            if (_useAnimation)
            {
                _animator.enabled = true;
                // var anim = _animNo2 ? "PopUp_2" : "PopUp";
                // _animNo2 = !_animNo2;
                _animator.Play("PopUp");
                if(OnBuiltCallback != null)
                    Delay(OnBuiltCallback, _callbackDelay);
            }
            else
            {
                OnBuiltCallback.Invoke();
            }
        }

        public void ShowNow()
        {
            _animator.enabled = true;
            gameObject.SetActive(true);
        }

        public void HideNow()
        {
            gameObject.SetActive(false);            
        }

        public void HideAnimated()
        {
            transform.DOScale(Vector3.zero, .24f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
        
        public void Destroy()
        {
            if (_simpleDestroy)
            {
                CLog.Log($"{gameObject.name} simple destroy");
                gameObject.SetActive(false);
                return;
            }
            _animator.enabled = false;
            _destroyer.BreakAll(()=>{});    
            _sinkingPartsController.Activate();
        }

    }
}