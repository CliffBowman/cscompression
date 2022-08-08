using System.IO;
using System.IO.Compression;
using System.Text;
using compression;

var outputStats = (int inputLength, int outputLength) =>
{
    // Console.WriteLine($"{inputLength} in, {outputLength} out, {(1 - ((double)inputLength / (double)outputLength)) * 100}%");
    Console.WriteLine($"{inputLength} in, {outputLength} out, {((double)outputLength / (double)inputLength) * 100}% of original size.");
};

// var input = File.ReadAllText(@"don_quixote.txt").Substring(0, 10_000);
// var output = "";

// using (new SimpleTimer("Lzss"))
//     output = new Lzss(CompressionLevel.SmallestSize).Encode(input);

// Console.WriteLine($"{input.Length} in, {output.Length} out, {((double)output.Length / (double)input.Length) * 100}%");

// using (new SimpleTimer("RLE"))
//     output = new RunLengthEncoding().Encode(input);

// Console.WriteLine($"{input.Length} in, {output.Length} out, {((double)output.Length / (double)input.Length) * 100}%");


// var input = "AAAAA";
// var bytes = Encoding.ASCII.GetBytes(input);
// var output = new RunLengthEncoding().Encode(bytes);

// Console.WriteLine($"{input.Length} in, {output.Length} out, {((double)output.Length / (double)input.Length) * 100}%");


// var b = (byte)50;
// var binary = Convert.ToString(b, 2);
// var output = binary.PadLeft(8, '0');


// var input = Enumerable.Range(0, 1000).Select(i => (byte)1).ToArray<byte>();
// var output = new RunLengthEncoding().EncodeBytes(input);

// File.WriteAllBytes("repeat.bin", output);

byte[] input;
byte[] output;

input = File.ReadAllBytes("don_quixote.txt");

using (new SimpleTimer("RLE bytes"))
{
    output = new RunLengthEncoding().EncodeBytes(input);
    outputStats(input.Length, output.Length);
}


input = File.ReadAllBytes("don_quixote.txt").ToArray();
output = new MoveToFrontTransform().Encode(input);

using (new SimpleTimer("RLE bytes"))
{
    output = new RunLengthEncoding().EncodeBytes(output);
    outputStats(input.Length, output.Length);
}

var i = 1;
