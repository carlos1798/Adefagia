using System.ComponentModel.Design.Serialization;

namespace Adefagia.Model
{
    internal class BTree
    {
        public ulong root;
        public Func<ulong, byte[]> get;
        public Func<byte[], ulong> newP;
        public Func<ulong> del;
    }
}