using MonKey.Editor.Internal;
using MonKey.Internal;

namespace MonKey.Settings.Internal
{
    internal static class MonKeySessionStatus
    {
        public static bool IsMonKeyReady()
        {
            return CommandManager.Instance
                   && MonkeyStyle.Instance
                   && MonKeyInternalSettings.Instance
                   && MonKeyLocManager.CurrentLoc;
        }

    }
}