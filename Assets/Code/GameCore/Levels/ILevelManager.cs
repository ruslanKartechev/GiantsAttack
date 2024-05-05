namespace GameCore
{
    public interface ILevelManager
    {
        /// <summary>
        /// Loads level at current level
        /// </summary>
        void LoadCurrent();
        /// <summary>
        /// Moves level index to next
        /// </summary>
        void NextLevel();
        
        void LoadPrev();
        public ILevelData CurrentLevel { get; }
        int CurrentIndex { get; }
        int NextIndex {get;}
    }
}