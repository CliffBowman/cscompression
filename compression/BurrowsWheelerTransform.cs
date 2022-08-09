using System.Text;

namespace compression;

public class BurrowsWheelerTransform
{
    public (byte[] data, int index) Encode(byte[] input)
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
            var suffixIndex = (suffixes[i].Index - 1 + suffixes.Count) % suffixes.Count;
            output.Add(input[suffixIndex]);
        }

        var index = -1;

        for (int i = 0; i < suffixes.Count; i++)
        {
            if (suffixes[i].Index == 0)
            {
                index = i;
                break;
            }
        }

        return (output.ToArray(), index);
    }

    public byte[] Decode(byte[] input, int index)
    {
        List<byte> output = new();
        List<List<byte>> table = new();

        for (int i = 0; i < input.Length; i++)
        {
            var row = new List<byte>();

            table.Add(row);

            for (int j = 0; j < input.Length; j++)
            {
                row.Add(0);
            }
        }

        for (int col = 0; col < input.Length; col++)
        {
            var colIndex = input.Length - col - 1;

            for (int row = 0; row < input.Length; row++)
                table[row][colIndex] = input[row];

            table.Sort(new ListComparer<byte>());
        }

        // Console.WriteLine("====");

        // for (var row = 0; row < table.Count; row++)
        //     Console.WriteLine(row + " - " + string.Join("", table[row].Select(b => (char)b)));

        return table[index].ToArray();
    }

    private class Suffix
    {
        public int Index { get; set; }
        public List<byte> SuffixBytes { get; set; } = new List<byte>();

        public override string ToString()
        {
            return $"{string.Join("", Encoding.ASCII.GetString(SuffixBytes.ToArray()))} {Index}";
        }
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
