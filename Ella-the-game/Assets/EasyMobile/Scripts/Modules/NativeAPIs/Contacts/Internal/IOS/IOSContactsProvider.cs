#if UNITY_IOS && EM_CONTACTS
using System;
using System.Threading;
using System.Collections.Generic;
using EasyMobile.iOS.UIKit;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.Contacts;
using EasyMobile.iOS.ContactsUI;
using UnityEngine;

namespace EasyMobile.Internal.NativeAPIs.Contacts
{
    internal class iOSContactsProvider : INativeContactsProvider
    {
        private const string ExecuteSaveRequestFailedMessage = "Failed to execute save request.";
        private const string FetchingContactsThreadRunningMessage = "Another fetching contacts progress is running.";

        public bool IsFetchingContacts
        {
            get
            {
                return isFetchingAllContacts;
            }
        }

        private bool isFetchingAllContacts = false;

        public void GetContacts(Action<string, Contact[]> callback)
        {
            if (isFetchingAllContacts)
            {
                callback(FetchingContactsThreadRunningMessage, null);
                return;
            }

            isFetchingAllContacts = true;
            new Thread(() =>
            {
                try
                {
                    List<Contact> result = new List<Contact>();

                    var contactStore = InteropObjectFactory<CNContactStore>.Create(
                        () => new CNContactStore(),
                        c => c.ToPointer());

                    CNContactFetchRequest request = CNContactFetchRequest.InitWithKeysToFetch(GetPropertyKeys());
                    request.SortOrder = CNContactSortOrder.CNContactSortOrderUserDefault;
                    request.UnifyResults = true;

                    NSError error = null;
                    contactStore.EnumerateContactsWithFetchRequest(request, out error, (CNContact cnContact, out bool stop) =>
                    {
                        stop = false;
                        result.Add(cnContact.ToContact());
                    });

                    if (error != null && !string.IsNullOrEmpty(error.ToString()))
                        Debug.LogError(error.ToString());

                    RuntimeHelper.RunOnMainThread(() => callback(null, result.ToArray()));
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    RuntimeHelper.RunOnMainThread(() => callback(e.Message, null));
                }
                finally
                {
                    isFetchingAllContacts = false;
                }
            }).Start();
        }

        public string AddContact(Contact contact)
        {
            try
            {
                CNContactStore contactStore = new CNContactStore();
                CNSaveRequest saveRequest = new CNSaveRequest();

                CNMutableContact nativeContact = contact.ToCNMutableContact();
                saveRequest.AddContactToContainerWithIdentifier(nativeContact, null);

                NSError saveError;
                bool executeSuccess = contactStore.ExecuteSaveRequest(saveRequest, out saveError);

                return executeSuccess
                    ? null
                    : saveError != null
                        ? saveError.LocalizedDescription
                        : ExecuteSaveRequestFailedMessage;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string DeleteContact(string id)
        {
            try
            {
                CNContactStore contactStore = new CNContactStore();
                CNSaveRequest saveRequest = new CNSaveRequest();

                NSError searchError;
                NSString searchId = NSString.StringWithUTF8String(id);
                CNContact nativeContact = contactStore.UnifiedContactWithIdentifier(searchId, GetPropertyKeys(), out searchError);

                if (searchError != null)
                    return searchError.LocalizedDescription;

                saveRequest.DeleteContact(nativeContact.MutableCopy());

                NSError deleteError;
                bool executeSuccess = contactStore.ExecuteSaveRequest(saveRequest, out deleteError);
                return executeSuccess
                    ? null
                    : deleteError != null
                        ? deleteError.LocalizedDescription
                        : ExecuteSaveRequestFailedMessage;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public void PickContact(Action<string, Contact> callback)
        {
            var picker = InteropObjectFactory<CNContactPickerViewController>.Create(
                    () => new CNContactPickerViewController(),
                    c => c.ToPointer());

            picker.DisplayedPropertyKeys = GetPropertyKeys();

            picker.Delegate = new iOSContactsPickerCallback(callback, picker);

            using (var unityVC = UIViewController.UnityGetGLViewController())
                unityVC.PresentViewController(picker, true, null);
        }

        private NSArray<NSString> GetPropertyKeys()
        {
            NSMutableArray<NSString> result = new NSMutableArray<NSString>();
            result.AddObject(Constants.CNContactIdentifierKey);
            result.AddObject(Constants.CNContactFamilyNameKey);
            result.AddObject(Constants.CNContactMiddleNameKey);
            result.AddObject(Constants.CNContactGivenNameKey);
            result.AddObject(Constants.CNContactOrganizationNameKey);
            result.AddObject(Constants.CNContactBirthdayKey);
            result.AddObject(Constants.CNContactEmailAddressesKey);
            result.AddObject(Constants.CNContactPhoneNumbersKey);
            result.AddObject(Constants.CNContactImageDataKey);
            return result;
        }
    }
}

#endif // UNITY_IOS