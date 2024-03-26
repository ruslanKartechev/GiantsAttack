using System;
using System.Collections;
using UnityEngine;

namespace GiantsAttack
{
    [CreateAssetMenu(menuName = "SO/FlickerSettings", fileName = "FlickerSettings", order = 0)]
    public class FlickerSettingsSo : ScriptableObject
    {
        public Color color;
        public float flickTime;
    }
    public class FlickerAnimator : MonoBehaviour
    {
        [SerializeField] private string _colorKey = "_BaseColor";
        [SerializeField] private Renderer _renderer;
        [SerializeField] private FlickerSettingsSo _flickerSettings;
        private bool _isFlicking;
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif

        public void Flick()
        {
            if (_isFlicking)
                return;
            StopAllCoroutines();
            StartCoroutine(Flicking());
        }


        private IEnumerator Flicking()
        {
            _isFlicking = true;
            var matBlock = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(matBlock);
            matBlock.SetColor(_colorKey, _flickerSettings.color);
            _renderer.SetPropertyBlock(matBlock);
            yield return new WaitForSeconds(_flickerSettings.flickTime);
            matBlock.SetColor(_colorKey, Color.white);
            _renderer.SetPropertyBlock(matBlock);
            _isFlicking = false;
        }
    }
}