using System.Linq;

namespace Matrices
{
    internal static class ArrayExt
    {
        internal static T[] GetColumn<T>(this T[,] array, int columnNumber)
        {
            return Enumerable.Range(0, array.GetLength(0))
                    .Select(x => array[x, columnNumber])
                    .ToArray();
        }

        internal static T[] GetRow<T>(this T[,] array, int rowNumber)
        {
            return Enumerable.Range(0, array.GetLength(1))
                    .Select(x => array[rowNumber, x])
                    .ToArray();
        }
    }
}
