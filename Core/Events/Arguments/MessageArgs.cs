using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Events
{
    public class MessageArgs : EventArgs
    {
        public string Message;

        public MessageArgs(string message) => Message = message;
    }
}
