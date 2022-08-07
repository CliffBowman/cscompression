namespace tests;

using Xunit.Abstractions;
using compression;

public class RunLengthEncodingTests
{
    private readonly ITestOutputHelper _output;

    public RunLengthEncodingTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void BasicEncodingTest()
    {
        var input = "AAAAA";
        var output = new RunLengthEncoding().Encode(input);

        Assert.Equal("5*A", output);
    }

    [Fact]
    public void MulitCharEncodingTest()
    {
        var input = "AAAAABBBCCCCCC";
        var output = new RunLengthEncoding().Encode(input);

        Assert.Equal("5*ABBB6*C", output);
    }

    [Fact]
    public void SimpleDecodeTest()
    {
        var input = "5*A";
        var output = new RunLengthEncoding().Decode(input);

        Assert.Equal("AAAAA", output);
    }

    [Fact]
    public void MulitCharDecodingTest()
    {
        var input = "5*ABBB6*C";
        var output = new RunLengthEncoding().Decode(input);

        Assert.Equal("AAAAABBBCCCCCC", output);
    }

    [Fact]
    public void DigitsAtEndDecodingTest()
    {
        var input = "5*A66";
        var output = new RunLengthEncoding().Decode(input);

        Assert.Equal("AAAAA66", output);
    }

    [Fact]
    public void EncodeBytesTest()
    {
        var input = new byte[] { 1, 1, 1, 2, 3, 4, 5, 6, 6, 6, 6 };
        var expected = new byte[] { 131, 1, 5, 2, 3, 4, 5, 6, 131, 6 };
        var output = new RunLengthEncoding().EncodeBytes(input);

        Assert.Equal(expected, output);
    }
    
    [Fact]
    public void EncodeMaxEncodeSizeTest()
    {
        var input = Enumerable.Range(0, 128).Select(i => (byte)1).ToArray<byte>();
        var expected = new byte[] { 255, 1, 129, 1 };
        var output = new RunLengthEncoding().EncodeBytes(input);

        Assert.Equal(expected, output);
    }

    [Fact]
    public void DecodeBytesTest()
    {
        var input = new byte[] { 131, 1, 5, 2, 3, 4, 5, 6, 131, 6 };
        var expected = new byte[] { 1, 1, 1, 2, 3, 4, 5, 6, 6, 6, 6 };
        var output = new RunLengthEncoding().DecodeBytes(input);

        Assert.Equal(expected, output);
    }

}
