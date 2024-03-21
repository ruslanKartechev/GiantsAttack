using SleepDev;

namespace GameCore.UI
{
    public interface IControlsUI
    {
        public ProperButton InputButton { get;}
        void On();
        void Off();
    }
}