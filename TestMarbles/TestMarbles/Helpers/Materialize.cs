using System.Collections.Generic;

namespace TestMarbles.Helpers
{
    internal static partial class Helpers
    {
        public static IEnumerable<MaterializedEnumerable<T>> Materialize<T>(this IEnumerable<T> self)
        {
            using (var enumerator = self.GetEnumerator())
            {
                bool isFirst = true;
                if (enumerator.MoveNext())
                {
                    bool isLast;
                    do
                    {
                        var current = enumerator.Current;
                        isLast = !enumerator.MoveNext();
                        yield return new MaterializedEnumerable<T>(current, isFirst, isLast);
                        isFirst = false;
                    } while (!isLast);
                }
            }
        }
    }

    internal class MaterializedEnumerable<T>
    {
        public MaterializedEnumerable(T value, bool isFirst, bool isLast)
        {
            IsFirst = isFirst;
            IsLast = isLast;
            Value = value;
        }

        public bool IsFirst { get; }

        public bool IsLast { get; }

        public T Value { get; }
    }
}