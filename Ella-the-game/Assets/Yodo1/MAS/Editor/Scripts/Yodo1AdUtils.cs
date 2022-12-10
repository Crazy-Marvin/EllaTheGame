namespace Yodo1.MAS
{
    using UnityEditor;
    using System.IO;
    using System.Xml;

    public class Yodo1AdUtils
    {
        /// <summary>
        /// Show Alert
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="positiveButton"></param>
        public static void ShowAlert(string title, string message, string positiveButton)
        {
            if (!string.IsNullOrEmpty(positiveButton))
            {
                int index = EditorUtility.DisplayDialogComplex(title, message, positiveButton, "", "");

            }
            return;
        }

        private static readonly string VERSION_PATH = Path.GetFullPath(".") + "/Assets/Yodo1/MAS/version.xml";

        public static string GetPluginVersion()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(VERSION_PATH, settings);

            XmlDocument xmlReadDoc = new XmlDocument();
            xmlReadDoc.Load(VERSION_PATH);
            XmlNode xnRead = xmlReadDoc.SelectSingleNode("versions");
            XmlElement unityNode = (XmlElement)xnRead.SelectSingleNode("unity");
            string version = unityNode.GetAttribute("version").ToString();
            string suffix = unityNode.GetAttribute("suffix").ToString();
            if (!string.IsNullOrEmpty(suffix))
            {
                version = version + "-" + suffix;
            }
            reader.Close();
            return version;
        }
    }
}