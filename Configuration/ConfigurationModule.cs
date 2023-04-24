using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Configuration
{
    public class ConfigurationModule : IConfigurationService, IEnumerable<ConfigurationCollection>
    {
        public ConfigurationModule() => Load();

        readonly string configName = "Configuration.json";

        public ConfigurationCollection Configurations = new ConfigurationCollection();

        public int Count => Configurations.Count;

        public int TotalCount => Configurations.TotalCount;

        public void CreateDefault(string path = null)
        {
            Configurations.Clear(); // just incase ;)

            // server

            Configurations.AddParent("server");


            Configurations.AddChild("name", "Navislamia");
            Configurations.AddChild("index", "0");
            Configurations.AddChild("screenshort.url", "about:new");
            Configurations.AddChild("adult", false);


            Configurations.AddParent("logs");

            // logs

            Configurations.AddChild("minimum-level", 0);
            Configurations.AddChild("packet.debug", false); 

            // network

            Configurations.AddParent("network");

            Configurations.AddChild("io.auth.ip", "127.0.0.1");
            Configurations.AddChild("io.auth.port", 4502);
            Configurations.AddChild("auth.server_idx", 1);
            Configurations.AddChild("io.upload.ip", "127.0.0.1");
            Configurations.AddChild("io.upload.port", 4616);
            Configurations.AddChild("io.ip", "127.0.0.1");
            Configurations.AddChild("io.port", 4515);
            Configurations.AddChild("io.backlog", 100);
            Configurations.AddChild("io.buffer_size", 32768);
            Configurations.AddChild("cipher.key", "}h79q~B%al;k'y $E");


            // database

            Configurations.AddParent("database");

            Configurations.AddChild("world.ip", "127.0.0.1");
            Configurations.AddChild("world.port", 1433);
            Configurations.AddChild("world.trusted_connection", true);
            Configurations.AddChild("world.name", "Arcadia");
            Configurations.AddChild("world.user", "sa");
            Configurations.AddChild("world.user.pass", "");
            Configurations.AddChild("player.ip", "127.0.0.1");
            Configurations.AddChild("player.port", 1433);
            Configurations.AddChild("player.trusted_connection", true);
            Configurations.AddChild("player.name", "Telecaster");
            Configurations.AddChild("player.user", "sa");
            Configurations.AddChild("player.user.pass", "");

            // scripts

            Configurations.AddParent("scripts");

            Configurations.AddChild("directory", ".\\Scripts");


            // maps

            Configurations.AddParent("maps");

            Configurations.AddChild("directory", ".\\Maps");
            Configurations.AddChild("width", 700000);
            Configurations.AddChild("height", 1000000);
            Configurations.AddChild("max_layer", 256);
            Configurations.AddChild("skip_loading", false);
            Configurations.AddChild("skip_loading_nfa", false);
            Configurations.AddChild("no_collision_check", false);

            // TODO: game

            Save();
        }

        public T Get<T>(string key, string parent, object defaultValue = null)
        {
            var category = Configurations.Find(c => c.Category == parent);

            if (category is null)
                return default(T);

            var val = (category.Collection.ContainsKey(key)) ? category.Collection[key] : defaultValue ?? default(T);

            return (T)Convert.ChangeType(val, typeof(T));
        }

        public object Get(string key, string parent, object defaultValue = null)
        {
            var category = Configurations.Find(c => c.Category == parent);

            if (category is null)
                return default;

            return (category.Collection.ContainsKey(key)) ? category.Collection[key] : defaultValue ?? default;
        }

        public string GetDirectory(string key, string parent, string defaultValue = null)
        {
            var category = Configurations.Find(c=>c.Category == parent);

            if (category is null)
                return default;

                return (category.Collection.ContainsKey(key)) ? category.Collection[key].ToString() : defaultValue ?? default;
        }

        public void Set(string key, string parent = null, object value = null)
        {
            var category = Configurations.Find(c => c.Category == parent);

            if (category is null)
                Configurations.Add(new Configuration(parent));

            Configurations[parent].Collection.Add(key, value);
        }

        public bool Load(string path = null)
        {
            var workingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            
            string filename = path ?? $"{workingDir}//{configName}";

            if (!File.Exists(filename))
                return false;

            string jsonStr = File.ReadAllText(filename);

            if (!string.IsNullOrEmpty(jsonStr))
            {
                Configurations = JsonConvert.DeserializeObject<ConfigurationCollection>(jsonStr);

#if DEBUG
                var runtimeCfg = new Configuration("Runtime");
                runtimeCfg.Collection.Add("debug", true);
                Configurations.Add(runtimeCfg);
#endif

                return Configurations.Count > 0;
            }

            return false;
        }

        public void Save(string path = null)
        {
            string filename = path ?? $"{Directory.GetCurrentDirectory()}\\Configuration.json";

            var serializer = new JsonSerializer();

            serializer.Formatting = Formatting.Indented;

            using (StreamWriter sw = new StreamWriter(filename))
                using (JsonWriter jw = new JsonTextWriter(sw))
                    serializer.Serialize(jw, Configurations);
        }

        public IEnumerator<ConfigurationCollection> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
