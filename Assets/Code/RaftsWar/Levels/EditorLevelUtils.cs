using System.Collections.Generic;
using RaftsWar.Boats;
using SleepDev;
using UnityEngine;
using UnityEngine.Assertions;

namespace RaftsWar.Levels
{
    public class EditorLevelUtils : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("EDITOR UTILS")] 
        [SerializeField] private int _playerTowerLevel;
        [SerializeField] private int _enemyTowerLevels;
        [SerializeField] private GameObjectFactory _factory;
        [SerializeField] private List<GameObject> _editorSpawned;
        [SerializeField] private LevelTeamsManager _teams;
        [SerializeField] private Level _level;

        public void E_SpawnAllTowers()
        {
            E_ClearAll();
            E_SpawnTower(_teams.PlayerTeam, _playerTowerLevel);
            E_SpawnBoat(_teams.PlayerTeam, GlobalConfig.PlayerBoatID);
            foreach (var team in _teams.EnemyTeams)
            {
                E_SpawnTower(team, _enemyTowerLevels);
                E_SpawnBoat(team, GlobalConfig.EnemyBoatID);
            }
        }
        
        public void E_ClearAll()
        {
            foreach (var go in _editorSpawned)
            {
                if(go != null)
                    DestroyImmediate(go);
            }
            _editorSpawned.Clear();
            UnityEditor.EditorUtility.SetDirty(this);
        }

        public void E_SetCameraToStart()
        {
            var camera = BoatUtils.GetCamera();
            camera.SetPoint(_level.StartCameraPoint);
        }

        public void E_SetCameraToPlayer()
        {
            var player = FindObjectOfType<BoatPlayer>();
            Assert.IsNotNull(player);
            var level = (CoreLevel)_level;
            if (level == null)
                return;
            player.CameraPointsManger.E_SetCameraToFirstPoint(level.CameraPointsSettings.settings);
            UnityEditor.EditorUtility.SetDirty(this);
        }

        public void NextInd()
        {
            _playerTowerLevel++;
            _enemyTowerLevels++;
            CheckIndicies();
        }

        public void PrevInd()
        {
            _playerTowerLevel--;
            _enemyTowerLevels--;
            CheckIndicies();
        }

        private void CheckIndicies()
        {
            _playerTowerLevel = Mathf.Clamp(_playerTowerLevel, 0, 4);
            _enemyTowerLevels = Mathf.Clamp(_enemyTowerLevels, 0, 4);
            UnityEditor.EditorUtility.SetDirty(this);

        }
        
        private void E_SpawnTower(Team team, int level)
        {
            var spawnPoint = team.SpawnPoint.towerSpawnPoint;
            var towerGo = _factory.SpawnAsPrefab(GlobalConfig.TowerID);
            _editorSpawned.Add(towerGo);
            towerGo.transform.SetParent(spawnPoint);
            towerGo.transform.CopyPosRot(spawnPoint);
            var tower = towerGo.GetComponent<ITower>();
            Assert.IsNotNull(tower);
            tower.Init(team, level);
            var catapultSpawn = team.SpawnPoint.catapultSpawnPoint;
            if (catapultSpawn.childCount > 0)
            {
                for (var i = 0; i < catapultSpawn.childCount; i++)
                    _editorSpawned.Add(catapultSpawn.GetChild(i).gameObject);
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }

        private void E_SpawnBoat(Team team, string id)
        {
            var spawnPoint = team.SpawnPoint.boatSpawnPoint;
            var boatGo = _factory.SpawnAsPrefab(id);
            Assert.IsNotNull(boatGo);
            _editorSpawned.Add(boatGo);
            var tr = boatGo.transform;
            tr.parent = spawnPoint;
            tr.CopyPosRot(spawnPoint);
            var pos = tr.position;
            pos.y = _teams.BoatsPlane.position.y;
            tr.position = pos;
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}