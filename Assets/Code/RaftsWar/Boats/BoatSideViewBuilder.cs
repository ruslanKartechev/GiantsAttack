using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class BoatSideViewBuilder : MonoBehaviour
    {
        #if UNITY_EDITOR
        public float sideLength;
        public float sideWidth;
        public float sideHeight;
        [Space(10)]
        public Transform parent;
        public GameObject prefab;
        public GameObject cornerPrefab;
        [Space(10)] 
        public List<BoatSideView> sideViews;
        [Space(10)]
        public List<BoatSideViewParts> parts;
        public List<Transform> spawned;

        [ContextMenu("Spawn")]
        public void Spawn()
        {
            Clear();
            parts.Clear();
            parts = new List<BoatSideViewParts>();
            for (var i = 0; i < 4; i++)
                parts.Add(new BoatSideViewParts());
            
            var rotations = new List<float>()
            {
                0, 90, 180, 270
            };
            var directions = new List<Vector3>()
            {
                Vector3.forward,
                Vector3.right,
                -Vector3.forward,
                -Vector3.right,
            };
            var cornerDirs = new List<Vector3>()
            {
                new Vector3(-.5f, 0f, .5f), // top left
                new Vector3(.5f, 0f, .5f), // top right
                new Vector3(.5f, 0f, -.5f),// bot right
                new Vector3(-.5f, 0f, -.5f), // bot left
            };
            
            var sideScale = new Vector3(sideLength, sideHeight, sideWidth);
            for (var i = 0; i < 4; i++)
            {
                var localPos = directions[i] * sideLength / 2f;
                var localRot = Quaternion.Euler(0, rotations[i], 0);
                var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                var tr = instance.transform;
                tr.parent = parent;
                tr.localRotation = localRot;
                tr.localScale = sideScale;
                tr.localPosition = localPos;
                spawned.Add(instance.transform);
                tr.gameObject.name = $"side_{i}";
                var part = parts[i];
                part.side = tr;
                part.renderers.Add(instance.GetComponentInChildren<Renderer>());
            }
            // spawning corners
            var cornerScale = new Vector3(sideWidth, sideHeight, sideWidth);
            var corners = new List<Transform>(4);
            for (var i = 0; i < 4; i++)
            {
                var localPos = cornerDirs[i] * (sideLength+sideWidth);
                var localRot = Quaternion.Euler(0, rotations[i], 0);
                var instance = PrefabUtility.InstantiatePrefab(cornerPrefab) as GameObject;
                var tr = instance.transform;
                tr.parent = parent;
                tr.localRotation = localRot;
                tr.localScale = cornerScale;
                tr.localPosition = localPos;
                tr.gameObject.name = $"corner_{i}";
                spawned.Add(tr);
                corners.Add(tr);
            }

            for (var i = 0; i < 4; i++)
            {
                var part = parts[i];
                var c1 = i;
                var c2 = c1 + 1;
                if (c2 > 3)
                    c2 = 0;
                var corner1 = corners[c1];
                var corner2 = corners[c2];
                part.corners.Add(corner1);
                part.corners.Add(corner2);
                part.renderers.Add(corner1.GetComponentInChildren<Renderer>());
                part.renderers.Add(corner2.GetComponentInChildren<Renderer>());
            }

            UnityEditor.EditorUtility.SetDirty(this);
            for (var i = 0; i < sideViews.Count; i++)
            {
                if (i <= parts.Count)
                {
                    sideViews[i].SetParts(parts[i]);
                    sideViews[i].transform.localPosition = parts[i].side.localPosition;
                    UnityEditor.EditorUtility.SetDirty(sideViews[i]);
                }
            }
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            foreach (var ss in spawned)
            {
                DestroyImmediate(ss.gameObject);
            }
            spawned.Clear();
        }
        #endif
    }
}