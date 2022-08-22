using System.Text;
using compression.common;

namespace compression.transforms;

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
        Rotation inputRotation = new(input, 0);

        for (int i = 0; i < rotations.Count; i++)
        {
            var rotation = rotations[i];

            output.Add(rotation[rotation.Length - 1]);

            if (new RotationComparer().Compare(rotation, inputRotation) == 0)
                index = i;
        }

        return (output.ToArray(), index.Value);
    }

    // Reference: https://www.geeksforgeeks.org/inverting-burrows-wheeler-transform/
    public byte[] Decode(byte[] input, int index)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        if (index < 0)
            throw new ArgumentOutOfRangeException(nameof(index));

        var indexLists = new List<List<int>>();
        var indexListsCurrentPosition = new Dictionary<List<int>, int>();
        var byteShift = new Dictionary<int, int>();
        var output = new List<byte>();

        // sort the input
        var sortedInput = input.OrderBy(b => b).ToList();

        // for all possible byte values create a list to keep track of indexes of that byte in the input array
        for (var i = 0; i <= byte.MaxValue; i++)
        {
            var list = new List<int>();
            indexLists.Add(list);
            indexListsCurrentPosition.Add(list, 0);
        }

        // iterate through the input and add all indexes found for the byte value
        for (var i = 0; i < input.Length; i++)
            indexLists[input[i]].Add(i);

        // iterate through the sorted input and find the list of indexes for that byte value
        // each time that byte value is found assign the index at the current position as the shift value
        // and move the current position to the next index.
        for (var i = 0; i < sortedInput.Count; i++)
        {
            var indexList = indexLists[sortedInput[i]];
            var currentPosition = indexListsCurrentPosition[indexList];
            byteShift[i] = indexList[currentPosition];
            indexListsCurrentPosition[indexList]++;
        }

        // apply the shift values to the input to get the original byte array
        for (var i = 0; i < input.Length; i++)
        {
            index = byteShift[index];
            output.Add(input[index]);
        }

        return output.ToArray();
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
            var minLength = Math.Min(x.Length, y.Length);

            for (var i = 0; i < minLength; i++)
            {
                var result = x[i].CompareTo(y[i]);

                if (result != 0)
                    return result;
            }

            if (x.Length == y.Length)
                return 0;

            return x.Length < y.Length ? -1 : 1;
        }
    }
}
