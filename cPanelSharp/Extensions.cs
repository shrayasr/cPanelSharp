namespace cPanelSharp
{
    public static class Extensions
    {
        public static bool IsEmpty(this string instance)
        {
            return instance == null
                ? true
                : instance.Length == 0;
        }

        public static bool IsNotEmpty(this string instance) 
            => !IsEmpty(instance);
    }
}
