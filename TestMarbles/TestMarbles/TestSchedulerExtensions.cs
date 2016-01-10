using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;

namespace TestMarbles
{
    public static class TestSchedulerExtensions
    {
        public const long FrameTimeFactor = 10;

        private static IEnumerable<Recorded<Notification<T>>> parseMarbles<T>(
            string marbles,
            IReadOnlyDictionary<char, T> values,
            Exception error)
        {
            if (marbles.IndexOf('!') != -1)
            {
                throw new ArgumentException("Conventional marble diagrams cannot have unsubscription marker '!'", nameof(marbles));
            }
            long subscribeIndex = marbles.IndexOf('^');
            long frameOffset = subscribeIndex == -1 ? 0 : subscribeIndex * -FrameTimeFactor;
            long groupStart = -1;
            for (int i = 0; i < marbles.Length; i++)
            {
                long frame = i * FrameTimeFactor + frameOffset;
                Notification<T> notification = null;
                var c = marbles[i];
                switch (c)
                {
                    case '-':
                    case ' ':
                        break; // TODO error
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
                        dynamic value = values != null ? values[c] : (dynamic)c;
                        notification = Notification.CreateOnNext(value);
                        break;
                }
                if (notification != null)
                {
                    yield return new Recorded<Notification<T>>(groupStart > -1 ? groupStart : frame, notification);
                }
            }
        }

        public static ITestableObservable<T> CreateColdObservable<T>(
            this TestScheduler scheduler,
            string marbles,
            IReadOnlyDictionary<char, T> values = null,
            Exception error = null)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }
            if (string.IsNullOrWhiteSpace(marbles))
            {
                throw new ArgumentException("Cannot be either null, empty, nor whitespace.", nameof(marbles));
            }
            if (marbles.IndexOf('^') != -1)
            {
                throw new ArgumentException("Cold observable cannot have subscription offset '^'", nameof(marbles));
            }
            if (marbles.IndexOf('!') != -1)
            {
                throw new ArgumentException("Cold observable cannot have unsubscription marker '!'", nameof(marbles));
            }
            if (values == null && typeof(T) != typeof(char))
            {
                throw new ArgumentNullException(nameof(values), "If observable type is not char, values dictionary has to be provided");
            }
            var input = marbles.Trim();
            var events = parseMarbles(input, values, error).ToArray();
            return scheduler.CreateColdObservable(events);
        }

        public static ITestableObservable<T> CreateHotObservable<T>(
            this TestScheduler scheduler,
            string sequence,
            IReadOnlyDictionary<char, T> values = null,
            Exception error = null)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException(nameof(scheduler));
            }
            if (string.IsNullOrWhiteSpace(sequence))
            {
                throw new ArgumentException("Cannot be either null, empty, nor whitespace only.", nameof(sequence)); 
            }
            if (values == null && typeof(T) != typeof(char))
            {
                throw new ArgumentNullException(nameof(values), "If observable type is not char, values dictionary has to be provided");
            }
            var input = sequence.Trim();
            var events = parseMarbles(input, values, error).ToArray();
            return scheduler.CreateHotObservable(events);
        }
    }
}
