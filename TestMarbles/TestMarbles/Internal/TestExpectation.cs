namespace TestMarbles.Internal
{
    internal abstract class TestExpectation
    {
        protected TestExpectation()
        {
            Ready = false;
        }

        public bool Ready { get; protected set; }

        public abstract void Assert();
    }
}