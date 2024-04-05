using System;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class Placer : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private string _nameDefaultParent;
        [SerializeField] private Transform _movable;
        [SerializeField] private List<Transform> _points;
        [SerializeField] private int _index;

        public void SetDefault()
        {
            var go = GameObject.Find(_nameDefaultParent);
            if (go == null)
                return;
            SetPoint(go.transform);
        }

        public void SetPoint(Transform point)
        {
            if (_movable == null)
            {
                CLog.Log("movable is null");
                return;
            }
            _movable.parent = point;
            _movable.SetPositionAndRotation(point.position, point.rotation);
            UnityEditor.EditorUtility.SetDirty(gameObject);
        }

        public void Next()
        {
            _index++;
            Place();
        }

        public void Prev()
        {
            _index--;
            Place();
        }

        public void Place()
        {
            if (_points.Count == 0)
                return;
            _index = Mathf.Clamp(_index, 0, _points.Count - 1);
            if (_points[_index] == null)
                return;
            SetPoint(_points[_index]);
            UnityEditor.EditorUtility.SetDirty(this);
        }

        public void OnOff()
        {
            if (_movable == null)
                return;
            var on = _movable.gameObject.activeSelf;
            _movable.gameObject.SetActive(!on);
            UnityEditor.EditorUtility.SetDirty(_movable);
        }
#endif
    }
}