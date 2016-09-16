﻿using System;
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
        internal static string TryGetMarblesAcceptingNullDict<T>(
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

        public static string TryFromNotifications<T>(
            IEnumerable<Recorded<Notification<T>>> notifications,
            IReadOnlyDictionary<T, char> values)
        {
            try
            {
                return FromNotifications(notifications, values);
            }
            catch (Exception)
            {
                return null;
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
            var groups = input
                .GroupBy(n => n.Time)
                .ToList();
            ValidateGroups(groups);
            if (!groups.Any())
            {
                return "";
            }
            var builder = new StringBuilder();
            long currentTime = 0;
            while (currentTime <= groups.Last().Key)
            {
                var group = groups.SingleOrDefault(g => g.Key == currentTime);
                var appended = group == null ? "-" : ToString(group);
                builder.Append(appended);
                var timeIncrement = appended.Length*MarbleScheduler.FrameTimeFactor;
                if (groups.Any(g => g.Key > currentTime && g.Key < timeIncrement))
                {
                    throw new ArgumentException("Groups cannot overlap.");
                }
                currentTime += timeIncrement;
            }
            return builder.ToString();
        }

        private static void ValidateGroups(List<IGrouping<long, Recorded<Notification<char>>>> groups)
        {
            var errorGroup = groups.FirstOrDefault(@group => @group.Key%MarbleScheduler.FrameTimeFactor != 0);
            if (errorGroup != null)
            {
                var message =
                    $"Notifications cannot have times not being a multiple of {MarbleScheduler.FrameTimeFactor} (in that case, {errorGroup.Key})";
                throw new ArgumentException(message);
            }


        }

        private static string ToString(IGrouping<long, Recorded<Notification<char>>> maybeGroup)
        {
            return maybeGroup.Count() == 1 
                ? ToMarker(maybeGroup.Single()).ToString() 
                : GroupToString(maybeGroup);
        }

        private static string GroupToString(IEnumerable<Recorded<Notification<char>>> group)
        {
            return Marker.GroupStart + string.Join("", group.Select(ToMarker)) + Marker.GroupEnd;
        }

        private static char ToMarker(Recorded<Notification<char>> notification)
        {
            switch (notification.Value.Kind)
            {
                case NotificationKind.OnCompleted:
                    return Marker.Completed;
                case NotificationKind.OnError:
                    return Marker.Error;
                case NotificationKind.OnNext:
                    return notification.Value.Value;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //public static string FromNotifications(IEnumerable<Recorded<Notification<char>>> notifications)
        //{
        //    Ensure.NotNull(notifications, nameof(notifications));
        //    var input = notifications as IList<Recorded<Notification<char>>> ?? notifications.ToList();
        //    Ensure.NotContainsMarkers(input, nameof(notifications));
        //    var builder = new StringBuilder();
        //    var entries = input
        //        .GroupBy(n => n.Time)
        //        .SelectMany(group => group
        //            .Materialize()
        //            .Select(n =>
        //            {
        //                var isGroup = !(n.IsFirst && n.IsLast);
        //                return new
        //                {
        //                    IsGroupStart = isGroup && n.IsFirst,
        //                    IsGroupEnd = isGroup && n.IsLast,
        //                    Notification = n.Value,
        //                };
        //            }))
        //        .Materialize();
        //    long lastNotificationTime = 0;
        //    foreach (var entry in entries)
        //    {
        //        var notification = entry.Value.Notification;
        //        if (notification.Time % MarbleScheduler.FrameTimeFactor != 0)
        //        {
        //            var message =
        //                $"Notifications cannot have times not being a multiple of {MarbleScheduler.FrameTimeFactor} (in that case, {notification.Time})";
        //            throw new ArgumentException(message, nameof(notifications));
        //        }
        //        var dashes = GetNumberOfDashes(notification, lastNotificationTime, entry.IsFirst);
        //        builder.Append(Marker.Nop1, dashes);
        //        if (entry.Value.IsGroupStart)
        //        {
        //            builder.Append(Marker.GroupStart);
        //        }
        //        switch (notification.Value.Kind)
        //        {
        //            case NotificationKind.OnCompleted:
        //                builder.Append(Marker.Completed);
        //                break;
        //            case NotificationKind.OnError:
        //                builder.Append(Marker.Error);
        //                break;
        //            case NotificationKind.OnNext:
        //                builder.Append(notification.Value.Value);
        //                break;
        //            default:
        //                throw new ArgumentOutOfRangeException();
        //        }
        //        if (entry.Value.IsGroupEnd)
        //        {
        //            builder.Append(Marker.GroupEnd);
        //        }
        //        lastNotificationTime = notification.Time;
        //    }
        //    return builder.ToString();
        //}

        private static int GetNumberOfDashes(long currentMarker, long prevMarker)
        {
            var d = (currentMarker - prevMarker)/MarbleScheduler.FrameTimeFactor;
            var dashes = (int) Math.Max(d, 0);
            return dashes;
        }
    }
}