using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;

namespace TestMarbles
{
    internal static class Utils
    {
        public const long FrameTimeFactor = 10;

        public static IEnumerable<Recorded<Notification<T>>> ParseMarbles<T>(
            string marbles,
            IReadOnlyDictionary<char, T> values,
            Exception error)
        {
            if (marbles.IndexOf('!') != -1)
            {
                throw new ArgumentException("Conventional marble diagrams cannot have unsubscription marker '!'",
                    nameof(marbles));
            }
            long subscribeIndex = marbles.IndexOf('^');
            long frameOffset = subscribeIndex == -1 ? 0 : subscribeIndex*-FrameTimeFactor;
            long groupStart = -1;
            for (int i = 0; i < marbles.Length; i++)
            {
                long frame = i*FrameTimeFactor + frameOffset;
                Notification<T> notification = null;
                var c = marbles[i];
                switch (c)
                {
                    case '-':
                    case ' ':
                        break; // TODO error?
                    case '(':
                        groupStart = frame;
                        break;
                    case ')':
                        groupStart = -1;
                        break;
                    case '|':
                        notification = Notification.CreateOnCompleted<T>();
                        break;
                    case '^':
                        break;
                    case '#':
                        notification = Notification.CreateOnError<T>(error ?? new Exception("error"));
                        break;
                    default:
                        dynamic value = values != null ? values[c] : (dynamic) c;
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