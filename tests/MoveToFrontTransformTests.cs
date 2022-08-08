namespace tests;

using Xunit.Abstractions;
using compression;

public class MoveToFrontTransformTests
{
    private readonly ITestOutputHelper _output;

    public MoveToFrontTransformTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void BasicEncodingTest()
    {
        var input = new byte[] { 1, 1, 2, 2, 10, 10, 10, 5, 5 };
        var output = new MoveToFrontTransform().Encode(input);
        var expected = new byte[] { 1, 0, 2, 0, 10, 0, 0, 6, 0 };

        Assert.Equal(expected, output);
    }

    [Fact]
    public void BasicDecodingTest()
    {
        var input = new byte[] { 1, 0, 2, 0, 10, 0, 0, 6, 0 };
        var output = new MoveToFrontTransform().Decode(input);
        var expected = new byte[] { 1, 1, 2, 2, 10, 10, 10, 5, 5 };

        Assert.Equal(expected, output);
    }
}
