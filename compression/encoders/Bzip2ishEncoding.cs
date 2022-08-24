using compression.common;
using compression.transforms;

namespace compression.encoders;

public class Bzip2ishEncoding
{
    public Bzip2ishEncoding()
    {
    }

    public bool[] Encode(byte[] input, ref Codebook codebook)
    {
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

        return outputBits;
    }

    // public byte[] Decode(byte[] input)
    // {
    //     var decodedHuffman = new HuffmanEncoding().Decode(outputBits, codebook);
    //     var decodedRLE = new RunLengthEncoding().Decode(decodedHuffman);
    //     var decodedMTF = new MoveToFrontTransform().Decode(decodedRLE);
    //     var decodedBWT = new BurrowsWheelerTransform().Decode(decodedMTF, outputBWT.index);
    //     decodedRLE = new RunLengthEncoding().Decode(decodedBWT);
    // }

    private void outputStats(string label, int inputLength, int outputLength)
    {
        var percent = string.Format("{0:F2}", ((double)outputLength / (double)inputLength) * 100);
        Console.WriteLine($"=== {label} {inputLength} in, {outputLength} out, {percent}% of original size. ===");
    }
}
