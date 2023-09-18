using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace EasyMobile.Internal.Privacy
{
    // Image according to the label inside the name attribute to load, read from the Resources directory.
    // The size of the image is controlled by the size property.
    // Use: <quad name=NAME size=25 width=1 />
    [ExecuteInEditMode] // Needed for culling images that are not used.
    public class EditorConsentDialogClickableText : Text, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler, ISelectHandler
    {
        #region Define structures

        [Serializable]
        public struct IconName
        {
            public string name;
            public Sprite sprite;
        }

        /// <summary>
        /// Hyperlinks Info
        /// </summary>
        private class HrefInfo
        {
            public int startIndex;
            public int endIndex;
            public string name;

            public readonly List<Rect> boxes = new List<Rect>();
        }

        #endregion

        public readonly string HyperlinkColor = "#0000EE";

        public event Action<string> OnHyperlinkClicked;

        private readonly List<Image> imagesPool = new List<Image>();
        private readonly List<GameObject> culledImagesPool = new List<GameObject>();
        private readonly List<int> imagesVertexIndex = new List<int>();
        private static readonly StringBuilder textBuilder = new StringBuilder();
        private static readonly Regex hrefRegex = new Regex(@"<a href=([^>\n\s]+)>(.*?)(</a>)", RegexOptions.Singleline);
        private static readonly Regex regex = new Regex(@"<quad name=(.+?) size=(\d*\.?\d+%?) width=(\d*\.?\d+%?) />", RegexOptions.Singleline);
        private readonly List<HrefInfo> hrefInfos = new List<HrefInfo>();

        private string fixedString = "";
        private bool clearImages = false;
        private string outputText = "";
        private IconName[] inspectorIconList = null;
        private Dictionary<string, Sprite> iconList = new Dictionary<string, Sprite>();
        private float imageScalingFactor = 1;
        private Vector2 imageOffset = Vector2.zero;
        private Button button = null;
        private List<Vector2> positions = new List<Vector2>();
        private string previousText = "";
        private bool isCreatingHrefInfos = true;

        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            UpdateQuadImage();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, eventData.position, eventData.pressEventCamera, out localPoint);

            foreach (var hrefInfo in hrefInfos)
            {
                var boxes = hrefInfo.boxes;

                for (var i = 0; i < boxes.Count; ++i)
                {
                    if (boxes[i].Contains(localPoint))
                    {
                        if (OnHyperlinkClicked != null)
                            OnHyperlinkClicked.Invoke(hrefInfo.name);

                        return;
                    }
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //selected = true;
            if (imagesPool.Count >= 1)
            {
                foreach (Image img in imagesPool)
                {
                    if (button != null && button.isActiveAndEnabled)
                    {
                        img.color = button.colors.highlightedColor;
                    }
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //selected = false;
            if (imagesPool.Count >= 1)
            {
                foreach (Image img in imagesPool)
                {
                    if (button != null && button.isActiveAndEnabled)
                    {
                        img.color = button.colors.normalColor;
                    }
                    else
                    {
                        img.color = color;
                    }
                }
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            //selected = true;
            if (imagesPool.Count >= 1)
            {
                foreach (Image img in imagesPool)
                {
                    if (button != null && button.isActiveAndEnabled)
                    {
                        img.color = button.colors.highlightedColor;
                    }
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            button = GetComponent<Button>();
            if (inspectorIconList != null && inspectorIconList.Length > 0)
            {
                foreach (IconName icon in inspectorIconList)
                {
                    iconList.Add(icon.name, icon.sprite);
                }
            }
            ResetHrefInfos();
        }

        protected virtual void Update()
        {
            if (clearImages)
            {
                for (int i = 0; i < culledImagesPool.Count; i++)
                {
                    DestroyImmediate(culledImagesPool[i]);
                }
                culledImagesPool.Clear();
                clearImages = false;
            }

            if (previousText != text)
                ResetHrefInfos();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            UpdateQuadImage();
        }
#endif

        protected void UpdateQuadImage()
        {
#if UNITY_EDITOR
#if UNITY_2018_3_OR_NEWER
            bool isPrefab = UnityEditor.PrefabUtility.GetPrefabAssetType(this) == UnityEditor.PrefabAssetType.Regular;
#else
            bool isPrefab = UnityEditor.PrefabUtility.GetPrefabType(this) == UnityEditor.PrefabType.Prefab;
#endif
            if (isPrefab)
            {
                return;
            }
#endif
            outputText = GetOutputText();
            imagesVertexIndex.Clear();
            foreach (Match match in regex.Matches(outputText))
            {
                var picIndex = match.Index;
                var endIndex = picIndex * 4 + 3;
                imagesVertexIndex.Add(endIndex);

                imagesPool.RemoveAll(image => image == null);
                if (imagesPool.Count == 0)
                {
                    GetComponentsInChildren<Image>(imagesPool);
                }
                if (imagesVertexIndex.Count > imagesPool.Count)
                {
                    var resources = new DefaultControls.Resources();
                    var go = DefaultControls.CreateImage(resources);
                    go.layer = gameObject.layer;
                    var rt = go.transform as RectTransform;
                    if (rt)
                    {
                        rt.SetParent(rectTransform);
                        rt.localPosition = Vector3.zero;
                        rt.localRotation = Quaternion.identity;
                        rt.localScale = Vector3.one;
                    }
                    imagesPool.Add(go.GetComponent<Image>());
                }

                var spriteName = match.Groups[1].Value;
                //var size = float.Parse(match.Groups[2].Value);
                var img = imagesPool[imagesVertexIndex.Count - 1];
                if (img.sprite == null || img.sprite.name != spriteName)
                {
                    // img.sprite = Resources.Load<Sprite>(spriteName);
                    if (inspectorIconList != null && inspectorIconList.Length > 0)
                    {
                        foreach (IconName icon in inspectorIconList)
                        {
                            if (icon.name == spriteName)
                            {
                                img.sprite = icon.sprite;
                                break;
                            }
                        }
                    }
                }
                img.rectTransform.sizeDelta = new Vector2(fontSize * imageScalingFactor, fontSize * imageScalingFactor);
                img.enabled = true;
                if (positions.Count == imagesPool.Count)
                {
                    img.rectTransform.anchoredPosition = positions[imagesVertexIndex.Count - 1];
                }
            }

            for (var i = imagesVertexIndex.Count; i < imagesPool.Count; i++)
            {
                if (imagesPool[i])
                {
                    /* TEMPORARY FIX REMOVE IMAGES FROM POOL DELETE LATER SINCE CANNOT DESTROY */
                    imagesPool[i].gameObject.SetActive(false);
                    imagesPool[i].gameObject.hideFlags = HideFlags.HideAndDontSave;
                    culledImagesPool.Add(imagesPool[i].gameObject);
                    imagesPool.Remove(imagesPool[i]);
                }
            }
            if (culledImagesPool.Count > 1)
            {
                clearImages = true;
            }
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            var orignText = m_Text;
            m_Text = GetOutputText();
            base.OnPopulateMesh(toFill);
            m_Text = orignText;
            positions.Clear();

            UIVertex vert = new UIVertex();
            for (var i = 0; i < imagesVertexIndex.Count; i++)
            {
                var endIndex = imagesVertexIndex[i];
                var rt = imagesPool[i].rectTransform;
                var size = rt.sizeDelta;
                if (endIndex < toFill.currentVertCount)
                {
                    toFill.PopulateUIVertex(ref vert, endIndex);
                    positions.Add(new Vector2((vert.position.x + size.x / 2), (vert.position.y + size.y / 2)) + imageOffset);

                    // Erase the lower left corner of the black specks
                    toFill.PopulateUIVertex(ref vert, endIndex - 3);
                    var pos = vert.position;
                    for (int j = endIndex, m = endIndex - 3; j > m; j--)
                    {
                        toFill.PopulateUIVertex(ref vert, endIndex);
                        vert.position = pos;
                        toFill.SetUIVertex(vert, j);
                    }
                }
            }

            if (imagesVertexIndex.Count != 0)
            {
                imagesVertexIndex.Clear();
            }

            // Hyperlinks surround processing box
            foreach (var hrefInfo in hrefInfos)
            {
                hrefInfo.boxes.Clear();
                if (hrefInfo.startIndex >= toFill.currentVertCount)
                {
                    continue;
                }

                // Hyperlink inside the text is added to surround the vertex index coordinate frame
                toFill.PopulateUIVertex(ref vert, hrefInfo.startIndex);
                var pos = vert.position;
                var bounds = new Bounds(pos, Vector3.zero);
                for (int i = hrefInfo.startIndex, m = hrefInfo.endIndex; i < m; i++)
                {
                    if (i >= toFill.currentVertCount)
                    {
                        break;
                    }

                    toFill.PopulateUIVertex(ref vert, i);
                    pos = vert.position;
                    if (pos.x < bounds.min.x) // Wrap re-add surround frame
                    {
                        hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
                        bounds = new Bounds(pos, Vector3.zero);
                    }
                    else
                    {
                        bounds.Encapsulate(pos); // Extended enclosed box
                    }
                }
                hrefInfo.boxes.Add(new Rect(bounds.min, bounds.size));
            }
            UpdateQuadImage();
        }

        protected string GetOutputText()
        {
            textBuilder.Length = 0;

            var indexText = 0;
            fixedString = this.text;
            if (inspectorIconList != null && inspectorIconList.Length > 0)
            {
                foreach (IconName icon in inspectorIconList)
                {
                    if (icon.name != null && icon.name != "")
                    {
                        fixedString = fixedString.Replace(icon.name, "<quad name=" + icon.name + " size=" + fontSize + " width=1 />");
                    }
                }
            }
            int count = 0;
            foreach (Match match in hrefRegex.Matches(fixedString))
            {
                textBuilder.Append(fixedString.Substring(indexText, match.Index - indexText));
                textBuilder.Append("<color=" + HyperlinkColor + ">");  // Hyperlink color

                var group = match.Groups[1];
                if (isCreatingHrefInfos)
                {
                    var hrefInfo = new HrefInfo
                    {
                        startIndex = textBuilder.Length * 4, // Hyperlinks in text starting vertex indices
                        endIndex = (textBuilder.Length + match.Groups[2].Length - 1) * 4 + 3,
                        name = group.Value
                    };
                    hrefInfos.Add(hrefInfo);
                }
                else
                {
                    if (hrefInfos.Count > 0)
                    {
                        hrefInfos[count].startIndex = textBuilder.Length * 4; // Hyperlinks in text starting vertex indices;
                        hrefInfos[count].endIndex = (textBuilder.Length + match.Groups[2].Length - 1) * 4 + 3;
                        count++;
                    }
                }

                textBuilder.Append(match.Groups[2].Value);
                textBuilder.Append("</color>");
                indexText = match.Index + match.Length;
            }
            // we should create array only once or if there is any change in the text
            if (isCreatingHrefInfos)
                isCreatingHrefInfos = false;

            textBuilder.Append(fixedString.Substring(indexText, fixedString.Length - indexText));

            return textBuilder.ToString();
        }

        private void ResetHrefInfos()
        {
            previousText = text;
            hrefInfos.Clear();
            isCreatingHrefInfos = true;
        }
    }
}