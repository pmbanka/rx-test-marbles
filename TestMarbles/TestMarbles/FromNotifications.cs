using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using Microsoft.Reactive.Testing;
using TestMarbles.Helpers;
using TestMarbles.Utils;

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
                : notifications.Select(p => p.ConvertToChar(values));
            string marblesOrError;
            TryFromNotifications(modifiedNotifications, out marblesOrError, out marblesOrError);
            return marblesOrError;
        }

        public static bool TryFromNotifications<T>(
            IEnumerable<Recorded<Notification<T>>> notifications,
            IReadOnlyDictionary<T, char> values,
            out string marbles,
            out string errorMessage)
        {
            try
            {
                errorMessage = null;
                marbles = FromNotifications(notifications, values);
                return true;
            }
            catch (Exception ex)
            {
                marbles = null;
                errorMessage = $"Could not generate marbles: {ex.Message}";
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
                errorMessage = null;
                marbles = FromNotifications(notifications);
                return true;
            }
            catch (Exception ex)
            {
                marbles = null;
                errorMessage = $"Could not generate marbles: {ex.Message}";
                return false;
            }
        }

        public static string FromNotifications<T>(
            IEnumerable<Recorded<Notification<T>>> notifications,
            IReadOnlyDictionary<T, char> values)
        {
            Ensure.NotNull(notifications, nameof(notifications));
            Ensure.NotNull(values, nameof(values));
            Ensure.NotContainsMarkers(values, nameof(values));
            var modifiedNotifications = notifications.Select(p => p.ConvertToChar(values));
            return FromNotifications(modifiedNotifications);
        }

        public static string FromNotifications(IEnumerable<Recorded<Notification<char>>> notifications)
        {
            Ensure.NotNull(notifications, nameof(notifications));
            var input = notifications as IList<Recorded<Notification<char>>> ?? notifications.ToList();
            Ensure.NotContainsMarkers(input, nameof(notifications));
            var builder = new StringBuilder();
            var entries = input
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
                if (notification.Time%MarbleScheduler.FrameTimeFactor != 0)
                {
                    var message =
                        $"Notifications cannot have times not being a multiple of {MarbleScheduler.FrameTimeFactor} (in that case, {notification.Time})";
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
            var d = (notification.Time - lastNotificationTime - (isFirst ? 0 : MarbleScheduler.FrameTimeFactor))/MarbleScheduler.FrameTimeFactor;
            var dashes = (int) Math.Max(d, 0);
            return dashes;
        }
    }
}