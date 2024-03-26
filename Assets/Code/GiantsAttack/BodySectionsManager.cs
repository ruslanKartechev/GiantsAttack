using System.Collections.Generic;
using GameCore.UI;
using SleepDev.Ragdoll;
using UnityEngine;

namespace GiantsAttack
{
    public class BodySectionsManager : MonoBehaviour
    {
        [Header("0-head, 1-left arm, 2-right arm, \n 3-left leg, 4-right leg, 5-body")]
        [SerializeField] private List<BodySection> _sections;

        public void Init(IHealth health, IBodySectionsUI ui)
        {
            ui.Show();
            foreach (var section in _sections)
            {
                section.SetUI(ui);
                section.Init(health);
            }
        }
        
        
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
                    flicker = tr.gameObject.GetComponent<FlickerAnimator>();
                    if (flicker != null)
                        break;
                }
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        [ContextMenu("E_AddOrGrabAllBodyParts")]
        public void E_AddOrGrabAllBodyParts()
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
#endif

    }
}