namespace TestMarbles.Helpers
{
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