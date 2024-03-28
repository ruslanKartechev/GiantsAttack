using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace SleepDev.UIUtils
{
    public class PopAnimator : MonoBehaviour
    {
        [SerializeField] private List<PopElement> _elements;

        private bool _isDone;
        public bool IsDone => _isDone;

        public void HideAll()
        {
            foreach (var el in _elements)
            {
                el.transform.localScale = Vector3.zero;
            }
        }

        public void HideAndPlay(Action onDone = null)
        {
            HideAll();
            StartCoroutine(Working(onDone));
        }
        
        public IEnumerator Working(Action onDone = null)
        {
            _isDone = false;
            var totalTime = 0f;
            foreach (var pop in _elements)
                totalTime += pop.Delay;
            var lastDur =_elements[^1].Duration;
            
            foreach (var pop in _elements)
            {
                yield return new WaitForSeconds(pop.Delay);
                pop.ScaleUp();
            }
            yield return new WaitForSeconds(lastDur);
            _isDone = true;
            onDone?.Invoke();
        }

        #if UNITY_EDITOR
        [Space(20)] 
        public float e_durationAll;
        public float e_delayAll;
        public Ease e_easeAll;
        public List<GameObject> e_buildFrom;

        [ContextMenu("Play")]
        public void E_Play()
        {
            HideAndPlay();
        }

        public void E_Build()
        {
            _elements.Clear();
            foreach (var go in e_buildFrom)
            {
                var element = go.GetComponent<PopElement>();
                if (element == null)
                    element = go.AddComponent<SimplePopElement>();
                element.Delay = e_delayAll;
                element.Duration = e_durationAll;
                var pp = (SimplePopElement)element;
                if(pp != null)
                    pp.Ease = e_easeAll;
                EditorUtility.SetDirty(element);
                _elements.Add(element);
            }
            EditorUtility.SetDirty(this);
        }
        
        [ContextMenu("Hide")]
        public void E_Hide()
        {
            HideAndPlay();
        }

        [ContextMenu("Set Delay All")]
        public void SetDelayAll()
        {
            foreach (var pop in _elements)
            {
                if(pop == null)
                    continue;
                pop.Delay = e_delayAll;
                UnityEditor.EditorUtility.SetDirty(pop);
            }
        }
        
        [ContextMenu("Set Duration All")]
        public void SetDurationAll()
        {
            foreach (var pop in _elements)
            {
                if(pop == null)
                    continue;
                pop.Duration = e_durationAll;
                UnityEditor.EditorUtility.SetDirty(pop);
            }
        }

        [ContextMenu("Set Ease All")]
        public void SetEaseAll()
        {
            foreach (var pop in _elements)
            {
                var pp = (SimplePopElement)pop;
                if(pp == null)
                    continue;
                pp.Ease = e_easeAll;
                UnityEditor.EditorUtility.SetDirty(pop);
            }
        }
#endif
    }
    
    
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(PopAnimator))]
    public class PopAnimatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as PopAnimator;
            var width = 150;
            GUILayout.Space(20);
            if (GUILayout.Button($"Play", GUILayout.Width(width)))
            {
                me.E_Play();
            }            
            if (GUILayout.Button($"Build from go", GUILayout.Width(width)))
            {
                me.E_Build();
            }
            if (GUILayout.Button($"Set Duration",GUILayout.Width(width)))
            {
                me.SetDurationAll();   
            }
            if (GUILayout.Button($"Build Delay",GUILayout.Width(width)))
            {
                me.SetDelayAll();
            }
            if (GUILayout.Button($"Build Ease", GUILayout.Width(width)))
            {
                me.SetEaseAll();   
            }
        }
    }
    #endif
}