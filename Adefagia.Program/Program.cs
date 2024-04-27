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
        TestNodeLookupLE();
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

    public static void TestNodeLookupLE()
    {
        var bNode = new BNode(new byte[4096]); // Create a BNode with empty data
        var key1 = new byte[] { 1, 2, 3 };
        var key2 = new byte[] { 4, 5, 6 };
        var key3 = new byte[] { 7, 8, 9 };

        bNode.setHeader(1, 10); // Set headers
        bNode.setPtr(0, 123456); // Set pointer at index 0

        // Set keys and values
        bNode.setOffset(1, (ushort)key1.Length);
        BitConverter.GetBytes((ushort)key1.Length).CopyTo(bNode.data, bNode.kvPos(0));
        key1.CopyTo(bNode.data, bNode.kvPos(0) + 4);

        bNode.setOffset(2, (ushort)key2.Length);
        BitConverter.GetBytes((ushort)key2.Length).CopyTo(bNode.data, bNode.kvPos(1));
        key2.CopyTo(bNode.data, bNode.kvPos(1) + 4);

        bNode.setOffset(3, (ushort)key3.Length);
        BitConverter.GetBytes((ushort)key3.Length).CopyTo(bNode.data, bNode.kvPos(2));
        key3.CopyTo(bNode.data, bNode.kvPos(2) + 4);

        // Test when key is found
        var index1 = bNode.nodeLookupLE(key2);
        Console.WriteLine($"Index of key {BitConverter.ToString(key2)}: {index1}"); // Should print Index of key 04-05-06: 1

        // Test when key is not found
        var nonExistentKey = new byte[] { 10, 11, 12 };
        var index2 = bNode.nodeLookupLE(nonExistentKey);
        Console.WriteLine($"Index of non-existent key {BitConverter.ToString(nonExistentKey)}: {index2}"); // Should print Index of non-existent key 0A-0B-0C: 0
    }
}