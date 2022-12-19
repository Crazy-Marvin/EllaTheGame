using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyMobile.Internal;

namespace EasyMobile.Demo
{
    public class StringStringCollectionView : MonoBehaviour
    {
        [SerializeField]
        private string title = "title";

        [SerializeField]
        private GameObject root = null;

        [SerializeField]
        private AddedStringStringView elementTemplate = null;

        [SerializeField]
        private Transform addedElementsScrollView = null;

        [SerializeField]
        private Text titleText = null;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                titleText.text = title;
            }
        }

        private List<StringStringKeyValuePair> Collection = null;
        private List<AddedStringStringView> addedElements = new List<AddedStringStringView>();

        public void Show(List<StringStringKeyValuePair> collection, string title)
        {
            root.SetActive(true);
            Title = title;
            Collection = collection;

            foreach (var pair in Collection)
            {
                var newElement = Instantiate(elementTemplate, addedElementsScrollView);
                newElement.Setup(pair.Key, pair.Value);

                newElement.OnDestroy += () =>
                {
                    Collection.Remove(pair);
                    addedElements.Remove(newElement);
                };

                addedElements.Add(newElement);
            }
        }

        public void Hide()
        {
            ClearAddedElement();
            root.SetActive(false);
        }

        private void ClearAddedElement()
        {
            foreach(var element in addedElements)
                element.Destroy(false);
            addedElements.Clear();
        }
    }
}
