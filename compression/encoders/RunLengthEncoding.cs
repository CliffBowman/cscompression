namespace compression.encoders;

public class RunLengthEncoding
{
    public const byte EscapeByte = 0x90;

    public byte[] Encode(byte[] input)
    {
        if (input == null || input.Length <= 3)
            return input;

        List<byte> output = new();
        byte? previousByte = null;
        int length = 0;

        for (var i = 0; i < input.Length; i++)
        {
            byte currentByte = input[i];

            if (currentByte == EscapeByte)
            {
                output.Add(EscapeByte);
                output.Add(0x00);
                continue;
            }

            if (currentByte == previousByte && length < byte.MaxValue - 1)
            {
                length++;
                continue;
            }

            if (length > 0)
            {
                output.Add(EscapeByte);
                output.Add((byte)length);
                length = 0;
            }

            output.Add(currentByte);
            previousByte = currentByte;
        }

        return output.ToArray();
    }

    public byte[] Decode(byte[] input)
    {
        if (input == null || input.Length < 2)
            return input;

        List<byte> output = new();
        byte? previousByte = null;

        for (var i = 0; i < input.Length; i++)
        {
            byte currentByte = input[i];

            if (currentByte == EscapeByte)
            {
                byte nextByte = input[i + 1];

                if (nextByte == 0x00)
                {
                    output.Add(EscapeByte);
                }
                else
                {
                    for (var j = 0; j < (int)nextByte; j++)
                        output.Add(previousByte.Value);
                }

                i++;
                continue;
            }

            output.Add(currentByte);
            previousByte = currentByte;
        }

        return output.ToArray();
    }
}
