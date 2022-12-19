#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;
using EasyMobile.iOS.CoreFoundation;

namespace EasyMobile.iOS.Foundation
{

    internal class NSPredicate : iOSObjectProxy
    {
        private NSString mPredicateFormat;

        internal NSPredicate(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        /// <summary>
        /// Creates and returns a new predicate formed by creating a new string with a given format and parsing the result.
        /// </summary>
        /// <returns>The with format.</returns>
        /// <param name="predicateFormat">Predicate format.</param>
        public static NSPredicate PredicateWithFormat(NSString predicateFormat)
        {
            if (predicateFormat == null)
                return null;

            NSPredicate predicate = null;
            var ptr = C.NSPredicate_predicateWithFormat(predicateFormat.ToPointer());

            if (PInvokeUtil.IsNotNull(ptr))
            {
                predicate = new NSPredicate(ptr);
                CFFunctions.CFRelease(ptr);
            }
            return predicate;
        }

        /// <summary>
        /// Creates and returns a predicate that always evaluates to a given Boolean value.
        /// </summary>
        /// <returns>The with value.</returns>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static NSPredicate PredicateWithValue(bool value)
        {
            NSPredicate predicate = null;
            var ptr = C.NSPredicate_predicateWithValue(value);

            if (PInvokeUtil.IsNotNull(ptr))
            {
                predicate = new NSPredicate(ptr);
                CFFunctions.CFRelease(ptr);
            }
            return predicate;
        }

        /// <summary>
        /// The predicate's format string.
        /// </summary>
        /// <value>The predicate format.</value>
        public NSString PredicateFormat
        {
            get
            {
                if (mPredicateFormat == null)
                {
                    var ptr = C.NSPredicate_predicateFormat(SelfPtr());

                    if (PInvokeUtil.IsNotNull(ptr))
                    {
                        mPredicateFormat = new NSString(ptr);
                        CFFunctions.CFRelease(ptr);
                    }
                }
                return mPredicateFormat;
            }
        }

        #region C Wrapper

        private static class C
        {
            // Creating a Predicate
            [DllImport("__Internal")]
            internal static extern /* NSPredicate */IntPtr NSPredicate_predicateWithFormat(/* NSString */IntPtr predicateFormat);

            [DllImport("__Internal")]
            internal static extern /* NSPredicate */IntPtr NSPredicate_predicateWithValue(bool value);

            // Getting a String Representation
            [DllImport("__Internal")]
            internal static extern /* NSString */IntPtr NSPredicate_predicateFormat(HandleRef pointer);
        }

        #endregion
    }
}
#endif