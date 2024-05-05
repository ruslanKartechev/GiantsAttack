using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    public class StarsUI : MonoBehaviour
    {
        [SerializeField] private List<Image> _stars;

        public void ShowStars(byte count)
        {
            if (count >= _stars.Count)
                count = (byte)_stars.Count;
            StartCoroutine(Working(count));
        }

        private IEnumerator Working(byte star)
        {
            foreach (var st in _stars)
                st.color = Color.black;
            const float delay = .2f;
            for (var i = 0; i < star; i++)
            {
                yield return new WaitForSeconds(delay);
                _stars[i].color = Color.white;
                _stars[i].transform.DOPunchScale(Vector3.one * 1.1f, .2f);
            }

            yield return null;
        }
    }
}