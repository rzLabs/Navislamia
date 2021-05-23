using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Events
{
    public class ExceptionArgs : EventArgs
    {
        public Exception Exception;

        public ExceptionArgs(Exception e) => Exception = e;
    }
}
