using System;
using System.Collections.Generic;
using UnityEngine;

namespace SleepDev.Utils
{
   
    public static class GameUtils
    {
        public static bool IsChildOf(Transform obj, Transform parent)
        {
            var pp = obj.parent;
            while (pp != null)
            {
                if (pp == parent)
                    return true;
                pp = pp.parent;
            }
            return false;
        }
        
        public static AT GetOrAdd<AT, AB>(this GameObject root) where AT : Component where AB : AT
        {
            var t = root.GetComponent<AT>();
            if (t == null)
                t = root.AddComponent<AB>();
            return t;
        }
        
        public static T GetOrAdd<T>(this GameObject root) where T: Component
        {
            var t = root.GetComponent<T>();
            if (t == null)
                t = root.AddComponent<T>();
            return t;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static List<T> GetFromAllChildren<T>(Transform parent, Condition<T> condition = null)
        {
            var list = new List<T>();
            var t = parent.GetComponent<T>();
            if (t != null)
            {
                if((condition != null && condition(t)) 
                   || condition == null)
                    list.Add(t);
            }
            GetFromChildrenAndAdd(list, parent.transform, condition);
            
            return list;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void GetFromChildrenAndAdd<T>(ICollection<T> list, Transform parent, Condition<T> condition = null)
        {
            if (parent.childCount == 0)
                return;
            for (var i = 0; i < parent.childCount; i++)
            {
                var target = parent.GetChild(i).GetComponent<T>();
                if (target != null )
                {
                    if((condition != null && condition(target)) 
                       || condition == null)
                        list.Add(target);
       
                }
                GetFromChildrenAndAdd<T>(list, parent.GetChild(i), condition);
            }
        }
        
        public static GameObject FindInChildren(Transform parent, Condition<GameObject> condition)
        {
            if (parent.childCount == 0)
                return null;
            GameObject result = null;
            for (var i = 0; i < parent.childCount; i++)
            {
                var go = parent.GetChild(i).gameObject;
                if (condition(go))
                    return go;
                result = FindInChildren(go.transform, condition);
                if (result != null)
                    return result;
            }
            return result;
        }
        
        
        public static List<Transform> GetAllChildrenAndRename(Transform parent, string name)
        {
            var list = new List<Transform>();
            var count = parent.childCount;
            for (var i = 0; i < count; i++)
            {
                var tr = parent.GetChild(i);
                tr.gameObject.name = $"{name}_{i}";
                list.Add(tr);
                #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(tr.gameObject);
                #endif
            }
            return list;
        }

        public static void ClampLocalRotation(this Transform transform, Vector3 bounds)
        {
            transform.localRotation = ClampQuaternion(transform.localRotation, bounds);
        }        
        
        public static Quaternion ClampQuaternion(Quaternion q, Vector3 bounds)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;
            var angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
            angleX = Mathf.Clamp(angleX, -bounds.x, bounds.x);
            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
 
            var angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);
            angleY = Mathf.Clamp(angleY, -bounds.y, bounds.y);
            q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);
 
            var angleZ = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.z);
            angleZ = Mathf.Clamp(angleZ, -bounds.z, bounds.z);
            q.z = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleZ);
            return q;
        }
        
        
        
        
        
        
        public static T FindClosestTo<T>(ICollection<T> collection, Vector3 center) where T : MonoBehaviour
        {
            T result = null;
            var minD2 = float.MaxValue;
            foreach (var item in collection)
            {
                var d2 = (item.transform.position - center).sqrMagnitude;
                if (d2 <= minD2)
                {
                    minD2 = d2;
                    result = item;
                }
            }
            return result;
        }
        
        public static Transform FindClosestTo(ICollection<Transform> collection, Vector3 center)
        {
            Transform result = null;
            var minD2 = float.MaxValue;
            foreach (var item in collection)
            {
                var d2 = (item.transform.position - center).sqrMagnitude;
                if (d2 <= minD2)
                {
                    minD2 = d2;
                    result = item;
                }
            }
            return result;
        }
        
        public static void Destroy(GameObject go)
        {
#if UNITY_EDITOR
            if(Application.isPlaying)
                 UnityEngine.Object.Destroy(go);
            else
                UnityEngine.Object.DestroyImmediate(go);
#else
                Object.Destroy(go);
#endif
            
        }
        
        
        
        
        
        
        public static T[] ExtendAndCopy<T>(T[] original, int addedLength)
        {
            T[] tempArray = new T[original.Length + addedLength];
            int i = 0;
            foreach (var item in original)
            {
                tempArray[i] = item;
                i++;
            }
            
            return tempArray;
        }
        public static T[] CopyFromArray<T>(this T[] original, T[] from)
        {
            T[] tempArray = new T[original.Length + from.Length];
            int i = 0;
            foreach (var item in original)
            {
                tempArray[i] = item;
                i++;
            }

            foreach (var item in from)
            {
                tempArray[i] = item;
                i++;
            }
            return tempArray;
        }

        public static T[] AddToArray<T>(this T[] original, T nextItem)
        {
            T[] tempArray = new T[original.Length + 1];
            int i = 0;
            foreach (var item in original)
            {
                tempArray[i] = item;
                i++;
            }
            tempArray[i] = nextItem;
            return tempArray;

        }
    }
}