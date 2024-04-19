using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace GameCore.UI
{
    public class TextByCharPrinter : MonoBehaviour
    {
        [SerializeField] private GameObject _go;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private string _text;
        [SerializeField] private float _charDelay;
        [SerializeField] private float _hideDuration;
        private Coroutine _printing;

        public void PrintText()
        {
            _go.SetActive(true);
            _printing = StartCoroutine(PrintingTitle());
        }
        
        public void Hide()
        {
            _go.SetActive(false);
            if(_printing != null)
                StopCoroutine(_printing);
        }

        public void HideAnimated()
        {
            if(_printing != null)
                StopCoroutine(_printing);
            _go.transform.DOScale(new Vector3(1f, 0f, 1f), _hideDuration).OnComplete(() =>
            {
                _go.SetActive(false);
            });
        }
     
        private IEnumerator PrintingTitle()
        {
            var length = _text.Length;
            var en = _text.GetEnumerator();
            var str = "";
            en.MoveNext();
            for (var i = 0; i < length; i++)
            {
                str += en.Current;
                en.MoveNext();
                _title.text = str;
                yield return new WaitForSeconds(_charDelay);
            }
            en.Dispose();
        }
    }
}