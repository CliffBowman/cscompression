using System.Text;

namespace compression;

public class HuffmanEncoding
{
    public bool[] Encode(byte[] input, ref Dictionary<byte, bool[]> codeBook)
    {
        HuffmanNode treeRoot = CreateHuffmanTree(input);
        GetByteEncodings(ref codeBook, treeRoot, new List<bool>());
        List<bool[]> output = new();

        foreach (var b in input)
        {
            var encodedByte = codeBook[b].ToArray();
            output.Add(encodedByte);
        }

        return output
            .SelectMany(b => b)
            .ToArray();
    }

    // Naive implementation just to validate encode method. Switch to tree traversal at some point.
    public byte[] Decode(bool[] input, Dictionary<byte, bool[]> codeBook)
    {
        var bitsDict = codeBook.ToDictionary(kv => kv.Value.Select(v => v ? "1" : "0").Aggregate((a, b) => a + b), kv => kv.Key);
        var output = new List<byte>();
        var key = new StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            key.Append(input[i] ? "1" : "0");

            if (bitsDict.ContainsKey(key.ToString()))
            {
                output.Add(bitsDict[key.ToString()]);
                key.Clear();
            }
        }

        if (key.Length > 0)
            throw new ApplicationException("Decoding error, invalid input or codebook");

        return output.ToArray();
    }

    public Dictionary<byte, int> GetByteFrequencies(byte[] input)
    {
        if (input == null)
            return null;

        var output = new Dictionary<byte, int>();

        foreach (byte b in input)
        {
            if (output.ContainsKey(b))
                output[b]++;
            else
                output.Add(b, 1);
        }

        return output;
    }

    public HuffmanNode CreateHuffmanTree(byte[] input)
    {
        var frequencies = GetByteFrequencies(input);
        var queue = new PriorityQueue<HuffmanNode, int>();

        foreach (var entry in frequencies)
        {
            var node = new HuffmanNode
            {
                Byte = entry.Key,
                Frequency = entry.Value,
            };

            queue.Enqueue(node, node.Frequency);
        }

        while (queue.Count > 1)
        {
            var lowestFreqNode = queue.Dequeue();
            var secondLowestFreqNode = queue.Dequeue();
            var newNode = new HuffmanNode
            {
                Frequency = lowestFreqNode.Frequency + secondLowestFreqNode.Frequency,
                LeftNode = secondLowestFreqNode,
                RightNode = lowestFreqNode,
            };

            queue.Enqueue(newNode, newNode.Frequency);
        }

        return queue.Dequeue();
    }

    public void GetByteEncodings(ref Dictionary<byte, bool[]> dict, HuffmanNode node, List<bool> currentPath)
    {
        if (node.LeftNode != null)
        {
            currentPath.Add(true);
            GetByteEncodings(ref dict, node.LeftNode, currentPath);
        }

        if (node.RightNode != null)
        {
            currentPath.Add(false);
            GetByteEncodings(ref dict, node.RightNode, currentPath);
        }

        if (node.LeftNode == null && node.RightNode == null)
        {
            dict.Add(node.Byte.Value, currentPath.ToArray());
        }

        if (currentPath.Any())
            currentPath.RemoveAt(currentPath.Count - 1);
    }

    public class HuffmanNode
    {
        public byte? Byte { get; set; }
        public int Frequency { get; set; }
        public HuffmanNode LeftNode { get; set; }
        public HuffmanNode RightNode { get; set; }
    }
}
