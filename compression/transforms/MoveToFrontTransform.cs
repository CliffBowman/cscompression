namespace compression.transforms;

public class MoveToFrontTransform
{
    public byte[] Encode(byte[] input)
    {
        if (input == null)
            return null;

        List<byte> orderedSymbols = Enumerable.Range(0, 255)
            .Select(s => (byte)s)
            .OrderBy(s => s)
            .ToList();
        List<byte> output = new();

        for (int i = 0; i < input.Length; i++)
        {
            var currentByte = input[i];
            var index = orderedSymbols.IndexOf(currentByte);

            output.Add((byte)index);

            orderedSymbols.RemoveAt(index);
            orderedSymbols = orderedSymbols.Prepend(currentByte).ToList();
        }

        return output.ToArray();
    }

    public byte[] Decode(byte[] input)
    {
        if (input == null)
            return null;

        List<byte> orderedSymbols = Enumerable.Range(0, 255)
            .Select(s => (byte)s)
            .OrderBy(s => s)
            .ToList();

        List<byte> output = new();

        for (int i = 0; i < input.Length; i++)
        {
            var currentByte = input[i];
            var lookupValue = orderedSymbols[currentByte];

            output.Add((byte)lookupValue);

            orderedSymbols.RemoveAt(currentByte);
            orderedSymbols = orderedSymbols.Prepend(lookupValue).ToList();
        }

        return output.ToArray();
    }
}
