namespace tests;

using System.IO.Compression;
using Xunit.Abstractions;
using compression;

public class LzssTests
{
    private readonly ITestOutputHelper _output;

    public LzssTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void BasicTest()
    {
        var encoder = new Lzss(CompressionLevel.SmallestSize);
        var input = "I AM SAM. I AM SAM. SAM I AM.";
        var output = encoder.LzssEncode(input);

        Assert.Equal("I AM SAM. <10,10>SAM I AM.", output);
    }
}

/*
I AM SAM. I AM SAM. SAM I AM.

THAT SAM-I-AM! THAT SAM-I-AM! I DO NOT LIKE THAT SAM-I-AM!

DO WOULD YOU LIKE GREEN EGGS AND HAM?

I DO NOT LIKE THEM,SAM-I-AM.
I DO NOT LIKE GREEN EGGS AND HAM.
*/