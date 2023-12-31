using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Navislamia.Game.Filer
{
    public class KStream : MemoryStream
    {
        public KStream(string fileName) : base(File.ReadAllBytes(fileName)) { }

        public ushort ReadUShort()
        {
            if (readEOF(sizeof(ushort)))
                throw new EndOfStreamException();

            byte[] buffer = new byte[sizeof(ushort)];
            base.Read(buffer, 0, buffer.Length);

            return BitConverter.ToUInt16(buffer);
        }

        public int ReadInt()
        {
            if (readEOF(sizeof(int)))
                throw new EndOfStreamException();

            byte[] buffer = new byte[sizeof(int)];
            base.Read(buffer, 0, buffer.Length);

            return BitConverter.ToInt32(buffer);    
        }

        public long ReadLong()
        {
            if (readEOF(sizeof(long)))
                throw new EndOfStreamException();

            byte[] buffer = new byte[sizeof(long)];
            base.Read(buffer, 0, buffer.Length);

            return BitConverter.ToInt64(buffer);
        }

        public float ReadFloat()
        {
            if (readEOF(sizeof(float)))
                throw new EndOfStreamException();

            byte[] buffer = new byte[sizeof(float)];
            base.Read(buffer, 0, buffer.Length);

            return BitConverter.ToSingle(buffer);
        }

        public string ReadString(int length)
        {
            if (readEOF(length))
                throw new EndOfStreamException();

            byte[] buffer = new byte[length];
            base.Read(buffer, 0, buffer.Length);

            return Encoding.Default.GetString(buffer).Trim('\0');
        }

        bool readEOF(int length) => base.Position + length > base.Length;
    }
}
