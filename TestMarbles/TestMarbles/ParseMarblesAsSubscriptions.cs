using System;
using Microsoft.Reactive.Testing;

namespace TestMarbles
{
    public static partial class TestSchedulerEx
    {
        public static Subscription ParseMarblesAsSubscriptions(string marbles)
        {
            long groupStart = -1;
            long subscriptionFrame = long.MaxValue;
            long unsubscriptionFrame = long.MaxValue;
            for (int i = 0; i < marbles.Length; i++)
            {
                var frame = i*Constants.FrameTimeFactor;
                var c = marbles[i];
                switch (c)
                {
                    case '-':
                    case ' ':
                        break;
                    case '(':
                        groupStart = frame;
                        break;
                    case ')':
                        groupStart = -1;
                        break;
                    case '^':
                        if (subscriptionFrame != long.MaxValue)
                        {
                            throw new ArgumentException(
                                $"Found a second subscription point '^' in a {marbles} subscription marble diagram. There can be only one.",
                                nameof(marbles));
                        }
                        subscriptionFrame = groupStart > -1 ? groupStart : frame;
                        break;
                    case '!':
                        if (unsubscriptionFrame != long.MaxValue)
                        {
                            // TODO FIX IN JS
                            throw new ArgumentException(
                                $"Found a second unsubscription point '!' in a {marbles} subscription marble diagram. There can be only one.",
                                nameof(marbles));
                        }
                        unsubscriptionFrame = groupStart > -1 ? groupStart : frame;
                        break;
                    default:
                        throw new ArgumentException(
                            $"There can only be '^' and '!' markers in a {marbles} subscription marble diagram. Found instead {c}.");
                }
            }
            return unsubscriptionFrame < 0
                ? new Subscription(subscriptionFrame)
                : new Subscription(subscriptionFrame, unsubscriptionFrame);
        }
    }
}