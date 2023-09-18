using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace EasyMobile.ManifestGenerator.Elements
{
    /// <summary>
    /// Base class for all the AndroidManifest elements.
    /// Note that we can't use interface or abstract class here so it can be serialized.
    /// </summary>
    [Serializable]
    public class AndroidManifestElement
    {
        #region Fields and Properties

        [SerializeField]
        private List<string> addedAttributesKey = new List<string>();

        [SerializeField]
        private List<string> addedAttributesValue = new List<string>();

        [SerializeField]
        private List<string> addedAttributesPrefix = new List<string>();

        /// <summary>
        /// Used to store child elements' id,
        /// workaround for Unity serialization limit.
        /// </summary>
        [SerializeField]
        private List<int> childElementsId = new List<int>();

        [SerializeField]
        private int id;

        [SerializeField]
        private AndroidManifestElementStyles style = AndroidManifestElementStyles.None;

        public virtual AndroidManifestElementStyles Style
        {
            get { return style; }
            protected set { style = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public virtual IEnumerable<AndroidManifestElementStyles> ParentStyles
        {
            get { yield break; }
        }

        public virtual IEnumerable<AndroidManifestElementStyles> ChildStyles
        {
            get { yield break; }
        }

        public virtual IEnumerable<string> AllAvailableAttributes
        {
            get { yield break; }
        }

        public virtual IEnumerable<string> RemainedAttributes
        {
            get
            {
                foreach(var attribute in AllAvailableAttributes)
                {
                    if (!AddedAttributesKey.Contains(attribute))
                    {
                        yield return attribute;
                    }
                }
            }
        }

        public List<string> AddedAttributesKey
        {
            get { return addedAttributesKey; }
        }

        public List<string> AddedAttributesValue
        {
            get { return addedAttributesValue; }
        }

        public List<string> AddedAttributesPrefix
        {
            get { return addedAttributesPrefix; }
        }

        public int AttributesCount
        {
            get
            {
                if (AddedAttributesKey.Count != AddedAttributesValue.Count)
                    return 0;

                return AddedAttributesKey.Count;
            }
        }

        public List<int> ChildElementsId
        {
            get { return childElementsId; }
        }

        public bool CanAddChildElement
        {
            get { return ChildStyles.Any(); }
        }

        #endregion

        #region Public Methods

        public AndroidManifestElement()
        {
            Id = GetHashCode();
        }

        public bool AddAttribute(string key, string value)
        {
            if (!IsAvailableAttribute(key))
                return false;

            AddedAttributesKey.Add(key);
            AddedAttributesValue.Add(value);
            AddedAttributesPrefix.Add(key.IsAndroidAttribute() ? "http://schemas.android.com/apk/res/android" : null);

            return true;
        }

        public bool AddInnerElement(AndroidManifestElement element)
        {
            if (!IsAvailableInnerElement(element.Style))
                return false;

            ChildElementsId.Add(element.Id);
            return true;
        }

        public bool AddInnerElement(AndroidManifestElementStyles style)
        {
            return AddInnerElement(style.CreateElementClass());
        }

        public XmlElement ToXmlElement(XmlDocument xmlDocument, List<AndroidManifestElement> elementsFactory)
        {
            XmlElement xmlElement = xmlDocument.CreateElement(ToString());

            /// Setup all the attributes.
            for (int i = 0; i < AttributesCount; i++)
            {
                if (string.IsNullOrEmpty(AddedAttributesPrefix[i]))
                    xmlElement.SetAttribute(AddedAttributesKey[i], AddedAttributesValue[i]);
                else
                    xmlElement.SetAttribute(AddedAttributesKey[i], AddedAttributesPrefix[i], AddedAttributesValue[i]);
            }

            /// Setup all inner elements.
            var childElements = GetChildElements(elementsFactory);
            if (childElements != null && childElements.Count > 0)
                foreach(var element in childElements)
                    xmlElement.AppendChild(element.ToXmlElement(xmlDocument, elementsFactory));

            return xmlElement;
        }

        public virtual List<AndroidManifestElement> GetChildElements(List<AndroidManifestElement> elementsFactory)
        {
            if (childElementsId == null)
                return null;

            var result = new List<AndroidManifestElement>();
            foreach (int childElementId in childElementsId)
            {
                var element = elementsFactory.Find(e => e.Id == childElementId);

                if (element != null)
                    result.Add(element);
            }
            return result;
        }

        public bool IsChildStyle(AndroidManifestElementStyles style)
        {
            return ChildStyles.ToList().Contains(style);
        }

        public bool IsParentStyle(AndroidManifestElementStyles style)
        {
            return ParentStyles.ToList().Contains(style);
        }

        public override string ToString()
        {
            return Style.ToAndroidManifestFormat();
        }

        #endregion

        #region Others

        protected bool IsAvailableAttribute(string key)
        {
            if (AllAvailableAttributes == null)
                return false;

            foreach(var attribute in AllAvailableAttributes)
            {
                if (attribute.Equals(key))
                    return true;
            }

            return false;
        }

        protected bool IsAvailableInnerElement(AndroidManifestElementStyles style)
        {
            if (ChildStyles == null)
                return false;

            foreach (var validStyle in ChildStyles)
            {
                if (validStyle.Equals(style))
                    return true;
            }

            return false;
        }

        #endregion
    }
}
