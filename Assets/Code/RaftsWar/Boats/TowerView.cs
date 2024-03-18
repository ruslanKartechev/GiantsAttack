using System;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class TowerView : MonoBehaviour
    {
        [SerializeField] private Transform _healthBarPoint;
        [SerializeField] private Transform _progbarPoint;
        [SerializeField] private List<Renderer> _renderers;
        [SerializeField] private TowerAnimator _towerAnimator;
        [SerializeField] private Transform _unitsParent;
        [SerializeField] private ByPartsDestroyer _destroyer;
        [SerializeField] private ParticleSystem _idleParticles;
        [SerializeField] private List<Transform> _unitSpawnPoint;
        [SerializeField] private SinkingPartsController _sinkingPartsController;
        [SerializeField] private Transform _rotatable;
        public Transform HealthBarPoint => _healthBarPoint;
        public Transform ProgbarPoint => _progbarPoint;
        public Transform UnitsParent => _unitsParent;
        public List<Transform> UnitSpawnPoint => _unitSpawnPoint;
        public TowerAnimator TowerAnimator => _towerAnimator;
        
        public void SetView(TowerViewSettings viewSettings)
        {
            for (var i = 0; i < _renderers.Count; i++)
                viewSettings.perRenderer[i].Set(_renderers[i]);
        }

        [ContextMenu("Destroy Tower")]
        public void DestroyTower()
        {
            _idleParticles.gameObject.SetActive(false);
            _destroyer.BreakAll(()=>{});    
            _sinkingPartsController.ActivateAfterDelay(GlobalConfig.SinkingDelay);
        }
 
    }
}