using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;

namespace TestMarbles.Helpers
{
    internal static class Extensions
    {
        public static Recorded<Notification<char>> CastToChar<T>(this Recorded<Notification<T>> oldNotification)
        {
            Notification<char> newNotification;
            switch (oldNotification.Value.Kind)
            {
                case NotificationKind.OnNext:
                    newNotification = Notification.CreateOnNext(oldNotification.Value.Value.CastToChar());
                    break;
                case NotificationKind.OnError:
                    newNotification = Notification.CreateOnError<char>(oldNotification.Value.Exception);
                    break;
                case NotificationKind.OnCompleted:
                    newNotification = Notification.CreateOnCompleted<char>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new Recorded<Notification<char>>(oldNotification.Time, newNotification);
        }

        public static Recorded<Notification<char>> CastToChar<T>(
            this Recorded<Notification<T>> oldNotification, 
            IReadOnlyDictionary<T, char> dict)
        {
            Notification<char> newNotification;
            switch (oldNotification.Value.Kind)
            {
                case NotificationKind.OnNext:
                    newNotification = Notification.CreateOnNext(dict[oldNotification.Value.Value]);
                    break;
                case NotificationKind.OnError:
                    newNotification = Notification.CreateOnError<char>(oldNotification.Value.Exception);
                    break;
                case NotificationKind.OnCompleted:
                    newNotification = Notification.CreateOnCompleted<char>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new Recorded<Notification<char>>(oldNotification.Time, newNotification);
        }

        public static char CastToChar<T>(this T value)
        {
            return (char)(object)value;
        }

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

        // Modified from http://stackoverflow.com/a/22595707/1108916
        public static IDictionary<TValue, TKey> ReverseKeyValue<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            var dictionary = new Dictionary<TValue, TKey>();
            foreach (var entry in source)
            {
                if (!dictionary.ContainsKey(entry.Value))
                {
                    dictionary.Add(entry.Value, entry.Key);
                }
            }
            return dictionary;
        }

        // Modified from http://stackoverflow.com/a/22595707/1108916
        public static IReadOnlyDictionary<TValue, TKey> ReverseKeyValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> source)
        {
            var dictionary = new Dictionary<TValue, TKey>();
            foreach (var entry in source)
            {
                if (!dictionary.ContainsKey(entry.Value))
                {
                    dictionary.Add(entry.Value, entry.Key);
                }
            }
            return dictionary;
        }
    }
}
