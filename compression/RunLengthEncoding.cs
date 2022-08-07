using System.Text;

namespace compression;

public class RunLengthEncoding
{
    private const char _control = '*';

    public string Encode(string input)
    {
        if (input == null || input.Length <= 3)
            return input;

        var output = new StringBuilder();
        char previousChar = input[0];
        int length = 1;

        for (int i = 1; i < input.Length; i++)
        {
            var currentChar = input[i];

            if (currentChar == previousChar)
            {
                length++;

                if (i != input.Length - 1)
                    continue;
            }

            if (length <= 3)
            {
                for (int j = 0; j < length; j++)
                    output.Append(previousChar);
            }
            else
            {
                output.Append($"{length}{_control}{previousChar}");
            }

            previousChar = currentChar;
            length = 1;
        }

        return output.ToString();
    }

    /*
    0 to 127 	Copy the next n symbols verbatim
    128 to 255  Repeat
    
    1,1,1
    131,1

    1,2,3

    - prev = index 0
    1 if curr != prev add to buffer
    2 if curr == prev increment length
    3 


    */
    public byte[] EncodeBytes(byte[] input)
    {
        if (input == null || input.Length < 2)
            return input;

        List<byte> output = new();
        List<byte> buffer = new();
        byte previousByte = input[0];
        int length = 0;

        if (previousByte == input[1])
            length++;
        else
            buffer.Add(previousByte);

        for (int i = 1; i < input.Length; i++)
        {
            var currentByte = input[i];

            if (currentByte == previousByte)
            {
                length++;
            }
            else
            {
                buffer.Add(currentByte);
            }

            if (i + 1 < input.Length)
            {
                if (currentByte == input[i + 1] && buffer.Count > 0 || buffer.Count == 127)
                {
                    output.Add((byte)buffer.Count);
                    output = output.Concat(buffer).ToList();
                    buffer.Clear();
                }
                else if (currentByte != input[i + 1] && length > 0 || length == 127)
                {
                    output.Add((byte)(128 + length));
                    output.Add(currentByte);
                    length = 0;
                }
            }
            else
            {
                if (buffer.Count > 0)
                {
                    output.Add((byte)buffer.Count);
                    output = output.Concat(buffer).ToList();
                    buffer.Clear();
                }
                else
                {
                    output.Add((byte)(128 + length));
                    output.Add(currentByte);
                    length = 0;
                }
            }

            previousByte = currentByte;
        }

        return output.ToArray();
    }

    public byte[] DecodeBytes(byte[] input)
    {
        List<byte> output = new();

        for (var i = 0; i < input.Length; i++)
        {
            var header = input[i];

            if (header < 128)
            {
                for (var j = 0; j < header; j++)
                    output.Add(input[++i]);
            }
            else
            {
                var repeatTimes = header - 128;
                var repeatByte = input[++i];

                for (var j = 0; j < repeatTimes; j++)
                    output.Add(repeatByte);
            }
        }

        return output.ToArray();
    }

    public string Decode(string input)
    {
        if (input == null || input.Length < 2)
            return input;

        var output = new StringBuilder();
        var digit = new StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsDigit(input[i]))
            {
                digit.Append(input[i]);
                continue;
            }

            if (input[i] == _control && digit.Length > 0 && i + 1 < input.Length)
            {
                var length = int.Parse(digit.ToString());
                var charToRepeat = input[++i];

                for (int j = 0; j < length; j++)
                    output.Append(charToRepeat);

                digit.Clear();
                continue;
            }

            output.Append(input[i]);
        }

        if (digit.Length > 0)
            output.Append(digit);

        return output.ToString();
    }
}
