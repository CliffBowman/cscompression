namespace tests;

using Xunit.Abstractions;
using compression;
using System.Text;

public class RunLengthEncodingRLE90ImplTests
{
    private readonly ITestOutputHelper _output;

    public RunLengthEncodingRLE90ImplTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public byte[] GetBytes(string txt) => Encoding.ASCII.GetBytes(txt);
    public byte EscapeByte = RunLengthEncodingRLE90Impl.EscapeByte;

    [Fact]
    public void NonRepeatingTest()
    {
        var input = GetBytes("abcdefgh");
        var output = new RunLengthEncodingRLE90Impl().Encode(input);

        Assert.Equal(GetBytes("abcdefgh"), output);
    }

    [Fact]
    public void OneRepeatingSequenceTest()
    {
        var input = GetBytes("abcccccdefgh");
        var output = new RunLengthEncodingRLE90Impl().Encode(input);
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
        var encoded = new RunLengthEncodingRLE90Impl().Encode(input);
        var decoded = new RunLengthEncodingRLE90Impl().Decode(encoded);

        Assert.Equal(input, decoded);
    }
}
