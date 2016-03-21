using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
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
    }
}
