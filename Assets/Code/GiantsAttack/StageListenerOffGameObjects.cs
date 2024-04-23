using System.Collections.Generic;
using UnityEngine;

namespace GiantsAttack
{
    public class StageListenerOffGameObjects : StageListener
    {
        [SerializeField] private List<GameObject> _offGo;


        public override void OnActivated()
        {
            foreach (var go in _offGo)
                go.SetActive(false);
        }

        public override void OnStopped()
        {
        }

        public override void OnCompleted()
        {
        }
    }
}