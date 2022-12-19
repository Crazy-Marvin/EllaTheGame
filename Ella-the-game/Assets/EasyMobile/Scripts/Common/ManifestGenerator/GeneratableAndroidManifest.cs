#if UNITY_EDITOR
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.ManifestGenerator.Elements;
using UnityEditor;

namespace EasyMobile.ManifestGenerator
{
    [Serializable]
    public class GeneratableAndroidManifest
    {
        [SerializeField]
        private AndroidManifestElement manifestElement = new ManifestElement();

        public AndroidManifestElement ManifestElement { get { return manifestElement; } }

        public void Save(string path, List<AndroidManifestElement> elementsFactory)
        {
            XmlDocument RootXmlDocument = new XmlDocument();

            /// Create header.
            XmlNode headerNode = RootXmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            RootXmlDocument.AppendChild(headerNode);

            RootXmlDocument.AppendChild(manifestElement.ToXmlElement(RootXmlDocument, elementsFactory));

            RootXmlDocument.Save(path);
            AssetDatabase.Refresh();
        }
    }
}
#endif