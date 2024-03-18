namespace SleepDev.Utils
{
    public static class ArrayUtils
    {
        public static T[] ExtendAndCopy<T>(T[] original, int addedLength)
        {
            T[] tempArray = new T[original.Length + addedLength];
            int i = 0;
            foreach (var item in original)
            {
                tempArray[i] = item;
                i++;
            }
            
            return tempArray;
        }
        public static T[] CopyFromArray<T>(this T[] original, T[] from)
        {
            T[] tempArray = new T[original.Length + from.Length];
            int i = 0;
            foreach (var item in original)
            {
                tempArray[i] = item;
                i++;
            }

            foreach (var item in from)
            {
                tempArray[i] = item;
                i++;
            }
            return tempArray;
        }

        public static T[] AddToArray<T>(this T[] original, T nextItem)
        {
            T[] tempArray = new T[original.Length + 1];
            int i = 0;
            foreach (var item in original)
            {
                tempArray[i] = item;
                i++;
            }
            tempArray[i] = nextItem;
            return tempArray;

        }

        
    }
}