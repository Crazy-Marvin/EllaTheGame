using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    public class ScrollableList : MonoBehaviour
    {
        public event System.Action<ScrollableList, string, string> ItemSelected = delegate {};

        public event System.Action<ScrollableList> UIClosed = delegate {};

        [Header("Visual Settings")]
        public Vector3 position = Vector3.zero;
        public bool horizontalScroll = true;
        public bool verticalScroll = true;
        public float width = 500;
        public float height = 700;
        public float itemHeight = 90;
        public float spacing = 10;
        public int paddingLeft = 10;
        public int paddingRight = 10;
        public int paddingTop = 20;
        public int paddingBottom = 20;
        public Color bodyColor = Color.white;
        public Color itemColor = Color.gray;

        [Header("Internal References")]
        public Text title;
        public ScrollRect scrollRect;
        public Transform content;
        public GameObject itemPrefab;
        public Dictionary<string, string> items;

        public static ScrollableList Create(GameObject listPrefab, string title, Dictionary<string, string> items)
        {
            GameObject listObj = Instantiate(listPrefab, listPrefab.transform.position, listPrefab.transform.rotation) as GameObject;
            ScrollableList scrollableList = listObj.GetComponent<ScrollableList>();
            scrollableList.title.text = title;
            scrollableList.items = items;
            scrollableList.Init();

            return scrollableList;
        }

        public void Init()
        {
            // Visual settings
            // Position
            transform.position = position;

            // Sizes
            scrollRect.GetComponent<RectTransform>().sizeDelta.Set(width, height);

            // Color
            scrollRect.GetComponent<Image>().color = bodyColor;

            var layoutGroup = content.GetComponent<HorizontalOrVerticalLayoutGroup>();
            layoutGroup.spacing = spacing;
            layoutGroup.padding.left = paddingLeft;
            layoutGroup.padding.right = paddingRight;
            layoutGroup.padding.top = paddingTop;
            layoutGroup.padding.bottom = paddingBottom;

            // Set content height
            float contentHeight = paddingTop + itemHeight * items.Count + spacing * (items.Count - 1) + paddingBottom;
            var contentRectTf = content.GetComponent<RectTransform>();
            contentRectTf.sizeDelta = new Vector2(contentRectTf.sizeDelta.x, contentHeight);
            Vector3 contentPos = contentRectTf.localPosition;
            contentPos.y = 0;
            contentRectTf.localPosition = contentPos;

            // Populate items
            foreach (KeyValuePair<string, string> item in items)
            {
                AddItem(item.Key, item.Value);
            }

            scrollRect.horizontal = false;
            scrollRect.vertical = false;

            // Enable scroll
            Invoke("EnableScroll", 0.1f);
        }

        public GameObject AddItem(string title, string subtitle)
        {
            GameObject newItem = Instantiate(itemPrefab, content.position, Quaternion.identity) as GameObject;
            newItem.GetComponent<LayoutElement>().preferredHeight = itemHeight;
            newItem.GetComponent<Image>().color = itemColor;
            ScrollableListItem itemComp = newItem.GetComponent<ScrollableListItem>();
            itemComp.title.text = title;
            itemComp.subtitle.text = subtitle;
            itemComp.button.onClick.AddListener(() =>
                {
                    // Fire event
                    ItemSelected(this, title, subtitle);

                    // Hide the whole list
                    Destroy(gameObject, 0.1f);
                });

            newItem.transform.SetParent(content, false);

            return newItem;
        }

        public void Close()
        {
            UIClosed(this);
            Destroy(gameObject);
        }

        void EnableScroll()
        {
            scrollRect.horizontal = horizontalScroll;
            scrollRect.vertical = verticalScroll;
            scrollRect.verticalNormalizedPosition = 1;
        }
    }
}

