using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace compression;

public class Lzss
{
    private readonly CompressionLevel _compressionLevel;

    public Lzss(CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        _compressionLevel = compressionLevel;
    }

    public string LzssEncode(string input)
    {
        int buffer_size = GetBufferSize(input.Length);
        var buffer = new StringBuilder(buffer_size);
        var chunk = new StringBuilder();
        var output = new StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            ReportProgress(i, input.Length);

            var result = FindIndexesOf(buffer.ToString(), input[i].ToString());

            buffer.Append(input[i]);

            if (result.Count == 0)
            {
                output.Append(input[i]);
                continue;
            }

            chunk.Append(input[i]);

            var lastIndex = result[result.Count - 1];

            while (i + 1 < input.Length)
            {
#if PERF                
                using var timer = new SimpleTimer("InnerLoop", TimeSpan.FromMilliseconds(1));
#endif

                i++;
                chunk.Append(input[i]);
                result = FindIndexesOf(buffer.ToString(), chunk.ToString());
                buffer.Append(input[i]);

                if (result.Count == 0)
                {
                    i--;
                    chunk.Remove(chunk.Length - 1, 1);
                    buffer.Remove(buffer.Length - 1, 1);
                    break;
                }

                lastIndex = result[result.Count - 1];
                buffer.TrimTail(buffer_size);

                ReportProgress(i, input.Length);
            }

            var reference = $"<{i - chunk.Length - lastIndex + 1},{chunk.Length}>";

            if (chunk.Length <= reference.Length)
                output.Append(chunk);
            else
                output.Append(reference);

            chunk.Clear();
            buffer.TrimTail(buffer_size);
        }

        return output.ToString();
    }

    public string LzssDecode(string input)
    {
        var match = Regex.Match(input, @"<(\d+),(\d+)>");
        var output = input;

        while (match.Success)
        {
            var reference = match.Groups[0].Value;
            var back = int.Parse(match.Groups[1].Value);
            var length = int.Parse(match.Groups[2].Value);
            var content = output.Substring(match.Index - back, length);

            output = output.Substring(0, match.Index) + content + output.Substring(match.Index + match.Length);
            match = Regex.Match(output, @"<(\d+),(\d+)>");
        }

        return output;
    }

    private int GetBufferSize(int inputLength)
    {
        int buffer_size;

        switch (_compressionLevel)
        {
            case CompressionLevel.Fastest:
                buffer_size = (int)(inputLength * 0.01);
                break;
            case CompressionLevel.Optimal:
                buffer_size = (int)(inputLength * 0.10);
                break;
            case CompressionLevel.SmallestSize:
                buffer_size = (int)(inputLength);
                break;
            case CompressionLevel.NoCompression:
                buffer_size = 0;
                break;
            default:
                throw new ApplicationException("Unhandled compression level.");
        }

        return buffer_size;
    }

    private List<int> FindIndexesOf(string buffer, string item)
    {
#if PERF                
        using var timer = new SimpleTimer("FindIndexesOf", TimeSpan.FromMilliseconds(1));
#endif    

        var results = new List<int>();
        var index = buffer.IndexOf(item);

        while (index != -1)
        {
            results.Add(index);
            index = buffer.IndexOf(item, index + 1);
        }

        return results;
    }

    private void ReportProgress(int i, int inputLength)
    {
        if (i % (inputLength * .05) == 0)
            Console.WriteLine($"{((double)i / (double)inputLength) * 100}% - {DateTime.Now}");
    }
}