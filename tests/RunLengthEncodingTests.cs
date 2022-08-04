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
}
