namespace tests;

using Xunit.Abstractions;
using compression;
using System.Text;
using compression.encoders;
using compression.transforms;
using compression.common;

public class FullCompressionTests
{
    private readonly ITestOutputHelper _output;

    public FullCompressionTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private void outputStats(string label, int inputLength, int outputLength)
    {
        var percent = string.Format("{0:F2}",((double)outputLength / (double)inputLength) * 100);
        _output.WriteLine($"=== {label} {inputLength} in, {outputLength} out, {percent}% of original size. ===");
    }

    [Fact]
    // Not all Bzip2 steps are present quite yet.
    public void Bzip2ishTest()
    {
        var bytes = (string data) => Encoding.UTF8.GetBytes(data);
        var str = (byte[] data) => Encoding.UTF8.GetString(data);
        var codebook = new Codebook();
        var input = File.ReadAllBytes("static/don_quixote.txt").Take(100_000).ToArray();

        // Encode steps
        var outputRLE1 = new RunLengthEncoding().Encode(input);
        outputStats("RLE90", input.Length, outputRLE1.Length);

        var outputBWT = new BurrowsWheelerTransform().Encode(outputRLE1);
        outputStats("BWT", outputRLE1.Length, outputBWT.data.Length);

        var outputMTF = new MoveToFrontTransform().Encode(outputBWT.data);
        outputStats("MTF", outputBWT.data.Length, outputMTF.Length);

        var outputRLE2 = new RunLengthEncoding().Encode(outputMTF);
        outputStats("RLE90", outputMTF.Length, outputRLE2.Length);

        var outputBits = new HuffmanEncoding().Encode(outputRLE2, ref codebook);
        outputStats("Huffman", input.Length, outputBits.Length / 8);

        // Decode steps
        var decodedHuffman = new HuffmanEncoding().Decode(outputBits, codebook);
        Assert.Equal(decodedHuffman, outputRLE2);

        var decodedRLE = new RunLengthEncoding().Decode(decodedHuffman);
        Assert.Equal(decodedRLE, outputMTF);

        var decodedMTF = new MoveToFrontTransform().Decode(decodedRLE);
        Assert.Equal(decodedMTF, outputBWT.data);

        var decodedBWT = new BurrowsWheelerTransform().Decode(decodedMTF, outputBWT.index);
        Assert.Equal(decodedBWT, outputRLE1);

        decodedRLE = new RunLengthEncoding().Decode(decodedBWT);
        Assert.Equal(decodedRLE, input);
    }
}
