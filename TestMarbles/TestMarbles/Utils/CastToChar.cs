using System;
using System.Collections.Generic;
using System.Reactive;
using Microsoft.Reactive.Testing;

namespace TestMarbles.Helpers
{
    internal static partial class Helpers
    {
        public static Recorded<Notification<char>> CastToChar<T>(this Recorded<Notification<T>> oldNotification)
        {
            Notification<char> newNotification;
            switch (oldNotification.Value.Kind)
            {
                case NotificationKind.OnNext:
                    newNotification = Notification.CreateOnNext((char)(object)oldNotification.Value.Value);
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

        public static Recorded<Notification<char>> ConvertToChar<T>(
            this Recorded<Notification<T>> oldNotification, 
            IReadOnlyDictionary<T, char> dict)
        {
            Notification<char> newNotification;
            switch (oldNotification.Value.Kind)
            {
                case NotificationKind.OnNext:
                    T oldValue = oldNotification.Value.Value;
                    char newValue;
                    if (!dict.TryGetValue(oldValue, out newValue))
                    {
                        throw new KeyNotFoundException($"Convertion to char not found for notification {oldNotification}");
                    }
                    newNotification = Notification.CreateOnNext(newValue);
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
    }
}
