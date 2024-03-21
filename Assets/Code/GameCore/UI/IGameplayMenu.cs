using SleepDev;

namespace GameCore.UI
{
    public interface IGameplayMenu : IUIScreen
    {
        JoystickUI JoystickUI { get; }
        IUIDamagedEffect DamagedEffect { get; }
    }
}