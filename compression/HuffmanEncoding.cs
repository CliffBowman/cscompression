namespace compression;

public class HuffmanEncoding
{
    public bool[] Encode(byte[] input, ref Dictionary<byte, bool[]> encodingDict)
    {
        HuffmanNode treeRoot = CreateHuffmanTree(input);
        GetByteEncodings(ref encodingDict, treeRoot, new List<bool>());
        List<bool[]> output = new();

        foreach (var b in input)
        {
            var encodedByte = encodingDict[b].ToArray();
            output.Add(encodedByte);
        }

        return output
            .SelectMany(b => b)
            .ToArray();
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
