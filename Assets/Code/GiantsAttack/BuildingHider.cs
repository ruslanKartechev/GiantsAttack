using UnityEngine;

namespace GiantsAttack
{
    public class BuildingHider
    {
        public static void HideBuildingsUnder(Vector3 position, Vector3 forward)
        {
            const float length = 200;
            const float size = 30;
            var box = new Vector3(size, size, size);
            var z = 0f;
            position.y = 0f;
            forward.y = 0f;
            var it = 0;
            var itMax = 25;
            while (z <= length & it < itMax)
            {
                it++;
                var pp = position + forward * z;
                z += size;
                var overlaps = Physics.OverlapBox(pp, box, Quaternion.identity);
                if (overlaps.Length > 0)
                {
                    foreach (var coll in overlaps)
                    {
                        if(coll.gameObject.CompareTag("Building"))
                            coll.gameObject.SetActive(false);
                    }
                }
            }

        }
    }
}