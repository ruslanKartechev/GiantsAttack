using System.Collections;
using DG.Tweening;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class Catapult : MonoExtended, ICatapult
    {
        private const float ScaleTime = .5f;

        [SerializeField] private TowerCollider _collider;
        [SerializeField] private CatapultBuilder _builder;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private RaftAcceptArea _acceptArea;

        private CatapultMagazine _magazine;
        private CatapultSettings _settings;
        private CatapultView _catapultView;
        private CatapultShooter _shooter;
        private CatapultMagazineReceiver _magazineReceiver;
        
        public Team Team { get; private set; }

        public IUnloadPointsManager UnloadPointsManager { get; private set; }
        public IBoatPartReceiver BoatPartReceiver { get; private set; }
        
        public void Init(Team team)
        {
            Team = team;
            _settings = team.CatapultSettings;
            _builder.Init(_settings.towerBuildingSettings, Team, Upgrade);
            _collider.Init();
            // _collider.UpdateCollider();
            UnloadPointsManager = _builder.UnloadPoints;
            BoatPartReceiver = _builder;
            Team.CurrentPartReceiver = _builder;
        }
        
        public void AllowBuilding()
        {
            _builder.AllowTaking(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show(bool animate)
        {
            gameObject.SetActive(true);
            if (animate)
            {
                transform.localScale = Vector3.zero;
                transform.DOScale(Vector3.one, ScaleTime).OnComplete(AllowBuilding);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void SetPosition(Transform point)
        {
            transform.SetParentAndCopy(point);
        }

        private void Upgrade()
        {
            StartCoroutine(Spawning());
        }

        private IEnumerator Spawning()
        {
            yield return new WaitForSeconds(.33f);
            _builder.Disable();
            var view = GCon.GOFactory.Spawn<CatapultView>(GlobalConfig.CatapultViewID);
            view.transform.parent = _spawnPoint;
            view.transform.CopyPosRot(_spawnPoint);
            view.Init(Team);
            view.AnimatePop();
            _catapultView = view;
            yield return new WaitForSeconds(.33f);
            Destroy(_builder);
            _magazine = AddMagazine(_catapultView);
            AddShooter(_catapultView, _magazine);
            Team.CurrentPartReceiver = _magazineReceiver;
        }

        private void AddShooter(CatapultView view, CatapultMagazine magazine)
        {
            _shooter = gameObject.AddComponent<CatapultShooter>();
            _shooter.Init(view, Team, magazine);
            _shooter.Activate();
        }

        private CatapultMagazine AddMagazine(CatapultView view)
        {
            var magazine = view.CatapultMagazine;
            magazine.Init(Team);
            _magazineReceiver = gameObject.AddComponent<CatapultMagazineReceiver>();
            _magazineReceiver.Init(Team, magazine, _acceptArea);
            BoatPartReceiver = _magazineReceiver;
            return magazine;
        }
    }
}