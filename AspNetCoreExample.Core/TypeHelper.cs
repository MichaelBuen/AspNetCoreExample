namespace AspNetCoreExample.Core
{
    public static class TypeHelper
    {
        // https://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
        public static bool IsOpenGenericAssignableFrom(this System.Type openGeneric, System.Type fromCheck)
        {
            while (fromCheck != null && fromCheck != typeof(object))
            {
                var cur = fromCheck.IsGenericType ? fromCheck.GetGenericTypeDefinition() : fromCheck;
                if (openGeneric == cur)
                {
                    return true;
                }
                fromCheck = fromCheck.BaseType;
            }
            return false;
        }
    }
}
