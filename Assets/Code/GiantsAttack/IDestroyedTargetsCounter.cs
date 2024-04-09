using System;
using GameCore.UI;

namespace GiantsAttack
{
    public interface IDestroyedTargetsCounter
    {
        Action AllDestroyedCallback { get; set; }
        
        IGameplayMenu UI { get; set; }
        
        void SetCounter(int maxCount);

        void MinusOne(bool updateUI);
        
    }
}