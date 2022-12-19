#if UNITY_IOS && EM_CONTACTS
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.Foundation;
using EasyMobile.iOS.CoreFoundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EasyMobile.iOS.Contacts
{

    internal static class Constants
    {
        private const string FrameworkName = "Contacts";

        /// <summary>
        /// Birthday.
        /// </summary>
        /// <value>The CN contact birthday key.</value>
        public static NSString CNContactBirthdayKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactBirthdayKey, FrameworkName);
            }
        }

        /// <summary>
        /// Contact dates.
        /// </summary>
        /// <value>The CN contact dates key.</value>
        public static NSString CNContactDatesKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactDatesKey, FrameworkName);
            }
        }

        /// <summary>
        /// Department name.
        /// </summary>
        /// <value>The CN contact department name key.</value>
        public static NSString CNContactDepartmentNameKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactDepartmentNameKey, FrameworkName);
            }
        }

        /// <summary>
        /// Email address.
        /// </summary>
        /// <value>The CN contact email addresses key.</value>
        public static NSString CNContactEmailAddressesKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactEmailAddressesKey, FrameworkName);
            }
        }

        /// <summary>
        /// Family name.
        /// </summary>
        /// <value>The CN contact family name key.</value>
        public static NSString CNContactFamilyNameKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactFamilyNameKey, FrameworkName);
            }
        }

        /// <summary>
        /// Given name.
        /// </summary>
        /// <value>The CN contact given name key.</value>
        public static NSString CNContactGivenNameKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactGivenNameKey, FrameworkName);
            }
        }

        /// <summary>
        /// The contact’s unique identifier.
        /// </summary>
        /// <value>The CN contact identifier key.</value>
        public static NSString CNContactIdentifierKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactIdentifierKey, FrameworkName);
            }
        }

        /// <summary>
        /// Image data availability.
        /// </summary>
        /// <value>The CN contact image data available key.</value>
        public static NSString CNContactImageDataAvailableKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactImageDataAvailableKey, FrameworkName);
            }
        }

        /// <summary>
        /// Image data.
        /// </summary>
        /// <value>The CN contact image data key.</value>
        public static NSString CNContactImageDataKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactImageDataKey, FrameworkName);
            }
        }

        /// <summary>
        /// Instant messages.
        /// </summary>
        /// <value>The CN contact instant message addresses key.</value>
        public static NSString CNContactInstantMessageAddressesKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactInstantMessageAddressesKey, FrameworkName);
            }
        }

        /// <summary>
        /// Job title.
        /// </summary>
        /// <value>The CN contact job title key.</value>
        public static NSString CNContactJobTitleKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactJobTitleKey, FrameworkName);
            }
        }

        /// <summary>
        /// Middle name.
        /// </summary>
        /// <value>The CN contact middle name key.</value>
        public static NSString CNContactMiddleNameKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactMiddleNameKey, FrameworkName);
            }
        }

        /// <summary>
        /// Name prefix.
        /// </summary>
        /// <value>The CN contact name prefix key.</value>
        public static NSString CNContactNamePrefixKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactNamePrefixKey, FrameworkName);
            }
        }

        /// <summary>
        /// Name suffix.
        /// </summary>
        /// <value>The CN contact name suffix key.</value>
        public static NSString CNContactNameSuffixKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactNameSuffixKey, FrameworkName);
            }
        }

        /// <summary>
        /// Nickname.
        /// </summary>
        /// <value>The CN contact nickname key.</value>
        public static NSString CNContactNicknameKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactNicknameKey, FrameworkName);
            }
        }

        /// <summary>
        /// Non-Gregorian birthday.
        /// </summary>
        /// <value>The CN contact non gregorian birthday key.</value>
        public static NSString CNContactNonGregorianBirthdayKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactNonGregorianBirthdayKey, FrameworkName);
            }
        }

        /// <summary>
        /// Note.
        /// </summary>
        /// <value>The CN contact note key.</value>
        public static NSString CNContactNoteKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactNoteKey, FrameworkName);
            }
        }

        /// <summary>
        /// Organization name.
        /// </summary>
        /// <value>The CN contact organization name key.</value>
        public static NSString CNContactOrganizationNameKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactOrganizationNameKey, FrameworkName);
            }
        }

        /// <summary>
        /// Phone number.
        /// </summary>
        /// <value>The CN contact phone numbers key.</value>
        public static NSString CNContactPhoneNumbersKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactPhoneNumbersKey, FrameworkName);
            }
        }

        /// <summary>
        /// Phonetic family name.
        /// </summary>
        /// <value>The CN contact phonetic family name key.</value>
        public static NSString CNContactPhoneticFamilyNameKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactPhoneticFamilyNameKey, FrameworkName);
            }
        }

        /// <summary>
        /// Phonetic given name.
        /// </summary>
        /// <value>The CN contact phonetic given name key.</value>
        public static NSString CNContactPhoneticGivenNameKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactPhoneticGivenNameKey, FrameworkName);
            }
        }

        /// <summary>
        /// Phonetic middle name.
        /// </summary>
        /// <value>The CN contact phonetic middle name key.</value>
        public static NSString CNContactPhoneticMiddleNameKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactPhoneticMiddleNameKey, FrameworkName);
            }
        }

        public static NSString CNContactPhoneticOrganizationNameKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactPhoneticOrganizationNameKey, FrameworkName);
            }
        }

        /// <summary>
        /// Postal address.
        /// </summary>
        /// <value>The CN contact postal addresses key.</value>
        public static NSString CNContactPostalAddressesKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactPostalAddressesKey, FrameworkName);
            }
        }

        /// <summary>
        /// Previous family name.
        /// </summary>
        /// <value>The CN contact previous family name key.</value>
        public static NSString CNContactPreviousFamilyNameKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactPreviousFamilyNameKey, FrameworkName);
            }
        }

        /// <summary>
        /// Exception thrown when an accessed property was not fetched.
        /// </summary>
        /// <value>The name of the CN contact property not fetched exception.</value>
        public static NSString CNContactPropertyNotFetchedExceptionName
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactPropertyNotFetchedExceptionName, FrameworkName);
            }
        }

        /// <summary>
        /// Contact relations.
        /// </summary>
        /// <value>The CN contact relations key.</value>
        public static NSString CNContactRelationsKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactRelationsKey, FrameworkName);
            }
        }

        /// <summary>
        /// Social profile.
        /// </summary>
        /// <value>The CN contact social profiles key.</value>
        public static NSString CNContactSocialProfilesKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactSocialProfilesKey, FrameworkName);
            }
        }

        /// <summary>
        /// Posted notifications when changes occur in another CNContactStore.
        /// </summary>
        /// <value>The CN contact store did change notification.</value>
        public static NSString CNContactStoreDidChangeNotification
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactStoreDidChangeNotification, FrameworkName);
            }
        }

        /// <summary>
        /// Thumbnail data.
        /// </summary>
        /// <value>The CN contact thumbnail image data key.</value>
        public static NSString CNContactThumbnailImageDataKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactThumbnailImageDataKey, FrameworkName);
            }
        }

        /// <summary>
        /// Contact type.
        /// </summary>
        /// <value>The CN contact type key.</value>
        public static NSString CNContactTypeKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactTypeKey, FrameworkName);
            }
        }

        /// <summary>
        /// URL Address.
        /// </summary>
        /// <value>The CN contact URL addresses key.</value>
        public static NSString CNContactUrlAddressesKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContactUrlAddressesKey, FrameworkName);
            }
        }

        /// <summary>
        /// Identifier key.
        /// </summary>
        /// <value>The CN container identifier key.</value>
        public static NSString CNContainerIdentifierKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContainerIdentifierKey, FrameworkName);
            }
        }

        /// <summary>
        /// Name key.
        /// </summary>
        /// <value>The CN container name key.</value>
        public static NSString CNContainerNameKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContainerNameKey, FrameworkName);
            }
        }

        /// <summary>
        /// Type key.
        /// </summary>
        /// <value>The CN container type key.</value>
        public static NSString CNContainerTypeKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNContainerTypeKey, FrameworkName);
            }
        }

        /// <summary>
        /// Error codes that may be returned when calling Contacts methods.
        /// </summary>
        /// <value>The CN error code.</value>
        public static NSString CNErrorCode
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNErrorCode, FrameworkName);
            }
        }

        public static NSString CNErrorDomain
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNErrorDomain, FrameworkName);
            }
        }

        /// <summary>
        /// Gets the CN group identifier key.
        /// </summary>
        /// <value>The CN group identifier key.</value>
        public static NSString CNGroupIdentifierKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNGroupIdentifierKey, FrameworkName);
            }
        }

        /// <summary>
        /// Group name.
        /// </summary>
        public static NSString CNGroupNameKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNGroupNameKey, FrameworkName);
            }
        }

        /// <summary>
        /// Instant message address service key.
        /// </summary>
        public static NSString CNInstantMessageAddressServiceKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNInstantMessageAddressServiceKey, FrameworkName);
            }
        }

        /// <summary>
        /// Instant message address user name key.
        /// </summary>
        public static NSString CNInstantMessageAddressUsernameKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNInstantMessageAddressUsernameKey, FrameworkName);
            }
        }

        /// <summary>
        /// Instant message service for AIM.
        /// </summary>
        public static NSString CNInstantMessageServiceAIM
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNInstantMessageServiceAIM, FrameworkName);
            }
        }

        /// <summary>
        /// Instant message service for Facebook.
        /// </summary>
        public static NSString CNInstantMessageServiceFacebook
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNInstantMessageServiceFacebook, FrameworkName);
            }
        }

        /// <summary>
        /// Instant message service for Gadu Gadu.
        /// </summary>
        public static NSString CNInstantMessageServiceGaduGadu
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNInstantMessageServiceGaduGadu, FrameworkName);
            }
        }

        /// <summary>
        /// Instant message service for Google Talk.
        /// </summary>
        public static NSString CNInstantMessageServiceGoogleTalk
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNInstantMessageServiceGoogleTalk, FrameworkName);
            }
        }

        /// <summary>
        /// Instant message service for ICQ.
        /// </summary>
        public static NSString CNInstantMessageServiceICQ
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNInstantMessageServiceICQ, FrameworkName);
            }
        }

        /// <summary>
        /// Instant message service for Jabber.
        /// </summary>
        public static NSString CNInstantMessageServiceJabber
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNInstantMessageServiceJabber, FrameworkName);
            }
        }

        /// <summary>
        /// Instant message service for MSN.
        /// </summary>
        public static NSString CNInstantMessageServiceMSN
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNInstantMessageServiceMSN, FrameworkName);
            }
        }

        /// <summary>
        /// Instant message service for QQ.
        /// </summary>
        public static NSString CNInstantMessageServiceQQ
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNInstantMessageServiceQQ, FrameworkName);
            }
        }

        /// <summary>
        /// Instant message service for Skype.
        /// </summary>
        public static NSString CNInstantMessageServiceSkype
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNInstantMessageServiceSkype, FrameworkName);
            }
        }

        /// <summary>
        /// Instant message service for Yahoo.
        /// </summary>
        public static NSString CNInstantMessageServiceYahoo
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNInstantMessageServiceYahoo, FrameworkName);
            }
        }

        /// <summary>
        /// Assistant.
        /// </summary>
        public static NSString CNLabelContactRelationAssistant
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelContactRelationAssistant, FrameworkName);
            }
        }

        /// <summary>
        /// Brother.
        /// </summary>
        public static NSString CNLabelContactRelationBrother
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelContactRelationBrother, FrameworkName);
            }
        }

        /// <summary>
        /// Child.
        /// </summary>
        public static NSString CNLabelContactRelationChild
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelContactRelationChild, FrameworkName);
            }
        }

        /// <summary>
        /// Father.
        /// </summary>
        public static NSString CNLabelContactRelationFather
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelContactRelationFather, FrameworkName);
            }
        }

        /// <summary>
        /// Friend.
        /// </summary>
        public static NSString CNLabelContactRelationFriend
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelContactRelationFriend, FrameworkName);
            }
        }

        /// <summary>
        /// Manager.
        /// </summary>
        public static NSString CNLabelContactRelationManager
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelContactRelationManager, FrameworkName);
            }
        }

        /// <summary>
        /// Mother.
        /// </summary>
        public static NSString CNLabelContactRelationMother
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelContactRelationMother, FrameworkName);
            }
        }

        /// <summary>
        /// Parent.
        /// </summary>
        public static NSString CNLabelContactRelationParent
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelContactRelationParent, FrameworkName);
            }
        }

        /// <summary>
        /// Partner.
        /// </summary>
        public static NSString CNLabelContactRelationPartner
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelContactRelationPartner, FrameworkName);
            }
        }

        /// <summary>
        /// Sister.
        /// </summary>
        public static NSString CNLabelContactRelationSister
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelContactRelationSister, FrameworkName);
            }
        }

        /// <summary>
        /// Spouse.
        /// </summary>
        public static NSString CNLabelContactRelationSpouse
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelContactRelationSpouse, FrameworkName);
            }
        }

        /// <summary>
        /// Anniversary date.
        /// </summary>
        public static NSString CNLabelDateAnniversary
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelDateAnniversary, FrameworkName);
            }
        }

        /// <summary>
        /// Email.
        /// </summary>
        public static NSString CNLabelEmailiCloud
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelEmailiCloud, FrameworkName);
            }
        }

        /// <summary>
        /// Home label.
        /// </summary>
        public static NSString CNLabelHome
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelHome, FrameworkName);
            }
        }

        /// <summary>
        /// Other label.
        /// </summary>
        public static NSString CNLabelOther
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelOther, FrameworkName);
            }
        }

        /// <summary>
        /// Home fax number.
        /// </summary>
        public static NSString CNLabelPhoneNumberHomeFax
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelPhoneNumberHomeFax, FrameworkName);
            }
        }

        /// <summary>
        /// Main phone number.
        /// </summary>
        public static NSString CNLabelPhoneNumberMain
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelPhoneNumberMain, FrameworkName);
            }
        }

        /// <summary>
        /// Mobile phone number.
        /// </summary>
        public static NSString CNLabelPhoneNumberMobile
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelPhoneNumberMobile, FrameworkName);
            }
        }

        /// <summary>
        /// Other fax number.
        /// </summary>
        public static NSString CNLabelPhoneNumberOtherFax
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelPhoneNumberOtherFax, FrameworkName);
            }
        }

        /// <summary>
        /// Pager phone number.
        /// </summary>
        public static NSString CNLabelPhoneNumberPager
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelPhoneNumberPager, FrameworkName);
            }
        }

        /// <summary>
        /// Work fax number.
        /// </summary>
        public static NSString CNLabelPhoneNumberWorkFax
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelPhoneNumberWorkFax, FrameworkName);
            }
        }

        /// <summary>
        /// iPhone number.
        /// </summary>
        public static NSString CNLabelPhoneNumberiPhone
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelPhoneNumberiPhone, FrameworkName);
            }
        }

        /// <summary>
        /// Identifier for the URL property.
        /// </summary>
        public static NSString CNLabelURLAddressHomePage
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelURLAddressHomePage, FrameworkName);
            }
        }

        /// <summary>
        /// Work label.
        /// </summary>
        public static NSString CNLabelWork
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelWork, FrameworkName);
            }
        }

        /// <summary>
        /// City.
        /// </summary>
        public static NSString CNPostalAddressCityKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNPostalAddressCityKey, FrameworkName);
            }
        }

        /// <summary>
        /// Country.
        /// </summary>
        public static NSString CNPostalAddressCountryKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNPostalAddressCountryKey, FrameworkName);
            }
        }

        /// <summary>
        /// ISO country code.
        /// </summary>
        public static NSString CNPostalAddressISOCountryCodeKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNPostalAddressISOCountryCodeKey, FrameworkName);
            }
        }

        /// <summary>
        /// Localized property of postal address.
        /// </summary>
        public static NSString CNPostalAddressLocalizedPropertyNameAttribute
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNPostalAddressLocalizedPropertyNameAttribute, FrameworkName);
            }
        }

        /// <summary>
        /// Postal code.
        /// </summary>
        public static NSString CNPostalAddressPostalCodeKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNPostalAddressPostalCodeKey, FrameworkName);
            }
        }

        /// <summary>
        /// Property of postal address.
        /// </summary>
        public static NSString CNPostalAddressPropertyAttribute
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNPostalAddressPropertyAttribute, FrameworkName);
            }
        }

        /// <summary>
        /// State.
        /// </summary>
        public static NSString CNPostalAddressStateKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNPostalAddressStateKey, FrameworkName);
            }
        }

        /// <summary>
        /// Street.
        /// </summary>
        public static NSString CNPostalAddressStreetKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNPostalAddressStreetKey, FrameworkName);
            }
        }

        public static NSString CNPostalAddressSubAdministrativeAreaKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNPostalAddressSubAdministrativeAreaKey, FrameworkName);
            }
        }

        public static NSString CNPostalAddressSubLocalityKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNPostalAddressSubLocalityKey, FrameworkName);
            }
        }

        /// <summary>
        /// The Facebook social profile service.
        /// </summary>
        public static NSString CNSocialProfileServiceFacebook
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNSocialProfileServiceFacebook, FrameworkName);
            }
        }

        /// <summary>
        /// The Flickr social profile service.
        /// </summary>
        public static NSString CNSocialProfileServiceFlickr
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNSocialProfileServiceFlickr, FrameworkName);
            }
        }

        /// <summary>
        /// The Game Center social profile service.
        /// </summary>
        public static NSString CNSocialProfileServiceGameCenter
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNSocialProfileServiceGameCenter, FrameworkName);
            }
        }

        /// <summary>
        /// The social profile service.
        /// </summary>
        public static NSString CNSocialProfileServiceKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNSocialProfileServiceKey, FrameworkName);
            }
        }

        /// <summary>
        /// The LinkedIn social profile service.
        /// </summary>
        public static NSString CNSocialProfileServiceLinkedIn
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNSocialProfileServiceLinkedIn, FrameworkName);
            }
        }

        /// <summary>
        /// The MySpace social profile service.
        /// </summary>
        public static NSString CNSocialProfileServiceMySpace
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNSocialProfileServiceMySpace, FrameworkName);
            }
        }

        /// <summary>
        /// The Sina Weibo social profile service.
        /// </summary>
        public static NSString CNSocialProfileServiceSinaWeibo
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNSocialProfileServiceSinaWeibo, FrameworkName);
            }
        }

        /// <summary>
        /// The Tencent Weibo social profile service.
        /// </summary>
        public static NSString CNSocialProfileServiceTencentWeibo
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNSocialProfileServiceTencentWeibo, FrameworkName);
            }
        }

        /// <summary>
        /// The Twitter social profile service.
        /// </summary>
        public static NSString CNSocialProfileServiceTwitter
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNSocialProfileServiceTwitter, FrameworkName);
            }
        }

        /// <summary>
        /// The Yelp social profile service.
        /// </summary>
        public static NSString CNSocialProfileServiceYelp
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNSocialProfileServiceYelp, FrameworkName);
            }
        }

        /// <summary>
        /// The social profile URL.
        /// </summary>
        public static NSString CNSocialProfileURLStringKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNSocialProfileURLStringKey, FrameworkName);
            }
        }

        /// <summary>
        /// The social profile user identifier.
        /// </summary>
        public static NSString CNSocialProfileUserIdentifierKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNSocialProfileUserIdentifierKey, FrameworkName);
            }
        }

        /// <summary>
        /// The social profile user name.
        /// </summary>
        public static NSString CNSocialProfileUsernameKey
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNSocialProfileUsernameKey, FrameworkName);
            }
        }

        public static NSString CNLabelContactRelationDaughter
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelContactRelationDaughter, FrameworkName);
            }
        }

        public static NSString CNLabelContactRelationSon
        {
            get
            { 
                return iOSInteropUtil.LookupStringConstant(
                    () => Constants.CNLabelContactRelationSon, FrameworkName);
            }
        }
                                                        
    }
}
#endif