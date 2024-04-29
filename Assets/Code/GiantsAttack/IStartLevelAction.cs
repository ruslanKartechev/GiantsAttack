using System;

namespace GiantsAttack
{
    public interface IStartLevelAction
    {
        IHelicopter Player { get; set; }
        IMonster Enemy { get; set; }
        void Execute(Action stagesBeginCallback);
    }
}