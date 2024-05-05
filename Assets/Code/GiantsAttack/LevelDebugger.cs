using GameCore;
using UnityEngine;

namespace GiantsAttack
{
    public class LevelDebugger : MonoBehaviour
    {
#if UNITY_EDITOR
        public Level level { get; set; }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                level.Win();
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                level.Fail();
            }
        }
#endif
    }
}