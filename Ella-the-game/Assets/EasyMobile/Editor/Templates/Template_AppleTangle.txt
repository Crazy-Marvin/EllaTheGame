#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do NOT modify! Generated file.

namespace UnityEngine.Purchasing.Security
{
    public class AppleTangle
    {
    	// This is a custom field added to detect if this is a dummy class created by us.
    	// It will be validated via reflection only, hence the warning suppression.
    	#pragma warning disable 0414
        private static bool isDummyClass = true;
        #pragma warning restore 0414

        private static byte[] data = System.Convert.FromBase64String("IAmDummyAppleTangleData");
        private static int[] order = new int[] { 1, 2 };
        private static int key = 111;

        public static byte[] Data()
        {
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
