namespace TestMarbles
{
    public static class Marker
    {
        public const char Subscription = '^';

        public const char Unsubscription = '!';

        public const char Nop1 = '-';

        public const char Nop2 = ' ';

        public const char GroupStart = '(';

        public const char GroupEnd = ')';

        public const char Error = '#';

        public const char Completed = '|';

        public static readonly string All = new string(new[]
        {
            Subscription,
            Unsubscription,
            Nop1,
            Nop2,
            GroupStart,
            GroupEnd,
            Error,
            Completed
        });
    }
}