using System;
using System.Collections.Generic;
using DG.Tweening;
using SleepDev;
using TMPro;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class CatapultMagazine : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private float _scale;
        [SerializeField] private float _size;
        [SerializeField] private float _moveTime;
        [SerializeField] private Transform _root;
        private Stack<ICatapultProjectile> _projectiles = new Stack<ICatapultProjectile>(10);
        private Team _team;
        
        public void Init(Team team)
        {
            _team = team;
        }

        public bool HasProjectiles() => _projectiles.Count > 0;
        public int Count => _projectiles.Count;

        /// <summary>
        /// Can throw exception if no projectiles stored
        /// </summary>
        public ICatapultProjectile GetProjectile()
        {
            var s =  _projectiles.Pop();
            _text.text = $"{Count}";
            return s;
        }

        public void Take(BoatPart part)
        {
            var projectile = GCon.CatapultProjectilesPool.GetObject();
            projectile.Init(_team);
            var tr = projectile.Go.transform;
            tr.CopyPosRot(part.transform);
            tr.parent = _root;
            var seq = DOTween.Sequence();
            seq.Append(tr.DOLocalMove(LocalPos(), _moveTime));
            seq.Join(tr.DOScale(Vector3.one * _scale, _moveTime));
            seq.Join(tr.DOLocalRotate(Vector3.zero, _moveTime));
            part.Hide();
            _projectiles.Push(projectile);
            _text.text = $"{Count}";
        }

        private Vector3 LocalPos()
        {
            var y = (_size * _scale / 2f)
                    + _size * _scale * _projectiles.Count;
            // CLog.LogRed($"y {y}, count {_projectiles.Count}");
            return new Vector3(0, y,0);
        }
    }
}