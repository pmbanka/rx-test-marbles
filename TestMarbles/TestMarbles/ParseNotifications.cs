using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using Microsoft.Reactive.Testing;
using TestMarbles.Helpers;

namespace TestMarbles
{
    public static partial class TestSchedulerEx
    {
        internal static string ParseNotificationsAllowingNullDict<T>(
            IEnumerable<Recorded<Notification<T>>> notifications,
            IReadOnlyDictionary<T, char> values)
        {
            var modifiedNotifications = values == null 
                ? notifications.Select(p => p.CastToChar()) 
                : notifications.Select(p => p.CastToChar(values));
            return ParseNotifications(modifiedNotifications);
        }

        public static string ParseNotifications<T>(
            IEnumerable<Recorded<Notification<T>>> notifications,
            IReadOnlyDictionary<T, char> values)
        {
            var modifiedNotifications = notifications.Select(p => p.CastToChar(values));
            return ParseNotifications(modifiedNotifications);
        }

        public static string ParseNotifications(IEnumerable<Recorded<Notification<char>>> notifications)
        {
            var builder = new StringBuilder("");
            var entries = notifications
                .GroupBy(n => n.Time)
                .SelectMany(group => group
                    .Materialize()
                    .Select(n =>
                    {
                        var isGroup = !(n.IsFirst && n.IsLast);
                        return new
                        {
                            IsGroupStart = isGroup && n.IsFirst,
                            IsGroupEnd = isGroup && n.IsLast,
                            Notification = n.Value,
                        };
                    }))
                .Materialize();
            long lastNotificationTime = 0;
            foreach (var entry in entries)
            {
                var notification = entry.Value.Notification;
                if (notification.Time%Constants.FrameTimeFactor != 0)
                {
                    throw new ArgumentException(
                        $"Notifications cannot have times not being a multiple of {Constants.FrameTimeFactor} (in that case, {notification.Time})",
                        nameof(notifications));
                }
                var dashes = GetNumberOfDashes(notification, lastNotificationTime, entry.IsFirst);
                builder.Append('-', dashes);
                if (entry.Value.IsGroupStart)
                {
                    builder.Append('(');
                }
                switch (notification.Value.Kind)
                {
                    case NotificationKind.OnCompleted:
                        builder.Append('|');
                        break;
                    case NotificationKind.OnError:
                        builder.Append('#');
                        break;
                    case NotificationKind.OnNext:
                        builder.Append(notification.Value.Value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (entry.Value.IsGroupEnd)
                {
                    builder.Append(')');
                }
                lastNotificationTime = notification.Time;
            }
            return builder.ToString();
        }

        private static int GetNumberOfDashes(Recorded<Notification<char>> notification, long lastNotificationTime, bool isFirst)
        {
            var d = (notification.Time - lastNotificationTime - (isFirst ? 0 : Constants.FrameTimeFactor))/Constants.FrameTimeFactor;
            var dashes = (int) Math.Max(d, 0);
            return dashes;
        }
    }
}