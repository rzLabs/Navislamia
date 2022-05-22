using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Configuration
{
    public class Configuration
    {
        public string Category;
        public Dictionary<string, object> Collection;

        public Configuration(string key) 
        {
            Category = key;
            Collection = new Dictionary<string, object>();
        }
    }

    public sealed class ConfigurationCollection : IList<Configuration>, IEnumerable<Configuration> // Children container
    {
        public string CurrentParent = null;

        List<Configuration> configurations = new List<Configuration>();

        public Configuration this[int index] 
        {
            get => configurations[index]; 
            set => configurations[index] = value; 
        }
        
        public Configuration this[string category]
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
                    foreach (var child in parent.Collection)
                        i++;

                return i;
            }
        }

        public bool IsReadOnly => ((ICollection<Configuration>)configurations).IsReadOnly;

        public void Add(Configuration item)
        {
            ((ICollection<Configuration>)configurations).Add(item);
        }

        public void AddParent(string key)
        {
            if (configurations.FindIndex(c => c.Category == key) >= 0)
                return;

            CurrentParent = key;

            configurations.Add(new Configuration(key));
        }

        public void AddChild(string key, object value)
        {
            if (CurrentParent == null)
                return;

            int index = -1;

            if ((index = configurations.FindIndex(c => c.Category == CurrentParent)) == -1)
                return;

            configurations[index].Collection.Add(key, value);
        }

        public void Clear() => configurations.Clear();

        public bool Contains(Configuration item)
        {
            return ((ICollection<Configuration>)configurations).Contains(item);
        }

        public void CopyTo(Configuration[] array, int arrayIndex)
        {
            ((ICollection<Configuration>)configurations).CopyTo(array, arrayIndex);
        }

        public IEnumerator<Configuration> GetEnumerator()
        {
            return ((IEnumerable<Configuration>)configurations).GetEnumerator();
        }

        public int IndexOf(Configuration item)
        {
            return ((IList<Configuration>)configurations).IndexOf(item);
        }

        public Configuration Find(Predicate<Configuration> match) => configurations.Find(match);

        public void Insert(int index, Configuration item)
        {
            ((IList<Configuration>)configurations).Insert(index, item);
        }

        public bool Remove(Configuration item)
        {
            return ((ICollection<Configuration>)configurations).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<Configuration>)configurations).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)configurations).GetEnumerator();
        }

        IEnumerator<Configuration> IEnumerable<Configuration>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
