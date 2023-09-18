using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using EasyMobile.Internal;
using System.Globalization;
using System.Collections;

namespace EasyMobile.Demo
{
    public class ContactDemo : MonoBehaviour
    {
        [SerializeField]
        private GameObject curtain = null;

        [SerializeField]
        private uint avatarWidth = 256, avatarHeight = 256;

        [SerializeField]
        private Color[] avatarColors = new Color[] { Color.black, Color.white };

        [SerializeField]
        private ContactView contactViewPrefab = null;

        [SerializeField]
        private Transform contactViewRoot = null;

        [SerializeField]
        private InputField firstnNameInput = null,
                           middleNameInput = null,
                           lastNameInput = null,
                           companyInput = null,
                           birthdayInput = null,
                           emailLabelInput = null, emailInput = null,
                           phoneNumberLabelInput = null, phoneNumberInput = null;

        [SerializeField]
        private RawImage avatarImage = null;

        [SerializeField]
        private StringStringCollectionView collectionView = null;

        [SerializeField]
        private Button getAllContactsButton = null,
            addContactButton = null,
            generateAvatarButton = null,
            pickContactsButton = null,
            clearAvatarButton = null,
            clearCreatedContactsButton = null,
            addEmailButton = null,
            addPhoneNumberButton = null,
            editEmailsButton = null,
            editPhoneNumbersButton = null;

        private List<ContactView> createdViews = new List<ContactView>();
        private List<StringStringKeyValuePair> addedEmails = new List<StringStringKeyValuePair>();
        private List<StringStringKeyValuePair> addedPhoneNumbers = new List<StringStringKeyValuePair>();

        private void Awake()
        {
            if (!EM_Settings.IsSubmoduleEnable(Submodule.Contacts))
            {
                curtain.SetActive(true);
                return;
            }
            else
            {
                curtain.SetActive(false);
            }

            getAllContactsButton.onClick.AddListener(GetAllContacts);
            addContactButton.onClick.AddListener(AddContact);
            generateAvatarButton.onClick.AddListener(GenerateAvatar);
            pickContactsButton.onClick.AddListener(PickContacts);
            clearAvatarButton.onClick.AddListener(ClearAvatar);
            clearCreatedContactsButton.onClick.AddListener(ClearContactViews);

            addEmailButton.onClick.AddListener(AddEmail);
            addPhoneNumberButton.onClick.AddListener(AddPhoneNumber);
            editEmailsButton.onClick.AddListener(() => collectionView.Show(addedEmails, "Emails"));
            editPhoneNumbersButton.onClick.AddListener(() => collectionView.Show(addedPhoneNumbers, "Phone Numbers"));

            firstnNameInput.keyboardType = TouchScreenKeyboardType.NamePhonePad;
            middleNameInput.keyboardType = TouchScreenKeyboardType.NamePhonePad;
            lastNameInput.keyboardType = TouchScreenKeyboardType.NamePhonePad;
            emailInput.keyboardType = TouchScreenKeyboardType.EmailAddress;
            phoneNumberInput.keyboardType = TouchScreenKeyboardType.PhonePad;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public void GetAllContacts()
        {
            StartCoroutine(CRGetAllContacts());
        }

        IEnumerator CRGetAllContacts()
        {
            yield return StartCoroutine(CRRequestPermission(AndroidPermission.AndroidPermissionReadContacts));

            DeviceContacts.GetContacts((error, contacts) =>
            {
                if (!string.IsNullOrEmpty(error))
                    NativeUI.Alert("Get All Contacts Error", error);

                if (contacts != null)
                {
                    foreach (var contact in contacts)
                        AddContactView(contact);
                }
            });
        }

        public void AddContact()
        {
            StartCoroutine(CRAddContact());
        }

        IEnumerator CRAddContact()
        {
            yield return StartCoroutine(CRRequestPermission(AndroidPermission.AndroidPermissionWriteContacts));

            string firstName = firstnNameInput.text;
            string middleName = middleNameInput.text;
            string lastName = lastNameInput.text;
            string company = companyInput.text;

            DateTime? birthday = null;
            if (!string.IsNullOrEmpty(birthdayInput.text))
            {
                DateTime parseBirthday;
                if (DateTime.TryParseExact(birthdayInput.text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parseBirthday))
                {
                    birthday = parseBirthday;
                }
                else
                {
                    NativeUI.Alert("Invalid birthday", "Can't convert birthday to yyyy-MM-dd format.");
                    yield break;
                }
            }

            var avatar = avatarImage.texture != null ? (Texture2D)avatarImage.texture : null;
            var error = DeviceContacts.AddContact(new Contact()
            {
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName,
                Company = company,
                Birthday = birthday,
                Emails = addedEmails.Select(email => new KeyValuePair<string, string>(email.Key, email.Value)).ToArray(),
                PhoneNumbers = addedPhoneNumbers.Select(phoneNumber => new KeyValuePair<string, string>(phoneNumber.Key, phoneNumber.Value)).ToArray(),
                Photo = avatar
            });

            NativeUI.Alert("Add Contact", error ?? "Added new contact successfully");
        }

        public void DeleteContact(ContactView view, Contact contact)
        {
            StartCoroutine(CRDeleteContact(view, contact));
        }

        private IEnumerator CRDeleteContact(ContactView view, Contact contact)
        {
            yield return StartCoroutine(CRRequestPermission(AndroidPermission.AndroidPermissionWriteContacts));

            if (contact == null)
                yield break;

            var message = string.Format("Delete contact with id: " + (contact.Id ?? "null"));
            var alert = NativeUI.ShowTwoButtonAlert("Delete Contact", message, "Yes", "No");
            alert.OnComplete += button =>
            {
                if (button == 1) // Click No
                    return;

                var deleteResult = DeviceContacts.DeleteContact(contact);
                if (deleteResult != null)
                {
                    Debug.LogWarning(deleteResult);
                    return;
                }

                createdViews.Remove(view);
                Destroy(view.gameObject);
            };
        }

        public void PickContacts()
        {
            StartCoroutine(CRPickContacts());
        }

        IEnumerator CRPickContacts()
        {
            yield return StartCoroutine(CRRequestPermission(AndroidPermission.AndroidPermissionReadContacts));

            DeviceContacts.PickContact((error, contact) =>
            {
                if (contact != null)
                {
                    AddContactView(contact);
                    return;
                }

                string errorMessage = error ?? "Received an empty contact with unknown error.";
                NativeUI.Alert("Pick Contact Error", errorMessage);
            });
        }

        IEnumerator CRRequestPermission(string perm)
        {
#if UNITY_2018_3_OR_NEWER
            Debug.Log("Requesting permission " + perm);
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(perm))
            {
                // Request for permission and wait until the user respond...
                UnityEngine.Android.Permission.RequestUserPermission(perm);
                while (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(perm))
                    yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield break;
            }
#else
            yield break;
#endif
        }

