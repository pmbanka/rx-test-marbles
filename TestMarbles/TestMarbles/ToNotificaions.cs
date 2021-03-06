﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Microsoft.Reactive.Testing;
using TestMarbles.Helpers;
using TestMarbles.Utils;

namespace TestMarbles
{
    public static partial class Marbles
    {
        public static IEnumerable<Recorded<Notification<char>>> ToNotifications(string marbles, Exception error = null)
        {
            Ensure.NotNull(marbles, nameof(marbles));
            return ToNotifications(marbles, new EchoDictionary<char>(), error);
        }

        public static IEnumerable<Recorded<Notification<T>>> ToNotifications<T>(
            string marbles,
            IReadOnlyDictionary<char, T> values,
            Exception error = null)
        {
            Ensure.NotNull(marbles, nameof(marbles));
            Ensure.NotNull(values, nameof(values));
            Ensure.NotContainsMarkers(values, nameof(values));
            if (marbles.Contains(Marker.Unsubscription))
            {
                throw new ArgumentException($"Conventional marble diagrams cannot have unsubscription marker '{Marker.Unsubscription}'",
                    nameof(marbles));
            }
            
            long subscribeIndex = marbles.IndexOf(Marker.Subscription);
            long frameOffset = subscribeIndex == -1 ? 0 : subscribeIndex * -MarbleScheduler.FrameTimeFactor;
            long groupStart = -1;
            for (int i = 0; i < marbles.Length; i++)
            {
                long frame = i * MarbleScheduler.FrameTimeFactor + frameOffset;
                Notification<T> notification = null;
                var c = marbles[i];
                switch (c)
                {
                    case Marker.Nop1:
                    case Marker.Nop2:
                        break;
                    case Marker.GroupStart:
                        groupStart = frame;
                        break;
                    case Marker.GroupEnd:
                        groupStart = -1;
                        break;
                    case Marker.Completed:
                        notification = Notification.CreateOnCompleted<T>();
                        break;
                    case Marker.Subscription:
                        break;
                    case Marker.Error:
                        notification = Notification.CreateOnError<T>(error ?? new Exception());
                        break;
                    default:
                        T newValue;
                        if (!values.TryGetValue(c, out newValue))
                        {
                            throw new KeyNotFoundException($"Convertion to {typeof(T)} not found for OnNext marker \"{c}\"");
                        }
                        notification = Notification.CreateOnNext(newValue);
                        break;
                }
                if (notification != null)
                {
                    yield return new Recorded<Notification<T>>(groupStart > -1 ? groupStart : frame, notification);
                }
            }
        }
    }
}
