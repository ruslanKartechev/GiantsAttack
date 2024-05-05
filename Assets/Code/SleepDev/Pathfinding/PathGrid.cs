namespace SleepDev
{
    [System.Serializable]
    public class PathGrid
    {
        public GridPoint[,] Points;

        public PathGrid(int sizeX, int sizeY)
        {
            Points = new GridPoint[sizeX, sizeY];
        }

        public GridPoint GetPoint(int x, int y)
        {
#if UNITY_EDITOR
            if (x >= Points.GetLength(0))
            {
                CLog.LogRed($"[PathGrid] X is out of range");
                x = Points.GetLength(0);
            }
            if (y >= Points.GetLength(1))
            {
                CLog.LogRed($"[PathGrid] Y is out of range");
                y = Points.GetLength(1);
            }
#endif
            return Points[x, y];
        } 
        
        
    }
}