using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Configuration
{
    public struct ConfigurationChild
    {
        public string Name;
        public object Value;

        public ConfigurationChild(string key, object value)
        {
            Name = key;
            Value = value;
        }
    }

    public class ConfigurationParent
    {
        public string Category;
        public List<ConfigurationChild> Configurations;

        public ConfigurationParent(string key) 
        {
            Category = key;
            Configurations = new List<ConfigurationChild>();
        }
    }

    public sealed class ConfigurationCollection : IList<ConfigurationParent>, IEnumerable<ConfigurationChild> // Children container
    {
        public string CurrentParent = null;

        List<ConfigurationParent> configurations = new List<ConfigurationParent>();

        public ConfigurationParent this[int index] 
        {
            get => configurations[index]; 
            set => configurations[index] = value; 
        }
        
        public ConfigurationParent this[string category]
        {
            get
            {
                int index = configurations.FindIndex(p => p.Category == category);

                return configurations?[index];            
            }
        }

        public int Count => configurations.Count;

        public int TotalCount
        {
            get
            {
                int i = 0;

                foreach (var parent in configurations)
                    foreach (var child in parent.Configurations)
                        i++;

                return i;
            }
        }

        public bool IsReadOnly => ((ICollection<ConfigurationParent>)configurations).IsReadOnly;

        public void Add(ConfigurationParent item)
        {
            ((ICollection<ConfigurationParent>)configurations).Add(item);
        }

        public void AddParent(string key)
        {
            if (configurations.FindIndex(c => c.Category == key) >= 0)
                return;

            CurrentParent = key;

            configurations.Add(new ConfigurationParent(key));
        }

        public void AddChild(string key, object value)
        {
            if (CurrentParent == null)
                return;

            int index = -1;

            if ((index = configurations.FindIndex(c => c.Category == CurrentParent)) == -1)
                return;

            configurations[index].Configurations.Add(new ConfigurationChild(key, value));
        }

        public void Clear() => configurations.Clear();

        public bool Contains(ConfigurationParent item)
        {
            return ((ICollection<ConfigurationParent>)configurations).Contains(item);
        }

        public void CopyTo(ConfigurationParent[] array, int arrayIndex)
        {
            ((ICollection<ConfigurationParent>)configurations).CopyTo(array, arrayIndex);
        }

        public IEnumerator<ConfigurationParent> GetEnumerator()
        {
            return ((IEnumerable<ConfigurationParent>)configurations).GetEnumerator();
        }

        public int IndexOf(ConfigurationParent item)
        {
            return ((IList<ConfigurationParent>)configurations).IndexOf(item);
        }

        public ConfigurationParent Find(Predicate<ConfigurationParent> match) => configurations.Find(match);

        public void Insert(int index, ConfigurationParent item)
        {
            ((IList<ConfigurationParent>)configurations).Insert(index, item);
        }

        public bool Remove(ConfigurationParent item)
        {
            return ((ICollection<ConfigurationParent>)configurations).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<ConfigurationParent>)configurations).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)configurations).GetEnumerator();
        }

        IEnumerator<ConfigurationChild> IEnumerable<ConfigurationChild>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
