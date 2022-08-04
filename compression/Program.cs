using System.IO.Compression;
using compression;

var input = File.ReadAllText(@"don_quixote.txt").Substring(0, 10_000);
var output = "";

using (new SimpleTimer("Lzss"))
    output = new Lzss(CompressionLevel.SmallestSize).LzssEncode(input);


// Console.WriteLine(input);
// Console.WriteLine("===");
// Console.WriteLine(output);
Console.WriteLine($"{input.Length} in, {output.Length} out, {((double)output.Length / (double)input.Length) * 100}%");
