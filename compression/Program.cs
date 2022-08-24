using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using compression;
using compression.common;
using compression.encoders;
using compression.transforms;

var outputStats = (int inputLength, int outputLength) =>
{
    // Console.WriteLine($"{inputLength} in, {outputLength} out, {(1 - ((double)inputLength / (double)outputLength)) * 100}%");
    Console.WriteLine($"=== {inputLength} in, {outputLength} out, {((double)outputLength / (double)inputLength) * 100}% of original size. ===");
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

// byte[] input;
// byte[] output;


// input = File.ReadAllBytes("don_quixote.txt");

// using (new SimpleTimer("RLE bytes"))
// {
//     output = new RunLengthEncoding().EncodeBytes(input);
//     outputStats(input.Length, output.Length);
// }


// input = File.ReadAllBytes("don_quixote.txt").ToArray();
// output = new MoveToFrontTransform().Encode(input);

// using (new SimpleTimer("RLE bytes"))
// {
//     output = new RunLengthEncoding().EncodeBytes(output);
//     outputStats(input.Length, output.Length);
// }

// encode

// var inputLength = 1000;
// var inputText = File.ReadAllText("don_quixote.txt").Substring(0, inputLength);
// var outputText = inputText;
// var output = Encoding.ASCII.GetBytes(outputText);
// var bwtIndex = -1;

// using (new SimpleTimer("BWT encode bytes"))
// {
//     var result = new BurrowsWheelerTransform().Encode(output);
//     output = result.data;
//     bwtIndex = result.index;
// }

// Dictionary<byte, bool[]> encodingDict = new();
// bool[] bitOutput;

// using (new SimpleTimer("HuffmanEncoding encode bytes"))
//     bitOutput = new HuffmanEncoding().Encode(output, ref encodingDict);


// using (new SimpleTimer("MTF bytes")) output = new MoveToFrontTransform().Encode(output);

// var tmpMTF = new byte[output.Length];
// Array.Copy(output, tmpMTF, tmpMTF.Length);

// outputText = Encoding.ASCII.GetString(output);

// using (new SimpleTimer("LZSS bytes")) outputText = new Lzss().Encode(outputText);

// // using (new SimpleTimer("BWT bytes")) output = new BurrowsWheelerTransform().Encode(output);
// // using (new SimpleTimer("MTF bytes")) output = new MoveToFrontTransform().Encode(output);
// // using (new SimpleTimer("RLE bytes")) output = new RunLengthEncoding().EncodeBytes(output);

// var outputLength = outputText.Length;
// outputStats(inputLength, bitOutput.Length);

// File.WriteAllText("don_quixote_compressed.txt", outputText);


// // decode

// // inputText = File.ReadAllText("don_quixote_compressed.txt");

// using (new SimpleTimer("LZSS bytes")) outputText = new Lzss().Decode(outputText);

// output = Encoding.ASCII.GetBytes(outputText);

// using (new SimpleTimer("MTF bytes")) output = new MoveToFrontTransform().Decode(output);
// // using (new SimpleTimer("BWT bytes")) output = new BurrowsWheelerTransform().Decode(output);

// byte[] decoded;
// string decodedText;

// using (new SimpleTimer("BWT encode bytes"))
// {
//     decoded = new BurrowsWheelerTransform().Decode(output, bwtIndex);
//     decodedText = Encoding.ASCII.GetString(decoded);
// }




// var input = Encoding.ASCII.GetBytes("helloworld");
// input = File.ReadAllBytes("don_quixote.txt");

// bool[] outputBits;
// byte[] output;
// Dictionary<byte, bool[]> encodingDict = new();

// using (new SimpleTimer("HuffmanEncoding encode bytes"))
//     outputBits = new HuffmanEncoding().Encode(input, ref encodingDict);

// using (new SimpleTimer("HuffmanEncoding decode bytes"))
//     output = new HuffmanEncoding().Decode(outputBits, encodingDict);

// outputStats(input.Length, outputBits.Length / 8);

// using StreamWriter writer = new StreamWriter("dict.txt");

// foreach (var entry in encodingDict.OrderBy(kv => kv.Value.Length))
// {
//     var sb = new StringBuilder();
//     foreach (var bit in entry.Value)
//         sb.Append(bit ? "1" : "0");
//     writer.WriteLine($"{(char)entry.Key};{sb}");
// }



using var timer = new SimpleTimer("Full operation");


// var input = File.ReadAllBytes("static/don_quixote.txt").Take(500_000).ToArray();
// input = Encoding.UTF8.GetBytes("banana$");
byte[] input;
(byte[] data, int index) transformed;
byte[] output;
string outputText;

using var stream = File.Open("static/enwiki-20220820-pages-articles-multistream1.xml", FileMode.Open, FileAccess.Read);

input = new byte[100_000];
stream.Read(input, 0, input.Length);

var codebook = new Codebook();
var outputBits = new Bzip2ishEncoding().Encode(input, ref codebook);
using var outStream = File.Open("output/output.bin", FileMode.Create, FileAccess.Write);

codebook.Save(outStream);

var outputBytes = codebook.GetBytes(outputBits);

outStream.Write(outputBytes, 0, outputBytes.Length);

var i = 1;
