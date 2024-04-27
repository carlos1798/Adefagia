using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Adefagia.Model
{
    /*
     | type | nkeys |  pointers  |   offsets  | key-values | unused |
     |  2B  |   2B  | nkeys * 8B | nkeys * 2B |     ...    |        |

                                            | klen | vlen | key | val |
                                            |  2B  |  2B  | ... | ... |

     */

    public class BNode
    {
        private const int HEADER = 4;
        private const int BTREE_PAGE_SIZE = 4096;
        private const int BTREE_MAX_KEY_SIZE = 1000;
        private const int BTREE_MAX_VALUE_SIZE = 3000;

        private int BTREE_MAX_NODE_SIZE;

        private const int BNODE_NODE = 1;
        private const int BNODE_LEAF = 2;

        public byte[] data;

        public BNode(byte[] data)
        {
            BTREE_MAX_NODE_SIZE = HEADER + 8 + 4 + 2 + BTREE_MAX_KEY_SIZE + BTREE_MAX_VALUE_SIZE;
            if (BTREE_MAX_NODE_SIZE > data.Length)
            {
                throw new ArgumentException($"The lenght of the data: {data.Length} is greater thand the max node size:{BTREE_MAX_NODE_SIZE} ");
            }
            this.data = data;
        }

        private void checkBoundsEquals(ulong idx)
        {
            if (idx >= nKeys())
            {
                throw new IndexOutOfRangeException($"The index: {idx} is greater or equal than the number of keys {nKeys()} ");
            }
        }

        private void checkBounds(ulong idx)
        {
            if (idx > nKeys())
            {
                throw new IndexOutOfRangeException($"The index: {idx} is greater than the number of keys {nKeys()} ");
            }
        }

        #region HEADERS

        public ushort bType()
        {
            return (ushort)BitConverter.ToUInt16(data, 0);
        }

        public ushort nKeys()
        {
            return (ushort)BitConverter.ToUInt16(data, 2);
        }

        public void setHeader(ushort btype, ushort nKeys)
        {
            BitConverter.GetBytes(btype).CopyTo(data, 0);
            BitConverter.GetBytes(nKeys).CopyTo(data, 2);
        }

        #endregion HEADERS

        #region POINTERS_CHILDS

        public ulong getPtr(ushort idx)
        {
            checkBounds(idx);

            var position = HEADER + 8 * idx;
            return BitConverter.ToUInt64(data, position);
        }

        public void setPtr(ushort idx, ulong value)
        {
            checkBounds(idx);
            var position = HEADER + 8 * idx;
            BitConverter.GetBytes(value).CopyTo(data, position);
        }

        #endregion POINTERS_CHILDS

        #region OFFSET

        public ushort offsetPos(ushort idx)
        {
            checkBounds(idx);
            Debug.Assert(1 <= idx && idx <= nKeys());
            return (ushort)(HEADER + 8 * nKeys() + 2 * (idx - 1));
        }

        public ushort getOffset(ushort idx)
        {
            if (idx == 0)
            {
                return 0;
            }
            return BitConverter.ToUInt16(data, offsetPos(idx));
        }

        public void setOffset(ushort idx, ushort offset)
        {
            BitConverter.GetBytes(offset).CopyTo(data, offsetPos(idx));
        }

        #endregion OFFSET

        #region KEY_VALUE

        public ushort kvPos(ushort idx)
        {
            checkBoundsEquals(idx);
            return (ushort)(HEADER + 8 * nKeys() + 2 * nKeys() + getOffset(idx));
        }

        public byte[] getKey(ushort idx)
        {
            checkBounds(idx);
            var position = kvPos(idx);
            var klen = BitConverter.ToUInt16(data, kvPos(idx));
            byte[] kval = new byte[klen];
            return data.Skip(position + 4).Take(klen).ToArray();
        }

        public byte[] getVal(ushort idx)
        {
            checkBounds(idx);

            var position = kvPos(idx);
            var klen = BitConverter.ToUInt16(data, position);
            var vLen = BitConverter.ToUInt16(data, position + 2);
            return data.Skip(position + klen + 4).Take(vLen).ToArray();
        }

        #endregion KEY_VALUE

        public ushort nodeLookupLE(byte[] key)
        {
            var keys = nKeys();
            for (ushort i = 1; i < keys; i++)
            {
                if (getKey(i).Equals(key))
                {
                    return i;
                }
            }
            return 0;
        }

        public void leafInsert(ushort idx, byte[] key, byte[] value)
        {
            BNode newN = (BNode)this.MemberwiseClone();
        }

        public ushort nBytes()
        {
            return kvPos(nKeys());
        }
    }
}