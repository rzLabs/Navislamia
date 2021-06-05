using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Data.Monster
{
    public enum TriggerType
    {
        None,
        HpBelowOnce,
        HpBelowAlways,
        TimeOnce,
        TimeAlways,
        TimeAlwaysIncludeStart
    }
}
