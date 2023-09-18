#if UNITY_IOS
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.Internal.iOS;

namespace EasyMobile.iOS.Foundation
{
    using C = InternalNSDictionary;

    /// <summary>
    /// This class is intended for working with iOS NSDictionary that
    /// contains interop objects as key.
    /// </summary>
    internal class NSDictionary<T, U> : iOSObjectProxy
    {
        internal NSDictionary(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        #region NSDictionary API

        /// <summary>
        /// The number of entries in the dictionary.
        /// </summary>
        /// <value>The count.</value>
        public uint Count
        {
            get { return C.NSDictionary_count(SelfPtr()); }
        }

        /// <summary>
        /// A new array containing the dictionary’s keys, or an empty array if the dictionary has no entries.
        /// </summary>
        /// <value>All keys.</value>
        public NSArray<T> AllKeys
        {
            get
            {
                var ptr = C.NSDictionary_allKeys(SelfPtr());
                var allKeys = InteropObjectFactory<NSArray<T>>.FromPointer(ptr, p => new NSArray<T>(p));
                CoreFoundation.CFFunctions.CFRelease(ptr);
                return allKeys;
            }
        }

        /// <summary>
        /// A new array containing the dictionary’s values, or an empty array if the dictionary has no entries.
        /// </summary>
        /// <value>All values.</value>
        public NSArray<U> AllValues
        {
            get
            {
                var ptr = C.NSDictionary_allValues(SelfPtr());
                var allValues = InteropObjectFactory<NSArray<U>>.FromPointer(ptr, p => new NSArray<U>(p));
                CoreFoundation.CFFunctions.CFRelease(ptr);
                return allValues;
            }
        }

        /// <summary>
        /// Returns the value associated with a given key.
        /// </summary>
        /// <returns>The for key.</returns>
        /// <param name="key">Key.</param>
        /// <param name="keyConverter">Key converter.</param>
        /// <param name="valueConverter">Value converter.</param>
        public U ObjectForKey(T key, Func<T, IntPtr> keyToPointer, Func<IntPtr, U> pointerToValue)
        {
            IntPtr objPtr = C.NSDictionary_objectForKey(SelfPtr(), keyToPointer(key));
            U obj = pointerToValue(objPtr);
            CoreFoundation.CFFunctions.CFRelease(objPtr);
            return obj;
        }

        /// <summary>
        /// Returns the value associated with a given key.
        /// </summary>
        /// <returns>The for key.</returns>
        /// <param name="key">Key.</param>
        /// <param name="valueConverter">Value converter.</param>
        public U ValueForKey(NSString key, Func<IntPtr, U> pointerToValue)
        {
            IntPtr objPtr = C.NSDictionary_valueForKey(SelfPtr(), key.ToPointer());
            U obj = pointerToValue(objPtr);
            CoreFoundation.CFFunctions.CFRelease(objPtr);
            return obj;
        }

        #endregion

        #region C# Utils

        /// <summary>
        /// Converts to C# dictionary, used when the NSDictionary key is not of type <see cref="NSString"/>.
        /// </summary>
        /// <returns>The dictionary.</returns>
        /// <param name="pointerToKey">Pointer to key.</param>
        /// <param name="keyToPointer">Key to pointer.</param>
        /// <param name="pointerToValue">Pointer to value.</param>
        public virtual Dictionary<T,U> ToDictionary(
            Func<IntPtr, T> pointerToKey, 
            Func<T, IntPtr> keyToPointer, 
            Func<IntPtr, U> pointerToValue)
        {
            // Cache the native array.
            var allKeys = AllKeys;
            var count = allKeys.Count;

            // Create managed dict.
            var dict = new Dictionary<T,U>((int)count);

            // Fill in entries.
            for (uint i = 0; i < count; i++)
            {
                T key = allKeys.ObjectAtIndex(i, pointerToKey);
                U value = ObjectForKey(key, keyToPointer, pointerToValue);
                dict.Add(key, value);
            }

            return dict;
        }

        #endregion
    }

    internal static class InternalNSDictionary
    {
        [DllImport("__Internal")]
        internal static extern uint NSDictionary_count(HandleRef self);

        [DllImport("__Internal")]
        internal static extern /* InteropNSArray<KeyType> */IntPtr NSDictionary_allKeys(HandleRef self);

        [DllImport("__Internal")]
        internal static extern /* InteropNSArray<ObjectType> */IntPtr NSDictionary_allValues(HandleRef self);

        [DllImport("__Internal")]
        internal static extern /* ObjectType */IntPtr NSDictionary_objectForKey(
            HandleRef self, /* KeyType */IntPtr aKey);

        [DllImport("__Internal")]
        internal static extern /* ObjectType */IntPtr NSDictionary_valueForKey(
            HandleRef self, /* InteropNSString */IntPtr aKey);
    }
}
#endif