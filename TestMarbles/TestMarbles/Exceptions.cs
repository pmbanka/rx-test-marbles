using System;
using System.Linq;
using System.Runtime.Serialization;

namespace TestMarbles
{
    [Serializable]
    public class ExpectObservableToBeFailedException : Exception
    {
        public ExpectObservableToBeFailedException()
        {
        }

        public ExpectObservableToBeFailedException(string message) : base(message)
        {
        }

        public ExpectObservableToBeFailedException(string message, Exception inner) : base(message, inner)
        {
        }

        public ExpectObservableToBeFailedException(
            string message, 
            string expectedMarbles, 
            string actualMarbles,
            int? markerPosition = null) : this(CreateMessage(message, expectedMarbles, actualMarbles, markerPosition))
        {
        }

        protected ExpectObservableToBeFailedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        private static string CreateMessage(string message, string expectedMarbles, string actualMarbles, int? markerPosition)
        {
            var msg = $"ExpectObservable.ToBe failed. {message}.\nExpected: \"{expectedMarbles}\"\nActual:   \"{actualMarbles}\"";
            if (markerPosition.HasValue)
            {
                var spaces = new string(Enumerable.Repeat(' ', 11 + markerPosition.Value).ToArray());
                msg = $"{msg}\n{spaces}^";
            }
            return msg;
        }
    }

    [Serializable]
    public class ExpectSubscriptionToBeFailedException : Exception
    {
        public ExpectSubscriptionToBeFailedException()
        {
        }

        public ExpectSubscriptionToBeFailedException(string message) : base(CreateMessage(message))
        {
        }

        public ExpectSubscriptionToBeFailedException(string message, Exception inner) : base(CreateMessage(message), inner)
        {
        }

        protected ExpectSubscriptionToBeFailedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        private static string CreateMessage(string message)
        {
            return $"ExpectSubscription.ToBe failed. {message}.";
        }
    }
}