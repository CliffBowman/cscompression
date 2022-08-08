using System.Diagnostics.CodeAnalysis;

namespace compression;

public class ListComparer<T> : IComparer<IList<T>> where T : IComparable
{
    public int Compare(IList<T> x, IList<T> y)
    {
        if (x.Count == 0 && y.Count == 0)
            return 0;

        if (x.Count > 0 && y.Count == 0)
            return 1;

        if (x.Count == 0 && y.Count > 0)
            return -1;

        for (var i = 0; i < Math.Min(x.Count, y.Count); i++)
        {
            var result = x[i].CompareTo(y[i]);

            if (result != 0)
                return result;
        }

        return 0;
    }
}
