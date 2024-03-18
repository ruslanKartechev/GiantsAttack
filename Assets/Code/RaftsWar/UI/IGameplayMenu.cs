using SleepDev;

namespace RaftsWar.UI
{
    public interface IGameplayMenu : IUIScreen
    {
        ProperButton InputButton { get; }
        JoystickUI JoystickUI { get; }
        NamesUIManager NamesUIManager { get; }
        IUIDamagedEffect DamagedEffect { get; }
    }
}