using System;
using Microsoft.Reactive.Testing;
using TestMarbles.Helpers;
using TestMarbles.Utils;

namespace TestMarbles
{
    public static partial class Marbles
    {
        public static Subscription ToSubscription(string marbles)
        {
            Ensure.NotNull(marbles, nameof(marbles));
            long groupStart = -1;
            long subscriptionFrame = Subscription.Infinite;
            long unsubscriptionFrame = Subscription.Infinite;
            for (int i = 0; i < marbles.Length; i++)
            {
                var frame = i*MarbleScheduler.FrameTimeFactor;
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
                    case Marker.Subscription:
                        if (subscriptionFrame != Subscription.Infinite)
                        {
                            throw new ArgumentException(
                                $"Found a second subscription point '{Marker.Subscription}' in a {marbles} subscription marble diagram. There can be only one.",
                                nameof(marbles));
                        }
                        subscriptionFrame = groupStart > -1 ? groupStart : frame;
                        break;
                    case Marker.Unsubscription:
                        if (unsubscriptionFrame != Subscription.Infinite)
                        {
                            // TODO FIX IN JS
                            throw new ArgumentException(
                                $"Found a second unsubscription point '{Marker.Unsubscription}' in a {marbles} subscription marble diagram. There can be only one.",
                                nameof(marbles));
                        }
                        unsubscriptionFrame = groupStart > -1 ? groupStart : frame;
                        break;
                    default:
                        throw new ArgumentException(
                            $"There can only be '{Marker.Subscription}' and '{Marker.Unsubscription}' markers in a {marbles} subscription marble diagram. Found instead {c}.");
                }
            }
            // TODO check js
            return new Subscription(subscriptionFrame, unsubscriptionFrame);
        }
    }
}