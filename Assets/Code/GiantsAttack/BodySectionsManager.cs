using System.Collections.Generic;
using GameCore.UI;
using SleepDev;
using SleepDev.Ragdoll;
using UnityEngine;

namespace GiantsAttack
{
    public class BodySectionsManager : MonoBehaviour
    {
        [Header("0-head,1-body, 2-left arm, 3-right arm, \n4-left leg, 5-right leg, ")]
        [SerializeField] private List<BodySection> _sections;

        [SerializeField] private GameObject _uiPrefab;

        public GameObject UIPrefab => _uiPrefab;

        public void Init(IHealth health, IBodySectionsUI ui)
        {
            ui.Show();
            foreach (var section in _sections)
            {
                section.SetUI(ui);
                section.Init(health);
            }
        }

        public BodySection GetRandomSection()
        {
            if (_sections.Count == 0)
                return null;
            var sc = _sections.Random();
            var it = 0;
            do
            {
                sc = _sections.Random();
                it++;
            } while (sc.Health <= 0 && it < _sections.Count);
            return sc;
        }

        #region Editor
#if UNITY_EDITOR
        public float e_maxHealth;
        public Ragdoll e_ragdoll;
        public void E_SetHealthAll()
        {
            foreach (var sec in _sections)
            {
                sec.MaxHealth = e_maxHealth;
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }

        public void E_GetFlickers()
        {
            foreach (var sec in _sections)
            {
                FlickerAnimator flicker = null;
                foreach (var tr in sec.targets)
                {
                    if(tr == null)
                        continue;
                    flicker = tr.gameObject.GetComponent<FlickerAnimator>();
                    if (flicker != null)
                        break;
                }
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        [ContextMenu("E_GrabParts")]
        public void E_AddOrGrabBodyTargets()
        {
            var bodyParts = new List<BodyPartTarget>(10);
            foreach (var rp in e_ragdoll.parts)
            {
                var go = rp.rb.gameObject;
                var part = go.GetComponent<BodyPartTarget>();
                if (part == null)
                    part = go.AddComponent<BodyPartTarget>();
                bodyParts.Add(part);
                UnityEditor.EditorUtility.SetDirty(go);
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }

        public void E_AssignToSections()
        {
            var names = new List<string>() { "Head" };
            for (var i = 0; i < _sections.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        names = new List<string>() { "Head" };
                        break;
                    case 1:
                        names = new List<string>() { "Spine2", "Hips" };
                        break;
                    case 2:
                        names = new List<string>() { "LeftArm","LeftForeArm" };
                        break;
                    case 3:
                        names = new List<string>() { "RightArm","RightForeArm" };
                        break;
                    case 4:
                        names = new List<string>() { "LeftUpLeg","LeftLeg" };
                        break;
                    case 5:
                        names = new List<string>() { "RightUpLeg","RightLeg" };
                        break;
                }
                E_Add(names, _sections[i]);
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }

        private void E_Add(List<string> names, BodySection section)
        {
            section.targets.Clear();
            foreach (var tName in names)
            {
                var go = SleepDev.Utils.GameUtils.FindInChildren(transform, (go) => go.name.Contains(tName));
                if (go != null)
                {
                    var target = go.GetComponent<BodyPartTarget>();
                    if (target == null)
                        target = go.AddComponent<BodyPartTarget>();
                    section.targets.Add(target);
                }
            }
        }
        
#endif
        #endregion

    }
}