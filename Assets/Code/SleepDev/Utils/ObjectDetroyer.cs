using UnityEngine;

namespace SleepDev.Utils
{
    public static class ObjectDetroyer
    {
        public static void Clear(GameObject go)
        {
            #if UNITY_EDITOR
            if(Application.isPlaying)
                Object.Destroy(go);
            else
                Object.DestroyImmediate(go);
            #else
                Object.Destroy(go);
#endif
            
        }
    }
}