using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    public class ContactView : MonoBehaviour
    {
        [SerializeField]
        private RawImage avatarImage = null;

        [SerializeField]
        private Text infoText = null;

        [SerializeField]
        private Button deleteButton = null, loadPhotoButton = null;

        public Button DeleteButton { get { return deleteButton; } }

        private Contact contact;

        public void UpdateContact(Contact contact)
        {
            this.contact = contact;
            infoText.text = GetContactInfoText();

            loadPhotoButton.onClick.RemoveAllListeners();
            loadPhotoButton.onClick.AddListener(LoadPhoto);

            UpdateAvatar();
        }

        public Contact GetContact()
        {
            return this.contact;
        }

        private string GetContactInfoText()
        {
            if (contact == null)
                return "null";

            return contact.ToString();
        }

        private void LoadPhoto()
        {
            if (contact == null)
                return;

            contact.LoadPhoto();
            UpdateAvatar();
        }

        private void UpdateAvatar()
        {
            if (contact.IsPhotoLoaded)
            {
                avatarImage.texture = contact.Photo;
                avatarImage.gameObject.SetActive(true);
                loadPhotoButton.gameObject.SetActive(false);
            }
        }
    }
}
