using System;

namespace MonKey.Extensions
{

    public static class EnumExt
    {
        public static bool HasFlag(this Enum e, Enum flag)
        {
            int ve = Convert.ToInt32(e);
            int fe = Convert.ToInt32(flag);
            return (ve & fe) == fe;
        }

        public static bool HasAnyFlag(this Enum e, Enum flag)
        {
            int ve = Convert.ToInt32(e);
            int fe = Convert.ToInt32(flag);
            return (ve & fe) != 0;
        }
    }
}