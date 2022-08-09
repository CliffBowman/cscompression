namespace tests;

using Xunit.Abstractions;
using compression;
using System.Text;

public class BurrowsWheelerTransformTests
{
    private readonly ITestOutputHelper _output;

    public BurrowsWheelerTransformTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void BasicEncodingTest()
    {
        var input = Encoding.ASCII.GetBytes("banana$");
        var output = new BurrowsWheelerTransform().Encode(input);

        Assert.Equal("annb$aa", Encoding.ASCII.GetString(output.data));
        Assert.Equal(output.index, 4);
    }
    
    [Fact]
    public void Basic2EncodingTest()
    {
        var input = Encoding.ASCII.GetBytes("abracadabra$");
        var output = new BurrowsWheelerTransform().Encode(input);

        Assert.Equal("ard$rcaaaabb", Encoding.ASCII.GetString(output.data));
        Assert.Equal(output.index, 3);
    }

    [Fact]
    public void EncodeDecodeTest()
    {
        var input = Encoding.ASCII.GetBytes("annb$aa");
        var output = new BurrowsWheelerTransform().Decode(input, 4);

        Assert.Equal("banana$", Encoding.ASCII.GetString(output));
    }
}
