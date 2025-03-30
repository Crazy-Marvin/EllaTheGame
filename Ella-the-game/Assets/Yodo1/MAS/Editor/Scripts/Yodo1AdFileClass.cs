namespace Yodo1.MAS
{
    using System.IO;
    using UnityEngine;

    public class Yodo1AdFileClass
    {
        private string filePath;

        public Yodo1AdFileClass(string fPath)
        {
            filePath = fPath;
            if (!File.Exists(filePath))
            {
                Debug.LogError(filePath + "The file does not exist in the path.");
            }
        }

        public bool IsHaveText(string text)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();
            if (text_all.Contains(text))
            {
                return true;
            }

            return false;
        }

        public void WriteBelow(string below, string text)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();
            if (text_all.Contains(text))
            {
                return;
            }

            int beginIndex = text_all.IndexOf(below);
            if (beginIndex == -1)
            {
                return;
            }

            int endIndex = text_all.LastIndexOf("\n", beginIndex + below.Length);

            text_all = text_all.Substring(0, endIndex) + "\n" + text + text_all.Substring(endIndex);

            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(text_all);
            streamWriter.Close();
        }
    }
}
