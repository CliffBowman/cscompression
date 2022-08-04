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
