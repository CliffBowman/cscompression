using System.Text;

namespace compression;

public class BurrowsWheelerTransform
{
    public (byte[] data, int index) Encode(byte[] input)
    {
        List<byte> output = new();
        List<Suffix> suffixes = new();

        using (new SimpleTimer("Build suffixes"))
        {
            for (int i = 0; i < input.Length; i++)
                suffixes.Add(new Suffix(input, i));
        }

        using (new SimpleTimer("Sort suffixes"))
        {
            suffixes.Sort(new SuffixComparer());
        }

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
        private byte[] _input;

        public Suffix(byte[] input, int index)
        {
            _input = input;
            Index = index;
        }

        public int Index { get; private set; }

        // Not a property due to overhead of span construction. Was sneaky slow as property.
        public ReadOnlySpan<byte> GetSpan() => new ReadOnlySpan<byte>(_input, Index, _input.Length - Index);

        public override string ToString()
        {
            var debugSpan = GetSpan();
            var decodedText = Encoding.ASCII.GetString(debugSpan.Slice(0, Math.Min(100, debugSpan.Length)));
            return $"{decodedText} {Index}";
        }
    }

    private class SuffixComparer : IComparer<Suffix>
    {
        public int Compare(Suffix x, Suffix y)
        {
            ReadOnlySpan<byte> spanX = x.GetSpan();
            ReadOnlySpan<byte> spanY = y.GetSpan();

            if (spanX.Length == 0 && spanY.Length == 0)
                return 0;

            if (spanX.Length > 0 && spanY.Length == 0)
                return 1;

            if (spanX.Length == 0 && spanY.Length > 0)
                return -1;

            for (var i = 0; i < Math.Min(spanX.Length, spanY.Length); i++)
            {
                var result = spanX[i].CompareTo(spanY[i]);

                if (result != 0)
                    return result;
            }

            return 0;
        }
    }
}
