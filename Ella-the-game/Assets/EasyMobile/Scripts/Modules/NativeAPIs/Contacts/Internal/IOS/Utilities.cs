#if UNITY_IOS && EM_CONTACTS
using System;
using System.Collections.Generic;
using EasyMobile.iOS.Contacts;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.UIKit;
using UnityEngine;

namespace EasyMobile.Internal.NativeAPIs.Contacts
{
    internal static class Utilities
    {
        #region Public Methods

        public static Contact ToContact(this CNContact cnContact)
        {
            if (cnContact == null)
                return null;

            return new Contact
            {
                Id = cnContact.Identifier != null ? cnContact.Identifier.UTF8String : string.Empty,
                FirstName = cnContact.GivenName != null ? cnContact.GivenName.UTF8String : string.Empty,
                MiddleName = cnContact.MiddleName != null ? cnContact.MiddleName.UTF8String : string.Empty,
                LastName = cnContact.FamilyName != null ? cnContact.FamilyName.UTF8String : string.Empty,
                Company = cnContact.OrganizationName != null ? cnContact.OrganizationName.UTF8String : string.Empty,
                Birthday = cnContact.Birthday != null ? new DateTime?(cnContact.Birthday.ToDateTime()) : null,
                Emails = ToContactEmails(cnContact.EmailAddresses),
                PhoneNumbers = ToContactPhoneNumbers(cnContact.PhoneNumbers),
                loadPhotoFunc = () =>
                {
                    if (cnContact.ImageData == null)
                        return null;

                    UIImage image = UIImage.ImageWithData(cnContact.ImageData);
                    return TextureUtilities.Decode(UIImage.UIImagePNGRepresentation(image.NormalizedImage));
                }
            };
        }

        public static CNMutableContact ToCNMutableContact(this Contact contact)
        {
            if (contact == null)
                return null;

            CNMutableContact nativeContact = new CNMutableContact();

            if (contact.FirstName != null)
                nativeContact.GivenName = NSString.StringWithUTF8String(contact.FirstName);

            if (contact.MiddleName != null)
                nativeContact.MiddleName = NSString.StringWithUTF8String(contact.MiddleName);

            if (contact.LastName != null)
                nativeContact.FamilyName = NSString.StringWithUTF8String(contact.LastName);

            if (contact.Company != null)
                nativeContact.OrganizationName = NSString.StringWithUTF8String(contact.Company);


            if (contact.Birthday != null)
                nativeContact.Birthday = contact.Birthday.Value.ToNSDateComponents();

            nativeContact.EmailAddresses = ToCNCotactEmails(contact.Emails);
            nativeContact.PhoneNumbers = ToCNContactPhoneNumbers(contact.PhoneNumbers);

            if (contact.Photo != null)
            {
                byte[] rawData = TextureUtilities.EncodeAsByteArray(contact.Photo, ImageFormat.PNG);
                nativeContact.ImageData = NSData.DataWithBytes(rawData, (uint)rawData.Length);
            }

            return nativeContact;
        }

        public static DateTime ToDateTime(this NSDateComponents dateComponents)
        {
            if (dateComponents == null)
                return default(DateTime);

            /// NSDateComponents allows empty year when DateTime doesn't,
            /// so we need to do this instead.
            int year = dateComponents.Year < 0 ? DateTime.Now.Year : dateComponents.Year;

            return new DateTime(year, dateComponents.Month, dateComponents.Day);
        }

        public static NSDateComponents ToNSDateComponents(this DateTime dateTime)
        {
            return new NSDateComponents()
            {
                Day = dateTime.Day,
                Month = dateTime.Month,
                Year = dateTime.Year,
            };
        }

        #endregion

        #region Private Methods

        private static KeyValuePair<string, string>[] ToContactPhoneNumbers(NSArray<CNLabeledValue<CNPhoneNumber>> phoneNumbers)
        {
            var result = new List<KeyValuePair<string, string>>();
            if (phoneNumbers == null || phoneNumbers.Count < 1)
                return result.ToArray();

            foreach (var labeledValue in phoneNumbers.ToArray(ptr => new CNLabeledValue<CNPhoneNumber>(ptr)))
            {
                if (labeledValue == null)
                {
                    Debug.Log("Detected a null [CNLabeledValue<CNPhoneNumber>].");
                    continue;
                }

                Func<IntPtr, CNPhoneNumber> valueConstructor = ptr => new CNPhoneNumber(ptr);
                CNPhoneNumber number = labeledValue.GetValue(valueConstructor);
                NSString label = labeledValue.Label;
                if(number == null)
                {
                    Debug.LogFormat("Detected null number value in [CNLabeledValue<CNPhoneNumber>] -> label:{0}.",
                        label != null ? label.UTF8String : "null");
                    continue;
                }
                var labelStr = label != null ? label.UTF8String : "null";
                var numberStr = number != null ? number.StringValue.UTF8String : "null";

                result.Add(new KeyValuePair<string, string>(labelStr, numberStr));
            }

            return result.ToArray();
        }

        private static KeyValuePair<string, string>[] ToContactEmails(NSArray<CNLabeledValue<NSString>> emails)
        {
            var result = new List<KeyValuePair<string, string>>();
            if (emails == null || emails.Count < 1)
                return result.ToArray();

            foreach (var labeledValue in emails.ToArray(ptr => new CNLabeledValue<NSString>(ptr)))
            {
                if (labeledValue == null)
                {
                    Debug.Log("Detected a null [CNLabeledValue<NSString>].");
                    continue;
                }

                Func<IntPtr, NSString> valueConstructor = ptr => new NSString(ptr);
                NSString email = labeledValue.GetValue(valueConstructor);
                NSString label = labeledValue.Label;

                if (label == null || email == null)
                {
                    Debug.LogFormat("Detected null value in [CNLabeledValue<NSString>] -> [{0}, {1}].",
                        label != null ? label.UTF8String : "null",
                        email != null ? email.UTF8String : "null");
                    continue;
                }

                result.Add(new KeyValuePair<string, string>(label.UTF8String, email.UTF8String));
            }

            return result.ToArray();
        }

        private static NSArray<CNLabeledValue<CNPhoneNumber>> ToCNContactPhoneNumbers(KeyValuePair<string, string>[] phoneNumbers)
        {
            if (phoneNumbers == null)
                return null;

            var result = new NSMutableArray<CNLabeledValue<CNPhoneNumber>>();
            foreach (var phoneNumber in phoneNumbers)
            {
                NSString label = NSString.StringWithUTF8String(phoneNumber.Key);
                CNPhoneNumber number = CNPhoneNumber.PhoneNumberWithStringValue(phoneNumber.Value);
                result.AddObject(CNLabeledValue<CNPhoneNumber>.LabeledValueWithLabel(label, number));
            }
            return result;
        }

        private static NSArray<CNLabeledValue<NSString>> ToCNCotactEmails(KeyValuePair<string, string>[] emails)
        {
            if (emails == null)
                return null;

            var result = new NSMutableArray<CNLabeledValue<NSString>>();
            foreach (var email in emails)
            {
                NSString label = NSString.StringWithUTF8String(email.Key);
                NSString nativeEmail = NSString.StringWithUTF8String(email.Value);
                result.AddObject(CNLabeledValue<NSString>.LabeledValueWithLabel(label, nativeEmail));
            }
            return result;
        }

        #endregion
    }
}
#endif