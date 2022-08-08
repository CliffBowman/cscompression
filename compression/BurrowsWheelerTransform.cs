using System.Text;

namespace compression;

public class BurrowsWheelerTransform
{
    public byte[] Encode(byte[] input)
    {
        List<byte> output = new();
        List<Suffix> suffixes = new();

        for (int i = 0; i < input.Length; i++)
        {
            var suffix = input.Skip(i).ToList();
            suffixes.Add(new Suffix { Index = i, SuffixBytes = suffix });
        }

        suffixes.Sort(new SuffixComparer());

        for (int i = 0; i < suffixes.Count; i++)
        {
            var index = (suffixes[i].Index - 1 + suffixes.Count) % suffixes.Count;
            output.Add(input[index]);
        }

        return output.ToArray();
    }

    public byte[] Decode(byte[] input)
    {
        throw new NotImplementedException();
    }

    private class Suffix
    {
        public int Index { get; set; }
        public List<byte> SuffixBytes { get; set; } = new List<byte>();
    }

    private class SuffixComparer : IComparer<Suffix>
    {
        public int Compare(Suffix x, Suffix y)
        {
            if (x.SuffixBytes.Count == 0 && y.SuffixBytes.Count == 0)
                return 0;

            if (x.SuffixBytes.Count > 0 && y.SuffixBytes.Count == 0)
                return 1;

            if (x.SuffixBytes.Count == 0 && y.SuffixBytes.Count > 0)
                return -1;

            for (var i = 0; i < Math.Min(x.SuffixBytes.Count, y.SuffixBytes.Count); i++)
            {
                var result = x.SuffixBytes[i].CompareTo(y.SuffixBytes[i]);

                if (result != 0)
                    return result;
            }

            return 0;
        }
    }
}
