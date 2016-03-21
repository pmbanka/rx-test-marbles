using System;
using Microsoft.Reactive.Testing;

namespace TestMarbles
{
    public static partial class Marbles
    {
        public static Subscription ToSubscription(string marbles)
        {
            long groupStart = -1;
            long subscriptionFrame = Subscription.Infinite;
            long unsubscriptionFrame = Subscription.Infinite;
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
                        if (subscriptionFrame != Subscription.Infinite)
                        {
                            throw new ArgumentException(
                                $"Found a second subscription point '^' in a {marbles} subscription marble diagram. There can be only one.",
                                nameof(marbles));
                        }
                        subscriptionFrame = groupStart > -1 ? groupStart : frame;
                        break;
                    case '!':
                        if (unsubscriptionFrame != Subscription.Infinite)
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
            // TODO check js
            return new Subscription(subscriptionFrame, unsubscriptionFrame);
        }
    }
}