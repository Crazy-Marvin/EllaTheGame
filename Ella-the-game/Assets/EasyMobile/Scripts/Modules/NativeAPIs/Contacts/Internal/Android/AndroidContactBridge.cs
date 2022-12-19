#if UNITY_ANDROID && EM_CONTACTS
using System;
using System.Linq;
using UnityEngine;
using EasyMobile.Android;
using System.Collections.Generic;
using System.Globalization;

namespace EasyMobile.Internal.NativeAPIs.Contacts
{
    internal class AndroidContactBridge : EMAndroidJavaObject
    {
        private const string NativeClassName = "com.sglib.easymobile.androidnative.contacts.Contact";
        private const string NativeId = "id";
        private const string NativeFirstName = "firstName";
        private const string NativeMiddleName = "middleName";
        private const string NativeLastName = "lastName";
        private const string NativeCompany = "company";
        private const string NativeBirthdayString = "birthdayString";
        private const string NativePhoneNumber = "getPhoneNumbers";
        private const string NativeEmail = "getEmails";
        private const string NativeEncodedPhoto = "getPhotoAsBase64String";

        public AndroidContactBridge() : base(NativeClassName) { }

        public AndroidContactBridge(AndroidJavaObject nativeObject) : base(nativeObject) { }

        public string Id
        {
            get { return Get<string>(NativeId); }
        }

        public string FirstName
        {
            get { return Get<string>(NativeFirstName); }
        }

        public string MiddleName
        {
            get { return Get<string>(NativeMiddleName); }
        }

        public string LastName
        {
            get { return Get<string>(NativeLastName); }
        }

        public string Company
        {
            get { return Get<string>(NativeCompany); }
        }

        public DateTime? Birthday
        {
            get
            {
                string birthdayString = Get<string>(NativeBirthdayString);
                if (string.IsNullOrEmpty(birthdayString))
                    return null;

                return DateTime.ParseExact(birthdayString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
        }

        public KeyValuePair<string, string>[] PhoneNumbers
        {
            get
            {
                var nativeObjects = Call<AndroidJavaObject[]>(NativePhoneNumber);

                if (nativeObjects == null)
                    return null;

                if (nativeObjects.Length < 1)
                    return new KeyValuePair<string, string>[0];
                    
                return nativeObjects.Select(no => new StringStringPairBridge(no).ToKeyValuePair()).ToArray();
            }
        }

        public KeyValuePair<string, string>[] Emails
        {
            get
            {
                var nativeObjects = Call<AndroidJavaObject[]>(NativeEmail);

                if (nativeObjects == null)
                    return null;

                if (nativeObjects.Length < 1)
                    return new KeyValuePair<string, string>[0];
                    
                return nativeObjects.Select(no => new StringStringPairBridge(no).ToKeyValuePair()).ToArray();
            }
        }

        public Func<Texture2D> LoadPhotoFunc
        {
            get
            {
                return () =>
                {
                    var encodedPhoto = Call<string>(NativeEncodedPhoto);
                    if (string.IsNullOrEmpty(encodedPhoto))
                        return null;

                    return TextureUtilities.Decode(encodedPhoto);
                };
            }
        }

        public Contact ToContact()
        {
            return (Contact)this;
        }

        public static explicit operator Contact(AndroidContactBridge bridge)
        {
            if (bridge == null)
                return null;

            return new Contact()
            {
                Id = bridge.Id,
                FirstName = bridge.FirstName,
                MiddleName = bridge.MiddleName,
                LastName = bridge.LastName,
                Company = bridge.Company,
                Birthday = bridge.Birthday,
                PhoneNumbers = bridge.PhoneNumbers,
                Emails = bridge.Emails,
                loadPhotoFunc = bridge.LoadPhotoFunc
            };
        }
    }
}
#endif
