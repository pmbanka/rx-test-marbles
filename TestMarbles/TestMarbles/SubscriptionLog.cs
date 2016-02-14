namespace TestMarbles
{
    public class SubscriptionLog
    {
        public SubscriptionLog(long subscribedFrame, long unsubscribedFrame = long.MaxValue)
        {
            SubscribedFrame = subscribedFrame;
            UnsubscribedFrame = unsubscribedFrame;
        }

        public long SubscribedFrame { get; }

        public long UnsubscribedFrame { get; }
    }
}