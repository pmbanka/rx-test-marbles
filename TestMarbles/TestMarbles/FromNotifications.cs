using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using Microsoft.Reactive.Testing;
using TestMarbles.Helpers;

namespace TestMarbles
{
    public static partial class Marbles
    {
        internal static string GetMarblesOrErrorMessage<T>(
            IEnumerable<Recorded<Notification<T>>> notifications,
            IReadOnlyDictionary<T, char> values)
        {
            var modifiedNotifications = values == null 
                ? notifications.Select(p => p.CastToChar()) 
                : notifications.Select(p => p.CastToChar(values));
            return FromNotifications(modifiedNotifications);
        }

        public static bool TryFromNotifications<T>(
            IEnumerable<Recorded<Notification<T>>> notifications,
            IReadOnlyDictionary<T, char> values,
            out string marbles,
            out string errorMessage)
        {
            try
            {
                marbles = FromNotifications(notifications, values);
                errorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Could not generate marbles: {ex.Message}";
                marbles = null;
                return false;
            }
        }

        public static bool TryFromNotifications(
            IEnumerable<Recorded<Notification<char>>> notifications,
            out string marbles,
            out string errorMessage)
        {
            try
            {
                marbles = FromNotifications(notifications);
                errorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Could not generate marbles: {ex.Message}";
                marbles = null;
                return false;
            }
        }

        public static string FromNotifications<T>(
            IEnumerable<Recorded<Notification<T>>> notifications,
            IReadOnlyDictionary<T, char> values)
        {
            if (notifications == null)
            {
                throw new ArgumentNullException(nameof(notifications));
            }
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            var modifiedNotifications = notifications.Select(p => p.CastToChar(values));
            return FromNotifications(modifiedNotifications);
        }

        public static string FromNotifications(IEnumerable<Recorded<Notification<char>>> notifications)
        {
            if (notifications == null)
            {
                throw new ArgumentNullException(nameof(notifications));
            }
            var builder = new StringBuilder();
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
                    var message =
                        $"Notifications cannot have times not being a multiple of {Constants.FrameTimeFactor} (in that case, {notification.Time})";
                    throw new ArgumentException(message, nameof(notifications));
                }
                var dashes = GetNumberOfDashes(notification, lastNotificationTime, entry.IsFirst);
                builder.Append(Marker.Nop1, dashes);
                if (entry.Value.IsGroupStart)
                {
                    builder.Append(Marker.GroupStart);
                }
                switch (notification.Value.Kind)
                {
                    case NotificationKind.OnCompleted:
                        builder.Append(Marker.Completed);
                        break;
                    case NotificationKind.OnError:
                        builder.Append(Marker.Error);
                        break;
                    case NotificationKind.OnNext:
                        builder.Append(notification.Value.Value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (entry.Value.IsGroupEnd)
                {
                    builder.Append(Marker.GroupEnd);
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