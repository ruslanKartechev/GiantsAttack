using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RaftsWar.Boats;
using SleepDev;
using UnityEngine;
using UnityEngine.Assertions;

namespace RaftsWar.Levels
{
    public class LevelTeamsManager : MonoBehaviour, ITeamsManager
    {
        [SerializeField] private int _playerStartLevel = 0;
        [SerializeField] private int _enemyStartLevel = 0;
        [SerializeField] private Team _playerTeam;
        [SerializeField] private List<EnemyTeam> _enemyTeams;
        [SerializeField] private Transform _boatsPlane;
        [SerializeField] private BoatPartsManager _partsManager;
        
        public IList<EnemyTeam> EnemyTeams => _enemyTeams;
        public Team PlayerTeam => _playerTeam;
        public BoatPlayer PlayerBoat { get; set; }
        public BoatPartsManager PartsManager => _partsManager;
        public Transform BoatsPlane => _boatsPlane;
        
        
        public void SpawnAll(Action onDone, PlayerCameraPointsSettings cameraPointsSettings)
        {
            StopAllCoroutines();
            StartCoroutine(Spawning(onDone, cameraPointsSettings));
        }

        private IEnumerator Spawning(Action onCompleted, PlayerCameraPointsSettings cameraPointsSettings)
        {
            yield return null;
            var delay = GlobalConfig.TowerSpawningDelay;
            SpawnTower(_playerTeam, _playerStartLevel);
            PlayerBoat = SpawnPlayerBoat(_playerTeam);
            PlayerBoat.Init(_playerTeam, _playerTeam.BoatSettings, cameraPointsSettings);
            _playerTeam.Player = PlayerBoat;
            yield return null;
            for (var i = 0; i < _enemyTeams.Count; i++)
            {
                var team = _enemyTeams[i];
                SpawnTower(team, _enemyStartLevel);
                var boat = SpawnEnemyBoat(team);
                team.EnemyBoat = boat;
                team.InitEnemy(_partsManager);
                yield return new WaitForSeconds(delay);
            }
            onCompleted?.Invoke();
        }

        private GameObject SpawnBoat(Team team, string id)
        {
            var spawnPoint = team.SpawnPoint;
            var boatGo = GCon.GOFactory.Spawn(id);
#if UNITY_EDITOR
            Assert.IsNotNull(boatGo);
#endif
            boatGo.transform.parent = _boatsPlane;
            boatGo.transform.CopyPosRot(spawnPoint.boatSpawnPoint);
            var localPos = boatGo.transform.localPosition;
            localPos.y = 0f;
            boatGo.transform.localPosition = localPos;
            return boatGo;
        }
        
        private BoatPlayer SpawnPlayerBoat(Team team)
        {
            var boatGo = SpawnBoat(team, GlobalConfig.PlayerBoatID);
            var player = boatGo.GetComponent<BoatPlayer>();
            boatGo.transform.localScale = Vector3.zero;
            boatGo.transform.DOScale(Vector3.one, GlobalConfig.TowerSpawningDuration);
#if UNITY_EDITOR
            Assert.IsNotNull(player);
#endif
            return player;
        }
        
        
        private BoatEnemy SpawnEnemyBoat(Team team)
        {
            var boatGo = SpawnBoat(team, GlobalConfig.EnemyBoatID);
            boatGo.transform.localScale = Vector3.zero;
            boatGo.transform.DOScale(Vector3.one, GlobalConfig.TowerSpawningDuration);
            var boat = boatGo.GetComponent<BoatEnemy>();
            boatGo.name = $"boat_{team.BoatName}";
#if UNITY_EDITOR
            Assert.IsNotNull(boat);
#endif
            return boat;
        }

        
        private void SpawnTower(Team team, int startLevel = 0)
        {
            var spawnPoint = team.SpawnPoint;
            var towerGo = GCon.GOFactory.Spawn(GCon.TowerRepository.GetCurrentPrefab());
#if UNITY_EDITOR
            Assert.IsNotNull(towerGo);
#endif
            towerGo.name = $"Tower_{team.BoatName}";
            towerGo.transform.parent = transform;
            towerGo.transform.CopyPosRot(spawnPoint.towerSpawnPoint);
            var tower = towerGo.GetComponent<ITower>();
#if UNITY_EDITOR
            Assert.IsNotNull(tower);
#endif
            team.Tower = tower;
            team.InitTower(startLevel);
        }
        

        
    }
}