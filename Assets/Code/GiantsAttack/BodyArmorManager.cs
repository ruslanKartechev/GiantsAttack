using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class BodyArmorManager : MonoBehaviour
    {
        [SerializeField] private float _pushForce;
        [SerializeField] private Transform _centerPoint;
        [SerializeField] private List<Transform> _parents;
        [SerializeField] private List<ArmorDataSo> _armorData;
        private IHealth _health;
        private List<ArmorPiece> _armorPieces;
        private List<float> _bars1 = new () {.8f, .55f, .33f};
        private List<float> _bars2 = new () {.9f, .7f, .6f, .5f, .4f, .3f};
        private List<float> _healthBars;
        private int _barIndex;


        public List<ArmorPiece> ArmorPieces => _armorPieces;
        
        public List<ArmorDataSo> ArmorData
        {
            get => _armorData;
            set => _armorData = value;
        }
        
        public void SpawnArmor()
        {
            _armorPieces = new List<ArmorPiece>(_armorData.Count);
            foreach (var data in _armorData)
            {
                var prefab = Resources.Load<ArmorPiece>($"Prefabs/Armor/{data.data.id}");
                var piece = Instantiate(prefab);
                piece.transform.parent = _parents[data.data.parentIndex];
                piece.transform.localPosition = data.data.localPosition;
                piece.transform.localEulerAngles = data.data.localEulers;
                piece.transform.localScale = data.data.scale;
                _armorPieces.Add(piece);
            }
            _health = gameObject.GetComponent<IHealth>();
            _health.OnDamaged += OnDamaged;
            if (_armorPieces.Count <= 3f)
                _healthBars = _bars1;
            else
                _healthBars = _bars2;
        }

        private void OnDamaged(IDamageable obj)
        {
            // if (_barIndex >= _healthBars.Count)
                // return;
            var health = _health.HealthPercent;
            if (health <= _healthBars[_barIndex])
            {
                _barIndex++;
                BreakOffRandomPiece();
                if (_barIndex >= _healthBars.Count)
                    _health.OnDamaged -= OnDamaged;
            }
        }


        public void BreakOffRandomPiece()
        {
            if (_armorPieces.Count == 0)
                return;
            var piece = _armorPieces.Random();
            piece.PushAway(_centerPoint.position, _pushForce);
            _armorPieces.Remove(piece);
        }
        
        
        #if UNITY_EDITOR
        [Space(20)] 
        [Header("EDITOR")] 
        public List<E_DataPiece> e_peices;
        public List<string> e_names;

        [System.Serializable]
        public class E_DataPiece
        {
            public ArmorPiece piece;
            public ArmorDataSo dataSo;

            public void Save(List<string> names)
            {
                if (piece == null || dataSo == null)
                    return;
                dataSo.data.scale = piece.transform.localScale;
                dataSo.data.localPosition = piece.transform.localPosition;
                dataSo.data.localEulers = piece.transform.localEulerAngles;
                var parentName = piece.transform.parent.name;
                if (names.Contains(parentName))
                {
                    dataSo.data.parentIndex = names.IndexOf(parentName);
                    dataSo.data.id = piece.gameObject.name;
                }
                UnityEditor.EditorUtility.SetDirty(dataSo);
            }
        }

        [ContextMenu("E_SetupDataSo")]
        public void E_SetupDataSo()
        {
            foreach (var data in e_peices)
            {
                data.Save(e_names);
            }
        }

        [ContextMenu("E_SaveNames")]
        public void E_SaveNames()
        {
            e_names = new List<string>(_parents.Count);
            foreach (var pp in _parents)
                e_names.Add(pp.gameObject.name);
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [ContextMenu("E_FindParentsFromNames")]
        public void E_FindParentsFromNames()
        {
            _parents = new List<Transform>(e_names.Count);
            foreach (var nn in e_names)
            {
                var pp = SleepDev.Utils.GameUtils.FindInChildren(transform, (g) => g.name.Contains(nn));
                if (pp == null)
                {
                    CLog.LogRed($"{nn} not found");
                    return;
                }
                _parents.Add(pp.transform);
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}