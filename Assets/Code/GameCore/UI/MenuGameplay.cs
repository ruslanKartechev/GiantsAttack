using System;
using UnityEngine;
using UnityEngine.UI;
using GCon = GameCore.Core.GCon;

namespace GameCore.UI
{
    public class MenuGameplay : MonoBehaviour, IGameplayMenu
    {
        [SerializeField] private Button _pauseButton;
        [SerializeField] private UIDamagedEffect _damagedEffect;
        [SerializeField] private AimUI _aimUI;
        [SerializeField] private DamageHitsUI _damageHits;
        [SerializeField] private EvadeUI _evadeUI;
        [SerializeField] private ShootAtTargetUI _shootAtTargetUI;
        [SerializeField] private BodySectionsUI _bodySectionsUI;

        public IAimUI AimUI => _aimUI;
        public IUIDamagedEffect DamagedEffect => _damagedEffect;
        public IDamageHitsUI DamageHits => _damageHits;
        public EvadeUI EvadeUI => _evadeUI;
        public IShootAtTargetUI ShootAtTargetUI => _shootAtTargetUI;
        public IBodySectionsUI EnemyBodySectionsUI => _bodySectionsUI;
        
        
        public void AddBodySectionsUI(GameObject prefab)
        {
            throw new NotImplementedException();
            // var go = Instantiate(prefab, transform);
            // _bodySectionsUI = go.GetComponent<IBodySectionsUI>();
        }

        public GameObject Go => gameObject;

        
        public void On()
        {
            gameObject.SetActive(true);
        }

        public void Off()
        {
            gameObject.SetActive(false);
        }

        public void Show(Action onDone)
        {
            On();
            onDone?.Invoke();
        }

        public void Hide(Action onDone)
        {
            Off();
            onDone?.Invoke();
        }
        
        private void ShowPause()
        {
            var pause = GCon.UIFactory.GetPauseUI();
            pause.Show(() => {});
        }
        
        private void OnEnable()
        {
            _pauseButton.onClick.AddListener(ShowPause);
        }
        
        private void OnDisable()
        {
            _pauseButton.onClick.RemoveListener(ShowPause);
        }
    }
}