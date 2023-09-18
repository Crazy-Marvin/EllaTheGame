#if UNITY_IOS
using System.Runtime.InteropServices;


namespace EasyMobile.Internal.Sharing
{
    internal class iOSSharingClient : ISharingClient
    {
        public void ShareImage(string imagePath, string message, string subject = "")
        {
            var data = new C.ShareData
            {
                text = message,
                url = "",
                image = imagePath,
                subject = subject
            };

            C.EM_Sharing_Share(ref data);
        }

        public void ShareText(string text, string subject = "")
        {
            var data = new C.ShareData
            {
                text = text,
                url = "",
                image = "",
                subject = subject
            };

            C.EM_Sharing_Share(ref data);
        }

        public void ShareURL(string url, string subject = "")
        {
            var data = new C.ShareData
            {
                text = "",
                url = url,
                image = "",
                subject = subject
            };

            C.EM_Sharing_Share(ref data);
        }

        #region C wrapper

        private static class C
        {
            internal struct ShareData
            {
                public string text;
                public string url;
                public string image;
                public string subject;
            }

            [DllImport("__Internal")]
            internal static extern void EM_Sharing_Share(ref ShareData data);
        }

        #endregion
    }
}
#endif