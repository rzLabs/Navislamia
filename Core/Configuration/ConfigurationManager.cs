using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Navislamia.Configuration.Structures;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System;

#pragma warning disable CS1998

namespace Navislamia.Configuration
{
    //TODO: implement setting call with default value!

    /// <summary>
    /// Provides configuration storage and real time manipulation
    /// </summary>
    public class ConfigurationManager
    {
        public static ConfigurationManager Instance;

        /// <summary>
        /// List of Configuration descriptions
        /// </summary>
        public List<Setting> Settings = new List<Setting>();

        string confDir = Directory.GetCurrentDirectory();
        string confName = Defaults.ConfigName;
        string _confPath = string.Empty;

        string confPath
        {
            get
            {
                if (string.IsNullOrEmpty(_confPath))
                    _confPath = $"{confDir}\\{confName}";

                return _confPath;
            }
            set
            {
                _confPath = value;
                confDir = Path.GetDirectoryName(_confPath);
                confName = Path.GetFileName(_confPath);
            }
        }

        string confText = string.Empty;

        /// <summary>
        /// Fully qualified path to the directory holding the configuration .json
        /// </summary>
        public string FolderName => confDir;

        /// <summary>
        /// Fully qualified path to the configuration .json (includes FolderName)
        /// </summary>
        public string FileName => confPath;

        /// <summary>
        /// Count of the elements in the Settings property
        /// </summary>
        public int Count => Settings.Count;

        /// <summary>
        /// Default constructor which will use default path variables
        /// </summary>
        public ConfigurationManager() => Instance = this;

        /// <summary>
        /// Constructor for initializing with a directory and file name.
        /// </summary>
        /// <param name="Directory">Directory holding the configuration .json</param>
        /// <param name="FileName">Name of the configuration .json</param>
        public ConfigurationManager(string Directory, string FileName)
        {
            confDir = Directory;
            confName = FileName;

            Instance = this;
        }

        /// <summary>
        /// Constructor for initializing with a fully qualified file path to the config .json
        /// </summary>
        /// <param name="FilePath"></param>
        public ConfigurationManager(string FilePath)
        {
            confPath = FilePath;

            Instance = this;
        }

        public dynamic this[int index] => Settings?[index];

        public dynamic this[string key]
        {
            get
            {
                int index = Settings.FindIndex(o => o.Name == key);
                if (index >= 0)
                    return Settings[index].Value;
                else
                    throw new IndexOutOfRangeException();
            }
            set
            {
                int index = Settings.FindIndex(o => o.Name == key);

                if (index >= 0)
                    Settings[index].Value = value;
                else
                    throw new IndexOutOfRangeException();
            }
        }

        public dynamic this[string key, string parent]
        {
            get
            {
                int index = Settings.FindIndex(o => o.Name == key && o.Parent == parent);
                if (index >= 0)
                    return Settings[index].Value;
                else
                    throw new IndexOutOfRangeException();
            }
            set
            {
                int index = Settings.FindIndex(o => o.Name == key && o.Parent == parent);

                if (index >= 0)
                    Settings[index].Value = value;
                else
                    throw new IndexOutOfRangeException();
            }
        }

        public T Get<T>(string key, string parent)
        {
            Setting s = null;

            if ((s = Settings.Find(s => s.Name == key && s.Parent == parent)) != null)
                return (T)s.Value;

            return default(T);
        }

        public dynamic GetSetting(int index) => Settings?[index];

        public dynamic GetSetting(string key) => Settings.Find(o => o.Name == key);

        public dynamic GetSetting(string key, string parent) => Settings.Find(o => o.Name == key && o.Parent == parent);

        public string GetDirectory(string key, string parent = null)
        {
            Setting opt = (parent != null) ? GetSetting(key, parent) : GetSetting(key);

            string optPath = opt.Value;
            bool isRelative = !Path.IsPathRooted(optPath);

            if (!string.IsNullOrEmpty(optPath))
                if (!isRelative)
                    return optPath;
                else
                    return Path.GetFullPath(optPath);

            return null;
        }

        public bool Initialize()
        {
            if (File.Exists(confPath))
                confText = File.ReadAllText(confPath);
            else
                throw new FileNotFoundException("File not found");

            JObject grandParent = JObject.Parse(confText);

            if (grandParent.Count > 0) //Gets root of json as JObject
            {
                Settings.Clear();

                foreach (JToken parent in grandParent.Children())
                    foreach (JToken child in parent.Children())
                        foreach (JToken grandChild in child.Children())
                        {
                            JProperty property = grandChild.Value<dynamic>();

                            Settings.Add(new Setting()
                            {
                                Parent = property.Parent.Path,
                                Name = property.Name,
                                Type = property.Value.GetType(),
                                Value = property.Value,
                                Comments = new List<string>()
                            });
                        }
            }

            return true;
        }

        public void Save()
        {
            JObject rss = new JObject();

            foreach (string parent in parents)
            {
                JProperty jParent = null;

                JObject jChildren = new JObject();

                foreach (Setting child in Settings.FindAll(c => c.Parent == parent))
                    jChildren.Add(new JProperty(child.Name, child.Value));

                jParent = new JProperty(parent, jChildren);

                if (!rss.ContainsKey(parent))
                    rss.Add(jParent);
            }

            if (rss.Count > 0)
                using (StreamWriter sw = new StreamWriter("Config.json", false, Encoding.Default))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(sw, rss);
                }
        }

        string[] parents
        {
            get
            {
                string[] p = new string[Settings.Count];

                for (int i = 0; i < p.Length; i++)
                    p[i] = Settings[i].Parent;

                return p;
            }


        }

        Setting[] getChildren(string parent) => Settings.FindAll(o => o.Parent == parent).ToArray();
    }
}
