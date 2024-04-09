using System;
using System.Collections;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GameCore.UI
{
    public class RouletteUI : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private RectTransform _rotatable;
        [SerializeField] private List<Section> _sections;
        [SerializeField] private Button _stopBtn;
        private byte _index;
        private Coroutine _working;

        public Action OnButtonCallback { get; set; }
        
        public GameObject CurrentSectionGO => _sections[_index].ObjGo;
        
        #if UNITY_EDITOR
        // private void Start()
        // {
        //     if(e_autoLaunchOnStart)
        //         Begin();
        // }
        #endif

        public void Begin()
        {
            StopSpinning();
            _working = StartCoroutine(Spinning());
            _stopBtn.onClick.RemoveListener(OnButtonClick);
            _stopBtn.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            Break();
            OnButtonCallback?.Invoke();
        }

        public void Break()
        {
            StopSpinning();
           
        }
        
        public void StopSpinning()
        {
            if(_working != null)
                StopCoroutine(_working);
        }

        private Section CurrentSection => _sections[_index];
        
        private IEnumerator Spinning()
        {
            _index = 0;
            var lastIndex = _sections.Count - 1;
            var startAngle = _sections[0].angleStart;
            var endAngle = _sections[^1].angleEnd;
            var a = startAngle;
            SetAngle(a);
            CurrentSection.highlighter.OnSelected();
            var dir = 1;
            while (true)
            {
                // clockwise
                while (dir == 1)
                {
                    a -= Time.deltaTime * _rotationSpeed * CurrentSection.speedMult;
                    if (a <= endAngle)
                    {
                        dir = -1;
                        a = endAngle;
                    }
                    SetAngle(a);
                    if (a <= CurrentSection.angleEnd && _index < lastIndex)
                        ChangeSelected((byte)(_index + 1) );
                    yield return null;
                }
            
                // counter-clockwise
                while (dir == -1)
                {
                    a += Time.deltaTime * _rotationSpeed * CurrentSection.speedMult;
                    if (a >= startAngle)
                    {
                        dir = 1;
                        a = startAngle;
                    }
                    SetAngle(a);
                    if (a >= CurrentSection.angleStart && _index > 0)
                        ChangeSelected((byte)(_index - 1));
                    yield return null;
                }
            }

            void ChangeSelected(byte nextIndex)
            {
                if (nextIndex == _index)
                    return;
                _sections[_index].highlighter.OnDeselected();
                _sections[nextIndex].highlighter.OnSelected();
                _index = nextIndex;
            }
            void SetAngle(float a)
            {
                _rotatable.localEulerAngles = new Vector3(0f, 0f, a);
            }
        }
                
        #region EDITOR
#if UNITY_EDITOR
        [Space(10), Header("Editor config")] 
        public int e_sectionIndex;
        public bool e_setToMiddle;
        public bool e_autoLaunchOnStart;

        public void E_Next()
        {
            e_sectionIndex++;
            E_SetToIndex();
        }
        
        public void E_Prev()
        {
            e_sectionIndex--;
            E_SetToIndex();
        }

        public void E_Start()
        {
            e_sectionIndex = 0;
            E_SetToIndex();
        }

        private void E_SetToIndex()
        {
            if (_sections.Count == 0)
            {
                Debug.Log($"No Section is assigned");
                return;
            }
            if (_rotatable == null)
            {
                Debug.Log($"Rotatable not assigned");
                return;
            }
            e_sectionIndex = Mathf.Clamp(e_sectionIndex, 0, _sections.Count-1);
            var angle = 0f;
            if(e_sectionIndex == 0)
                angle = _sections[e_sectionIndex].angleStart;
            else if (e_sectionIndex == _sections.Count - 1)
                angle = _sections[e_sectionIndex].angleEnd;
            else
            {
                if (e_setToMiddle)
                    angle = Mathf.Lerp(_sections[e_sectionIndex].angleStart, _sections[e_sectionIndex].angleEnd, .5f);
                else
                    angle = _sections[e_sectionIndex].angleStart;
            }
            Debug.Log($"Index {e_sectionIndex}, angle {angle}");
            _rotatable.localEulerAngles = new Vector3(0f, 0f, angle);
            UnityEditor.EditorUtility.SetDirty(this);
        }

#endif
        #endregion

        [System.Serializable]
        private class Section
        {
            public float angleStart;
            public float angleEnd;
            public float speedMult = 1f;
            public RouletteUIHighlighter highlighter;
            public GameObject ObjGo;
            
            public Object Obj { get; set; }
        }
        
    }
}