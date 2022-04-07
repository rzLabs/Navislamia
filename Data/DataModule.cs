using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Data
{
    public class DataModule : Autofac.Module, IDataService
    {
        Dictionary<string, object> storage = new Dictionary<string, object>();

        public DataModule() { }

        public T Get<T>(string key)
        {
            if (storage.ContainsKey(key))
                return (T)storage[key];

            return default(T);
        }

        public void Set<T>(string key, object obj) => storage.Add(key, obj);

        protected override void Load(ContainerBuilder builder)
        {
            var serviceTypes = Directory.EnumerateFiles(Environment.CurrentDirectory)
                .Where(filename => filename.Contains("Modules") && filename.EndsWith("Data.dll"))
                .Select(filepath => Assembly.LoadFrom(filepath))
                .SelectMany(assembly => assembly.GetTypes()
                .Where(type => typeof(IDataService).IsAssignableFrom(type) && type.IsClass));

            foreach (var serviceType in serviceTypes)
                builder.RegisterType(serviceType).As<IDataService>();
        }
    }
}
