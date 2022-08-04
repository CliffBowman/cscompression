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
    public void BasicEncodingTest()
    {
        var encoder = new Lzss(CompressionLevel.SmallestSize);
        var input = "I AM SAM. I AM SAM. SAM I AM.";
        var output = encoder.LzssEncode(input);

        Assert.Equal("I AM SAM. <10,10>SAM I AM.", output);
    }

    [Fact]
    public void DrSeussEncodingSmallestSizeTest()
    {
        var encoder = new Lzss(CompressionLevel.SmallestSize);
        var input = Strings.DrSeuss;
        var output = encoder.LzssEncode(input);
        var ratio = (double)output.Length / (double)input.Length;

        Assert.Equal(Strings.DrSeussLzssEncoded, output);
        Assert.Equal(ratio, 0.75);
    }
}
