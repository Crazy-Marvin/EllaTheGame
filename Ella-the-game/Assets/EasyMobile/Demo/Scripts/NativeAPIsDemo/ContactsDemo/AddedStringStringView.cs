using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    [Serializable]
    public class AddedStringStringView : MonoBehaviour
    {
        [SerializeField]
        private GameObject root = null;

        [SerializeField]
        private Text stringView = null;

        [SerializeField]
        private Button removeButton = null;

        public event Action OnDestroy;

        public void Setup(string label, string value)
        {
            removeButton.onClick.AddListener(() => Destroy(true));
            stringView.text = string.Format("Label: {0}, Value: {1}", label ?? "empty", value ?? "empty");
            root.SetActive(true);
        }

        public void Destroy(bool invokeCallback)
        {
            if (invokeCallback && OnDestroy != null)
                OnDestroy();
                
            GameObject.Destroy(root);
        }
    }
}
