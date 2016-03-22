using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMarbles.Helpers
{
    internal class FakeDictionary<T> : IReadOnlyDictionary<char, T>
    {
        public T this[char key] => (T)(object)key;

        public int Count => 0;

        public IEnumerable<char> Keys => Enumerable.Empty<char>();

        public IEnumerable<T> Values => Enumerable.Empty<T>();

        public bool ContainsKey(char key) { return true; }

        public IEnumerator<KeyValuePair<char, T>> GetEnumerator()
        {
            return Enumerable.Empty<KeyValuePair<char, T>>().GetEnumerator();
        }

        public bool TryGetValue(char key, out T value)
        {
            try
            {
                value = (T)(object)key;
                return true;
            }
            catch (Exception)
            {
                value = default(T);
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
