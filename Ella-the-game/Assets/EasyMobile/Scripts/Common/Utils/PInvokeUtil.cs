using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace EasyMobile.Internal
{
    internal static class PInvokeUtil
    {
        internal static HandleRef CheckNonNull(HandleRef reference)
        {
            if (IsNull(reference))
            {
                throw new System.InvalidOperationException();
            }

            return reference;
        }

        internal static bool IsNull(HandleRef reference)
        {
            return IsNull(HandleRef.ToIntPtr(reference));
        }

        internal static bool IsNull(IntPtr pointer)
        {
            return pointer.Equals(IntPtr.Zero);
        }

        internal static bool IsNotNull(HandleRef reference)
        {
            return !IsNull(reference);
        }

        internal static bool IsNotNull(IntPtr pointer)
        {
            return !IsNull(pointer);
        }

        internal delegate int NativeToManagedArray<T>([In, Out] T[] buffer,int length);

        internal static T[] GetNativeArray<T>(NativeToManagedArray<T> method)
        {
            int arraySize = method(null, 0);
        
            if (arraySize <= 0)
            {
                return new T[0];
            }
        
            T[] array = new T[arraySize];
            method(array, arraySize);
            return array;
        }

        internal static string GetNativeString(NativeToManagedArray<byte> nativeToManagedCharArray)
        {
            string str = null;
            byte[] charArr = GetNativeArray(nativeToManagedCharArray);

            if (charArr != null && charArr.Length > 0)
            {
                try
                {
                    str = Encoding.UTF8.GetString(charArr);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception creating string from char array: " + e);
                }
            }

            return str;
        }

        internal static byte[] CopyNativeByteArray(IntPtr data, int dataLength)
        {
            if (dataLength == 0 || data.Equals(IntPtr.Zero))
            {
                return null;
            }

            byte[] convertedData = new byte[dataLength];
            Marshal.Copy(data, convertedData, 0, dataLength);

            return convertedData;
        }

        internal static IEnumerable<T> ToEnumerable<T>(int size, Func<int, T> getElement)
        {
            for (int i = 0; i < size; i++)
            {
                yield return getElement(i);
            }
        }
    }
}

