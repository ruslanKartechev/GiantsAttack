using System;
using GiantsAttack;
using SleepDev.Sound;
using SleepDev.UIUtils;
using UnityEngine;
using UnityEngine.UI;
using GCon = GameCore.Core.GCon;

namespace GameCore.UI
{
    public class MenuGameplay : MonoBehaviour, IGameplayMenu
    {
        [SerializeField] private Button _pauseButton;
        [SerializeField] private AimUI _aimUI;
        [SerializeField] private DamageHitsUI _damageHits;
        [SerializeField] private EvadeUI _evadeUI;
        [SerializeField] private ShootAtTargetUI _shootAtTargetUI;
        [SerializeField] private BodySectionsUI _bodySectionsUI;
        [SerializeField] private FlashUI _flashUI;
        [SerializeField] private SoundSo _clickSound;
        [SerializeField] private PopAnimator _animator;
        [SerializeField] private CityDestroyUI _cityDestroyUI;
        [SerializeField] private TextByCharPrinter _eventPrinter;

        public IAimUI AimUI => _aimUI;
        public IDamageHitsUI DamageHits => _damageHits;
        public IShootAtTargetUI ShootAtTargetUI => _shootAtTargetUI;
        public IBodySectionsUI EnemyBodySectionsUI => _bodySectionsUI;
        public EvadeUI EvadeUI => _evadeUI;
        public FlashUI Flash => _flashUI;
        public CityDestroyUI CityDestroyUI => _cityDestroyUI;

        public TextByCharPrinter EventPrinter => _eventPrinter;

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
            _animator.ZeroAndPlay();
            onDone?.Invoke();
        }

        public void Hide(Action onDone)
        {
            Off();
            onDone?.Invoke();
        }
        
        private void ShowPause()
        {
            _clickSound.Play();
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