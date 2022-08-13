using System.Text;

namespace compression;

public class BurrowsWheelerTransform
{
    public (byte[] data, int index) Encode(byte[] input)
    {
        List<byte> output = new();
        List<Rotation> rotations = new();

        using (new SimpleTimer("Build rotations"))
        {
            for (int i = 0; i < input.Length; i++)
                rotations.Add(new Rotation(input, i));
        }

        using (new SimpleTimer("Sort rotations"))
        {
            rotations.Sort(new RotationComparer());
        }

        int? index = null;
        var inputRotation = new Rotation(input, 0);

        for (int i = 0; i < rotations.Count; i++)
        {
            var rotation = rotations[i];

            output.Add(rotation[rotation.Length - 1]);

            if (new RotationComparer().Compare(rotation, inputRotation) == 0)
                index = i;
        }

        return (output.ToArray(), index.Value);
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

    private class Rotation
    {
        private readonly byte[] _input;
        private readonly int _rotationPoint;

        public Rotation(byte[] input, int rotationPoint)
        {
            _input = input;
            _rotationPoint = rotationPoint;
        }

        public int Length => _input.Length;
        public byte this[int index] => _input[(_rotationPoint + index) % _input.Length];
    }

    private class RotationComparer : IComparer<Rotation>
    {
        public int Compare(Rotation x, Rotation y)
        {
            if (x.Length == 0 && y.Length == 0)
                return 0;

            if (x.Length > 0 && y.Length == 0)
                return 1;

            if (x.Length == 0 && y.Length > 0)
                return -1;

            for (var i = 0; i < Math.Min(x.Length, y.Length); i++)
            {
                var result = x[i].CompareTo(y[i]);

                if (result != 0)
                    return result;
            }

            return 0;
        }
    }
}
