namespace compression.common;

public class Codebook : Dictionary<byte, bool[]>
{
    public Codebook() : base()
    {
    }

    public Codebook(byte[] data) : base()
    {
        Load(data);
    }

    // length of bits for n byte
    // all bits
    public void Save(Stream stream)
    {
        List<bool> bits = new();

        for (byte i = 0; i < byte.MaxValue; i++)
        {
            int numberOfBits = 0;

            if (this.ContainsKey(i))
            {
                numberOfBits = this[i].Length;
                bits.AddRange(this[i]);
            }

            stream.WriteByte((byte)numberOfBits);
        }

        var bytes = GetBytes(bits.ToArray());

        stream.Write(bytes, 0, bytes.Length);
    }

    public void Load(byte[] data)
    {
        byte[] bitBytes = data.Skip(byte.MaxValue).ToArray();
        bool[] allBits = GetBits(bitBytes);
        int position = 0;

        for (byte i = 0; i < byte.MaxValue; i++)
        {
            byte numberOfBits = data[i];

            if (numberOfBits == 0)
                continue;

            bool[] bits = new bool[numberOfBits];

            Array.Copy(allBits, position, bits, 0, bits.Length);

            this.Add(i, bits);

            position += numberOfBits;
        }
    }

    public byte[] GetBytes(bool[] bits)
    {
        if (bits == null)
            throw new ArgumentNullException(nameof(bits));

        if (bits.Length == 0)
            return new byte[] { };

        List<byte> output = new();
        Func<int, bool> getBitOrPaddingValue = (int position) => position < bits.Length ? bits[position] : false;
        byte current = Convert.ToByte(getBitOrPaddingValue(0));

        for (var i = 1; i < bits.Length; i++)
        {
            byte currentBit = getBitOrPaddingValue(i) ? (byte)1 : (byte)0;

            if (i % 8 == 0)
            {
                output.Add(current);
                current = currentBit;
                continue;
            }

            current = (byte)(current << 1);
            current = (byte)(current | currentBit);
        }

        output.Add(current);

        return output.ToArray();
    }

    public bool[] GetBits(byte[] data)
    {
        List<bool> output = new();

        for (var i = 0; i < data.Length; i++)
            for (var position = 7; position >= 0; position--)
                output.Add(GetBitValue(data[i], position));

        return output.ToArray();
    }

    public bool GetBitValue(byte data, int position)
    {
        var shift = (data >> position) & 1;

        return shift == 1 ? true : false;
    }
}
