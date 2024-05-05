using DG.Tweening;
using UnityEngine;

namespace GameCore.UI
{
    public class LevelResultsUI : MonoBehaviour
    {
        [SerializeField] private DataPiece _hits;
        [SerializeField] private DataPiece _headshots;
        [SerializeField] private DataPiece _misses;
        
        
        [System.Serializable]
        public class DataPiece
        {
            public bool forwardLimits = true;
            public float limitHigh;
            public float limitMiddle;
            public StarsUI stars;
            public TextByCharPrinter labelPrinter;
            public TextByCharPrinter percentPrinter;

            public void Print(float percent)
            {
                percentPrinter.Text = $"{Mathf.RoundToInt(percent * 100f)}%";
                labelPrinter.PrintText();
                percentPrinter.PrintText();
                percentPrinter.transform.DOPunchScale(Vector3.one * 1.05f, .5f);
                byte starsCount = 1;
                if (forwardLimits)
                {
                    if (percent >= limitHigh)
                        starsCount = 3;
                    else if (percent >= limitMiddle)
                        starsCount = 2;
                }
                else
                {
                    if (percent >= limitHigh)
                        starsCount = 1;
                    else if (percent >= limitMiddle)
                        starsCount = 2;
                    else
                        starsCount = 3;
                }
   
                stars.ShowStars(starsCount);
                
            }
        }

        public void PrintHits(float percent)
        {
            _hits.Print(percent);
        }
        
        public void PrintHeadshots(float percent)
        {
            _headshots.Print(percent);
        }
        
        public void PrintMisses(float percent)
        {
            _misses.Print(percent);
        }

    }
}