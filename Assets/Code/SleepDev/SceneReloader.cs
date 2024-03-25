using System.Collections;
using GameCore.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SleepDev
{
    public class SceneReloader : MonoBehaviour
    {
        private void OnEnable()
        {
            StartCoroutine(Working());
        }

        private IEnumerator Working()
        {
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    GameCore.Core.GCon.PoolsManager.RecollectAll();
                    var scene = SceneManager.GetActiveScene();
                    if (GCon.SlowMotion != null)
                        GCon.SlowMotion.SetNormalTime();
                    SceneManager.LoadScene(scene.name);
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Break();
                }
                yield return null;
            }
        }
    }
}