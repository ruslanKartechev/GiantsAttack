using System;

namespace GiantsAttack
{
    public interface IFatality
    {
        public IHelicopter Player { get; set; }
        public IMonster Enemy { get; set; }
        void Init(IHelicopter helicopter, IMonster monster);
        void Play(Action onEnd);
    }
}