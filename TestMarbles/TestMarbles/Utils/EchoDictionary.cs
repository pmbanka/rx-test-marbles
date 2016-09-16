using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMarbles.Helpers
{
    internal class EchoDictionary<T> : IReadOnlyDictionary<T, T>
    {
        public T this[T key] => key;

        public int Count => 0;

        public IEnumerable<T> Keys => Enumerable.Empty<T>();

        public IEnumerable<T> Values => Enumerable.Empty<T>();

        public bool ContainsKey(T key) { return true; }

        public IEnumerator<KeyValuePair<T, T>> GetEnumerator()
        {
            return Enumerable.Empty<KeyValuePair<T, T>>().GetEnumerator();
        }

        public bool TryGetValue(T key, out T value)
        {
            value = key;
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