        private void AddContactView(Contact contact)
        {
            var contactView = Instantiate(contactViewPrefab, contactViewRoot);
            contactView.UpdateContact(contact);
            contactView.gameObject.SetActive(true);
            contactView.DeleteButton.onClick.AddListener(() => DeleteContact(contactView, contact));
            createdViews.Add(contactView);
        }

        private void ClearContactViews()
        {
            if (createdViews == null || createdViews.Count < 1)
                return;

            foreach (var view in createdViews)
                Destroy(view.gameObject);

            createdViews.Clear();
        }

        private void GenerateAvatar()
        {
            avatarImage.texture = TextureGenerator.GenerateRandomTexture2D((int)avatarWidth, (int)avatarHeight, avatarColors);
        }

        private void ClearAvatar()
        {
            avatarImage.texture = null;
        }

        private void AddEmail()
        {
            string label = emailLabelInput.text;
            if (string.IsNullOrEmpty(label))
            {
                NativeUI.Alert("Invalid email label", "Email's label can't be empty");
                return;
            }

            string email = emailInput.text;
            if (string.IsNullOrEmpty(email))
            {
                NativeUI.Alert("Invalid email", "Email can't be empty");
                return;
            }

            addedEmails.Add(new StringStringKeyValuePair(label, email));
            NativeUI.Alert("Success", "New email has been added.");
        }

        private void AddPhoneNumber()
        {
            string label = phoneNumberLabelInput.text;
            if (string.IsNullOrEmpty(label))
            {
                NativeUI.Alert("Invalid phone number's label", "Phone number's label can't be empty");
                return;
            }

            string phoneNumber = phoneNumberInput.text;
            if (string.IsNullOrEmpty(phoneNumber))
            {
                NativeUI.Alert("Invalid phone number", "Phone number can't be empty");
                return;
            }

            addedPhoneNumbers.Add(new StringStringKeyValuePair(label, phoneNumber));
            NativeUI.Alert("Success", "New phone number has been added.");
        }
    }
}
