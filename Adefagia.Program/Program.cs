using System.Buffers;
using System.Diagnostics;
using Adefagia.Model;

internal class Program
{
    private const int HEADER = 4;
    private const int BTREE_PAGE_SIZE = 4096;
    private const int BTREE_MAX_KEY_SIZE = 1000;
    private const int BTREE_MAX_VALUE_SIZE = 3000;

    public static void Main(string[] args)
    {
        TestHeaders();
        TestPointers();
        TestOffsets();
        TestKeyValue();
    }

    public static void TestHeaders()
    {
        // Test setting and getting headers
        var bNode = new BNode(new byte[4096]); // Create a BNode with empty data
        bNode.setHeader(1, 10); // Set headers
        Console.WriteLine($"bType: {bNode.bType()}, nKeys: {bNode.nKeys()}"); // Should print bType: 1, nKeys: 10
    }

    public static void TestPointers()
    {
        // Test setting and getting pointers
        var bNode = new BNode(new byte[4096]); // Create a BNode with empty data

        bNode.setHeader(1, 10); // Set headers
        bNode.setPtr(0, 123456); // Set pointer at index 0
        Console.WriteLine($"Pointer at index 0: {bNode.getPtr(0)}"); // Should print Pointer at index 0: 123456
    }

    public static void TestOffsets()
    {
        // Test setting and getting offsets
        var bNode = new BNode(new byte[4096]); // Create a BNode with empty data
        bNode.setHeader(1, 10); // Set headers
        bNode.setPtr(0, 123456); // Set pointer at index 0
        bNode.setOffset(1, 100); // Set offset at index 1
        Console.WriteLine($"Offset at index 1: {bNode.getOffset(1)}"); // Should print Offset at index 1: 100
    }

    public static void TestKeyValue()
    {
        // Test getting key-value pairs
        var bNode = new BNode(new byte[4096]); // Create a BNode with empty data
        var key = new byte[] { 1, 2, 3 }; // Sample key
        var val = new byte[] { 4, 5, 6, 7 }; // Sample value
                                             // Set key-value pair at index 0
        bNode.setHeader(1, 10); // Set headers
        bNode.setPtr(0, 123456); // Set pointer at index 0
        bNode.setOffset(1, 100); // Set offset at index 1

        bNode.setOffset(1, (ushort)key.Length);
        BitConverter.GetBytes((ushort)key.Length).CopyTo(bNode.data, bNode.kvPos(0));
        key.CopyTo(bNode.data, bNode.kvPos(0) + 4);
        BitConverter.GetBytes((ushort)val.Length).CopyTo(bNode.data, bNode.kvPos(0) + 2);
        val.CopyTo(bNode.data, bNode.kvPos(0) + 4 + key.Length);
        Console.WriteLine($"Key at index 0: {BitConverter.ToString(bNode.getKey(0))}, Value at index 0: {BitConverter.ToString(bNode.getVal(0))}");
        // Should print Key at index 0: 01-02-03, Value at index 0: 04-05-06-07
    }
}