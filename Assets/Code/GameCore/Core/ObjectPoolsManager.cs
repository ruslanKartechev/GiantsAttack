﻿using GiantsAttack;
using SleepDev;
using UnityEngine;

namespace GameCore.Core
{
    public class ObjectPoolsManager : MonoBehaviour, IObjectPoolsManager
    {
        private static bool _inited;
        [SerializeField] private int _startBulletsPoolSize = 100;
        [SerializeField] private BulletsPool _bulletsPool; 

        public void BuildPools()
        {
            if (_inited)
            {
                Destroy(gameObject);
                return;
            }
            _inited = true;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            _bulletsPool.GOFactory = GCon.GOFactory;
            _bulletsPool.BuildPool(_startBulletsPoolSize);
            // Setup container
            GCon.PoolsManager = this;
            GCon.BulletsPool = _bulletsPool;
        }

        public void RecollectAll()
        {
            CLog.Log($"[PoolsManager] RecollectAll");
        }
    }
}