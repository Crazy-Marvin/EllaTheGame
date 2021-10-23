
#if !UNITY_2017_2_OR_NEWER

using System;
using System.IO;
using UnityEngine.Networking;

namespace Yodo1.MAS
{
    public class Yodo1AdDownloadHandler : DownloadHandlerScript
    {
        // Required by DownloadHandler base class. Called when you address the 'bytes' property.
        protected override byte[] GetData()
        {
            return null;
        }

        private FileStream fileStream;

        public Yodo1AdDownloadHandler(string path) : base(new byte[2048])
        {
            var downloadDirectory = Path.GetDirectoryName(path);
            if (!Directory.Exists(downloadDirectory))
            {
                Directory.CreateDirectory(downloadDirectory);
            }

            try
            {
                //Open the current file to write to
                fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            catch (Exception exception)
            {
                Debug.LogError(string.Format("Failed to create file at {0}\n{1}", path, exception.Message));
            }
        }

        protected override bool ReceiveData(byte[] byteFromServer, int dataLength)
        {
            if (byteFromServer == null || byteFromServer.Length < 1 || fileStream == null)
            {
                return false;
            }

            try
            {
                //Write the current data to the file
                fileStream.Write(byteFromServer, 0, dataLength);
            }
            catch (Exception exception)
            {
                fileStream.Close();
                fileStream = null;
                Debug.LogError(string.Format("Failed to download file{0}", exception.Message));
            }

            return true;
        }

        protected override void CompleteContent()
        {
            fileStream.Close();
        }
    }
}

#endif
