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
    /// <summary>
    /// An object that defines fetching options to use while fetching contacts.
    /// </summary>
    internal class CNContactFetchRequest : iOSObjectProxy
    {
        internal CNContactFetchRequest(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// A Boolean value that indicates whether to return mutable contacts.
        /// </summary>
        /// <value><c>true</c> if mutable objects; otherwise, <c>false</c>.</value>
        public bool MutableObjects
        {
            get
            {
                return C.CNContactFetchRequest_mutableObjects(SelfPtr());
            }
            set
            {
                C.CNContactFetchRequest_setMutableObjects(SelfPtr(), value);
            }
        }

        /// <summary>
        /// A Boolean value that indicates whether to return linked contacts as unified contacts.
        /// </summary>
        /// <value><c>true</c> if unify results; otherwise, <c>false</c>.</value>
        public bool UnifyResults
        {
            get
            {
                return C.CNContactFetchRequest_unifyResults(SelfPtr());
            }
            set
            {
                C.CNContactFetchRequest_setUnifyResults(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The sort order for contacts.
        /// </summary>
        /// <value>The sort order.</value>
        public CNContactSortOrder SortOrder
        {
            get
            {
                return C.CNContactFetchRequest_sortOrder(SelfPtr());
            }
            set
            {
                C.CNContactFetchRequest_setSortOrder(SelfPtr(), value);
            }
        }

        /// <summary>
        /// The designated initializer for a fetch request that uses the specified keys.
        /// </summary>
        /// <returns>The with keys to fetch.</returns>
        /// <param name="keysToFetch">Keys to fetch.</param>
        public static CNContactFetchRequest InitWithKeysToFetch(NSArray<NSString> keysToFetch)
        {
            if (keysToFetch == null)
                return null;

            // This will automatically call alloc on native side before calling the init method.
            var ptr = C.CNContactFetchRequest_initWithKeysToFetch(keysToFetch.ToPointer());
            CNContactFetchRequest request = null;
            if (PInvokeUtil.IsNotNull(ptr))
            {
                request = new CNContactFetchRequest(ptr);
                CFFunctions.CFRelease(ptr);
            }

            return request;
        }

        /// <summary>
        /// The properties to fetch in the returned contacts.
        /// </summary>
        /// <value>The keys to fetch.</value>
        public NSArray<CNKeyDescriptor> KeysToFetch
        {
            get
            {
                NSArray<CNKeyDescriptor> keys = null;
                var ptr = C.CNContactFetchRequest_keysToFetch(SelfPtr());
                if (PInvokeUtil.IsNotNull(ptr))
                {
                    keys = new NSArray<CNKeyDescriptor>(ptr);
                    CFFunctions.CFRelease(ptr);
                }
                return keys;
            }
            set
            {
                if (value != null)
                    C.CNContactFetchRequest_setKeysToFetch(SelfPtr(), value.ToPointer());
            }
        }

        /// <summary>
        /// The predicate to match contacts against.
        /// </summary>
        /// <value>The predicate.</value>
        public NSPredicate Predicate
        {
            get
            {
                NSPredicate pred = null;
                var ptr = C.CNContactFetchRequest_predicate(SelfPtr());
                if (PInvokeUtil.IsNotNull(ptr))
                {
                    pred = new NSPredicate(ptr);
                    CFFunctions.CFRelease(ptr);
                }
                return pred;
            }
            set
            {
                C.CNContactFetchRequest_setPredicate(SelfPtr(), value == null ? IntPtr.Zero : value.ToPointer());
            }
        }

#region C wrapper

        private static class C
        {
            //  Contact Fetching Options
            [DllImport("__Internal")]
            internal static extern bool CNContactFetchRequest_mutableObjects(HandleRef selfPointer);

            [DllImport("__Internal")]
            internal static extern void CNContactFetchRequest_setMutableObjects(HandleRef selfPointer, bool mutableObjects);

            [DllImport("__Internal")]
            internal static extern bool CNContactFetchRequest_unifyResults(HandleRef selfPointer);

            [DllImport("__Internal")]
            internal static extern void CNContactFetchRequest_setUnifyResults(HandleRef selfPointer, bool unifyResults);

            [DllImport("__Internal")]
            internal static extern CNContactSortOrder CNContactFetchRequest_sortOrder(HandleRef selfPointer);

            [DllImport("__Internal")]
            internal static extern void CNContactFetchRequest_setSortOrder(HandleRef selfPointer, CNContactSortOrder sortOrder);

            //  Keys Used in Fetching Contacts
            [DllImport("__Internal")]
            internal static extern /* CNContactFetchRequest */IntPtr CNContactFetchRequest_initWithKeysToFetch(/* NSArray<id<CNKeyDescriptor>> */IntPtr keysToFetch);

            [DllImport("__Internal")]
            internal static extern /* NSArray<id<CNKeyDescriptor>> */IntPtr CNContactFetchRequest_keysToFetch(HandleRef selfPointer);

            [DllImport("__Internal")]
            internal static extern void CNContactFetchRequest_setKeysToFetch(HandleRef selfPointer, /* NSArray<id<CNKeyDescriptor>> */IntPtr keysToFetch);

            [DllImport("__Internal")]
            internal static extern /* NSPredicate */IntPtr CNContactFetchRequest_predicate(HandleRef selfPointer);

            [DllImport("__Internal")]
            internal static extern void CNContactFetchRequest_setPredicate(HandleRef selfPointer, /* NSPredicate */IntPtr predicate);
        }

#endregion

    }
}
#endif