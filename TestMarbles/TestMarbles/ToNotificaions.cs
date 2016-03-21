using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;

namespace TestMarbles
{
    public static partial class Marbles
    {
        public static IEnumerable<Recorded<Notification<char>>> ToNotifications(string marbles, Exception error = null)
        {
            return ToNotifications<char>(marbles, null, error);
        }

        public static IEnumerable<Recorded<Notification<T>>> ToNotifications<T>(
            string marbles,
            IReadOnlyDictionary<char, T> values,
            Exception error = null)
        {
            if (marbles.Contains(Marker.Unsubscription))
            {
                throw new ArgumentException($"Conventional marble diagrams cannot have unsubscription marker '{Marker.Unsubscription}'",
                    nameof(marbles));
            }
            // TODO handle cold observables in T
            long subscribeIndex = marbles.IndexOf(Marker.Subscription);
            long frameOffset = subscribeIndex == -1 ? 0 : subscribeIndex * -Constants.FrameTimeFactor;
            long groupStart = -1;
            for (int i = 0; i < marbles.Length; i++)
            {
                long frame = i * Constants.FrameTimeFactor + frameOffset;
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
                        notification = Notification.CreateOnError<T>(error ?? new Exception("error"));
                        break;
                    default:
                        T value = values != null ? values[c] : (T)(object)c;
                        notification = Notification.CreateOnNext(value);
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
