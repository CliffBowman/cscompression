namespace tests;

using Xunit.Abstractions;
using compression;
using System.Text;

public class FullCompressionTests
{
    private readonly ITestOutputHelper _output;

    public FullCompressionTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private void outputStats(int inputLength, int outputLength)
    {
        _output.WriteLine($"=== {inputLength} in, {outputLength} out, {((double)outputLength / (double)inputLength) * 100}% of original size. ===");
    }

    [Fact]
    // Not all Bzip2 steps are present quite yet.
    public void Bzip2Test()
    {
        var bytes = (string data) => Encoding.UTF8.GetBytes(data);
        var str = (byte[] data) => Encoding.UTF8.GetString(data);
        var codebook = new Dictionary<byte, bool[]>();
        var input = File.ReadAllText("don_quixote.txt").Substring(0, 500);

        // Encode steps
        var outputRLE1 = new RunLengthEncoding().EncodeBytes(bytes(input));
        var outputBWT = new BurrowsWheelerTransform().Encode(outputRLE1);
        var outputMTF = new MoveToFrontTransform().Encode(outputBWT.data);
        var outputRLE2 = new RunLengthEncoding().EncodeBytes(outputMTF);
        var outputBits = new HuffmanEncoding().Encode(outputRLE2, ref codebook);

        outputStats(input.Length, outputBits.Length / 8);

        // Decode steps
        var decodedHuffman = new HuffmanEncoding().Decode(outputBits, codebook);
        Assert.Equal(decodedHuffman, outputRLE2);

        var decodedRLE = new RunLengthEncoding().DecodeBytes(decodedHuffman);
        Assert.Equal(decodedRLE, outputMTF);

        var decodedMTF = new MoveToFrontTransform().Decode(decodedRLE);
        Assert.Equal(decodedMTF, outputBWT.data);

        var decodedBWT = new BurrowsWheelerTransform().Decode(decodedMTF, outputBWT.index);
        Assert.Equal(decodedBWT, outputRLE1);

        decodedRLE = new RunLengthEncoding().DecodeBytes(decodedBWT);
        Assert.Equal(decodedRLE, bytes(input));
    }
}
