using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Entities
{
    public class CString
    {
        public int Length => Data.Length;

        public string String
        {
            get => Encoding.Default.GetString(Data).Trim('\0');
            set
            {
                byte[] buffer = Encoding.Default.GetBytes(value);

                for (int i = 0; i < buffer.Length; i++)
                    Data[i] = buffer[i];
            }
        }

        public byte[] Data = new byte[0];

        public CString(int length) => Data = new byte[length];

    }
}
