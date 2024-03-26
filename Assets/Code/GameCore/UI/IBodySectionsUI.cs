namespace GameCore.UI
{
    public interface IBodySectionsUI
    {
        void Init();
        IBodyPartUI GetBodyPartByID(int id);
        void Show();
        void Hide();
    }
}