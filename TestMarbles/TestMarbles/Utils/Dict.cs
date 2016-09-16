using System.Collections.Generic;

namespace TestMarbles.Utils
{
    public static class Dict
    {
        public static Dictionary<char, T> Empty<T>()
        {
            return new Dictionary<char, T>();
        }

        public static Dictionary<char,T> Map<T>(char marker, T value)
        {
            return new Dictionary<char, T> {{marker, value}};
        }

        public static Dictionary<char, T> Map<T>(char marker1, T value1, char marker2, T value2)
        {
            return new Dictionary<char, T> { { marker1, value1 }, { marker2, value2 } };
        }

        public static Dictionary<char, T> Map<T>(char marker1, T value1, char marker2, T value2, char marker3, T value3)
        {
            return new Dictionary<char, T> { { marker1, value1 }, { marker2, value2 }, { marker3, value3 } };
        }

        public static Dictionary<char, T> Map<T>(char marker1, T value1, char marker2, T value2, char marker3, T value3, char marker4, T value4)
        {
            return new Dictionary<char, T> { { marker1, value1 }, { marker2, value2 }, { marker3, value3 }, { marker4, value4 } };
        }

        public static Dictionary<char, T> Map<T>(char marker1, T value1, char marker2, T value2, char marker3, T value3, char marker4, T value4, char marker5, T value5)
        {
            return new Dictionary<char, T> { { marker1, value1 }, { marker2, value2 }, { marker3, value3 }, { marker4, value4 }, {marker5, value5} };
        }
    }
}