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

        protected ExpectObservableToBeFailedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
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