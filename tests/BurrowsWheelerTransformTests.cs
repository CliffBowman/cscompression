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
    public void DecodeNullInputTest()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            new BurrowsWheelerTransform().Decode(null, 0);
        });
    }

    [Fact]
    public void DecodeInvalidIndexInputTest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            new BurrowsWheelerTransform().Decode(new byte[] { }, -1);
        });
    }

    [Fact]
    public void DecodeEmptyArrayInputTest()
    {
        var output = new BurrowsWheelerTransform().Decode(new byte[] { }, 0);

        Assert.Equal(new byte[] { }, output);
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
    public void Basic3EncodingTest()
    {
        var input = Encoding.ASCII.GetBytes("bana");
        var output = new BurrowsWheelerTransform().Encode(input);

        Assert.Equal("nbaa", Encoding.ASCII.GetString(output.data));
        Assert.Equal(output.index, 2);
    }

    [Fact]
    public void DecodeTest()
    {
        var input = Encoding.ASCII.GetBytes("annb$aa");
        var output = new BurrowsWheelerTransform().Decode(input, 4);

        Assert.Equal("banana$", Encoding.ASCII.GetString(output));
    }

    [Fact(Skip = "Long running full encode / decode validation test")]
    // [Fact]
    public void FullEncodeDecodeTest()
    {
        var input = File.ReadAllBytes("don_quixote.txt");
        (byte[] data, int index) transformed;
        byte[] output;

        using (new SimpleTimer("BWT transform"))
            transformed = new BurrowsWheelerTransform().Encode(input);

        using (new SimpleTimer("BTW inverse"))
            output = new BurrowsWheelerTransform().Decode(transformed.data, transformed.index);

        Assert.Equal(input, output);
    }
}
