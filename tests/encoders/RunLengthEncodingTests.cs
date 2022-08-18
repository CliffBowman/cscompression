namespace tests.encoders;

using Xunit.Abstractions;
using System.Text;
using compression.encoders;

public class RunLengthEncodingTests
{
    private readonly ITestOutputHelper _output;

    public RunLengthEncodingTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public byte[] GetBytes(string txt) => Encoding.ASCII.GetBytes(txt);
    public byte EscapeByte = RunLengthEncoding.EscapeByte;

    [Fact]
    public void NonRepeatingTest()
    {
        var input = GetBytes("abcdefgh");
        var output = new RunLengthEncoding().Encode(input);

        Assert.Equal(GetBytes("abcdefgh"), output);
    }

    [Fact]
    public void OneRepeatingSequenceTest()
    {
        var input = GetBytes("abcccccdefgh");
        var output = new RunLengthEncoding().Encode(input);
        var expected = GetBytes("abc")
            .Concat(new byte[] { EscapeByte, 0x04 })
            .Concat(GetBytes("defgh"))
            .ToArray();

        Assert.Equal(expected, output);
    }

    [Fact]
    public void EncodeDecodeTest()
    {
        var input = GetBytes("abcccccdefgh");
        var encoded = new RunLengthEncoding().Encode(input);
        var decoded = new RunLengthEncoding().Decode(encoded);

        Assert.Equal(input, decoded);
    }
}
