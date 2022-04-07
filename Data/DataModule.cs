using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Data
{
    public class DataModule : IDataService
    {
        Dictionary<string, object> storage = new Dictionary<string, object>();

        public DataModule() { }

        public T Get<T>(string key)
        {
            if (storage.ContainsKey(key))
                return (T)storage[key];

            return default(T);
        }

        public void Set<T>(string key, object obj)
        {
            if (storage.ContainsKey(key))
                return;

            storage[key] = obj;
        }
    }
}
