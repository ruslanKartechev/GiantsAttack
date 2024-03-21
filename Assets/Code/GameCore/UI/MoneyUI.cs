using DG.Tweening;
using GameCore.Core;
using TMPro;
using UnityEngine;

namespace GameCore.UI
{
    public class MoneyUI : MonoBehaviour
    {
        public static MoneyUI Inst { get; set; }

        [SerializeField] private Transform _scalable;
        [SerializeField] private TextMeshProUGUI _text;

        private void OnEnable()
        {
            if (Inst != null && Inst != this)
            {
                Inst.gameObject.SetActive(false);
            }
            Inst = this;
        }

        public void UpdateCount(float count)
        {
            _text.text = $"{count}";
            _scalable.DOPunchScale(Vector3.one * .05f, .25f);
        }

        public void UpdateCountFromPlayerData()
        {
            #if UNITY_EDITOR
            if (GCon.PlayerData == null)
            {
                UpdateCount(0f);
                return;
            }
            #endif
            var count = GCon.PlayerData.Money;
            UpdateCount(count);
        }
        
        public void SetCountFromPlayerData()
        {
#if UNITY_EDITOR
            if (GCon.PlayerData == null)
            {
                UpdateCount(0f);
                return;
            }
#endif
            var count = GCon.PlayerData.Money;
            SetCount(count);
        }

        public void SetCount(float count)
        {
            _text.text = $"{(int)count}";
        }
    }
}