namespace tests;

using Xunit.Abstractions;
using compression;
using System.Text;

public class HuffmanEncodingTests
{
    private readonly ITestOutputHelper _output;

    public HuffmanEncodingTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void GetByteFrequenciesTest()
    {
        var input = Encoding.ASCII.GetBytes("abbcdee");
        var output = new HuffmanEncoding().GetByteFrequencies(input);

        Assert.Equal(1, output[(int)'a']);
        Assert.Equal(2, output[(int)'b']);
        Assert.Equal(1, output[(int)'c']);
        Assert.Equal(1, output[(int)'d']);
        Assert.Equal(2, output[(int)'e']);
    }

    [Fact]
    public void CreateHuffmanTreeTest()
    {
        var input = Encoding.ASCII.GetBytes("abbcdee");
        var tree = new HuffmanEncoding().CreateHuffmanTree(input);

        Assert.Equal(7, tree.Frequency);
        Assert.Equal(4, tree.LeftNode.Frequency);
        Assert.Equal((byte)'b', tree.LeftNode.LeftNode.Byte);
        Assert.Equal(2, tree.LeftNode.LeftNode.Frequency);
        Assert.Equal((byte)'e', tree.LeftNode.RightNode.Byte);
        Assert.Equal(2, tree.LeftNode.RightNode.Frequency);
        Assert.Equal(3, tree.RightNode.Frequency);
        Assert.Equal(2, tree.RightNode.LeftNode.Frequency);
        Assert.Equal((byte)'c', tree.RightNode.LeftNode.LeftNode.Byte);
        Assert.Equal(1, tree.RightNode.LeftNode.LeftNode.Frequency);
        Assert.Equal((byte)'a', tree.RightNode.LeftNode.RightNode.Byte);
        Assert.Equal(1, tree.RightNode.LeftNode.LeftNode.Frequency);
        Assert.Equal((byte)'d', tree.RightNode.RightNode.Byte);
        Assert.Equal(1, tree.RightNode.RightNode.Frequency);
    }

    [Fact]
    public void DecodeHuffmanTest()
    {
        var input = Encoding.ASCII.GetBytes("abbcdee");
        Dictionary<byte, bool[]> codeBook = new();

        var outputBits = new HuffmanEncoding().Encode(input, ref codeBook);
        var outputBytes = new HuffmanEncoding().Decode(outputBits, codeBook);

        var output = Encoding.ASCII.GetString(outputBytes);

        Assert.Equal("abbcdee", output);
    }
}
