using System;
using GameCore.UI;

namespace GiantsAttack
{
    public interface IDestroyedTargetsCounter
    {
        Action AllDestroyedCallback { get; set; }
        
        
        void SetCounter(int maxCount);

        void MinusOne(bool updateUI);
        void Minus(int count, bool updateUI);

    }
}