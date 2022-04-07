using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Data
{
    public interface IDataService
    {
        public T Get<T>(string key);

        public void Set<T>(string key, object obj);
    }
}
