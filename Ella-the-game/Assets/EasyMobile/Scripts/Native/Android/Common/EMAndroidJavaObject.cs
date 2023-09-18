#if UNITY_ANDROID
using System;
using UnityEngine;

namespace EasyMobile.Android
{
    /// <summary>
    /// Base class for all Android native binding classes.
    /// Note that this class can't be sent directly to Android, use <see cref="NativeObject"/> instead.
    /// </summary>
    internal class EMAndroidJavaObject : IDisposable
    {
        private readonly AndroidJavaObject nativeObj = null;

        /// <summary>
        /// Use this when passing object to Android.
        /// </summary>
        public AndroidJavaObject NativeObject
        {
            get { return nativeObj; }
        }

        /// <summary>
        /// Create an instance of native object with its name.
        /// </summary>
        /// <param name="className">Specifies the Java class name.</param>
        /// <param name="args">An array of parameters passed to the constructor.</param>
        public EMAndroidJavaObject(string className, params object[] args)
        {
            nativeObj = new AndroidJavaObject(className, args);
        }

        /// <summary>
        /// Create a bridge to existing native object.
        /// </summary>
        /// <param name="nativeObj">Native Android object.</param>
        public EMAndroidJavaObject(AndroidJavaObject nativeObj)
        {
            this.nativeObj = nativeObj;
        }

        /// <summary>
        /// <see cref="IDisposable"/> callback.
        /// </summary>
        public void Dispose()
        {
            nativeObj.Dispose();
        }

        /// <summary>
        /// Calls a Java method on an object (non-static).
        /// </summary>
        /// <param name="methodName">Specifies which method to call.</param>
        /// <param name="args">An array of parameters passed to the method.</param>
        public void Call(string methodName, params object[] args)
        {
            nativeObj.Call(methodName, args);
        }

        /// <summary>
        /// Calls a Java method on an object (non-static) with a return value.
        /// </summary>
        /// <param name="methodName">Specifies which method to call.</param>
        /// <param name="args">An array of parameters passed to the method.</param>
        /// <typeparam name="T">Return type.</typeparam>
        public T Call<T>(string methodName, params object[] args)
        {
            return nativeObj.Call<T>(methodName, args);
        }

        /// <summary>
        /// Call a static Java method on a class.
        /// </summary>
        /// <param name="methodName">Specifies which method to call.</param>
        /// <param name="args">An array of parameters passed to the method.</param>
        public void CallStatic(string methodName, params object[] args)
        {
            nativeObj.CallStatic(methodName, args);
        }

        /// <summary>
        /// Call a static Java method on a class with a return value.
        /// </summary>
        /// <param name="methodName">Specifies which method to call.</param>
        /// <param name="args">An array of parameters passed to the method.</param>
        /// <typeparam name="T">Return type.</typeparam>
        public T CallStatic<T>(string methodName, params object[] args)
        {
            return nativeObj.CallStatic<T>(methodName, args);
        }

        /// <summary>
        /// Call a Java getter on an object (non-static).
        /// </summary>
        /// <param name="fieldName">Specific which getter to call.</param>
        /// <typeparam name="T">Return type.</typeparam>
        public T Get<T>(string fieldName)
        {
            return nativeObj.Get<T>(fieldName);
        }

        /// <summary>
        /// Call a static Java getter on a class.
        /// </summary>
        /// <param name="fieldName">Specific which getter to call.</param>
        /// <typeparam name="T">Return type.</typeparam>
        public T GetStatic<T>(string fieldName)
        {
            return nativeObj.GetStatic<T>(fieldName);
        }

        /// <summary>
        /// Call a Java setter on an object (non-static).
        /// </summary>
        /// <param name="fieldName">Specific which setter to call.</param>
        /// <param name="val">Update value.</param>
        /// <typeparam name="T">Return type.</typeparam>
        public void Set<T>(string fieldName, T val)
        {
            nativeObj.Set(fieldName, val);
        }

        /// <summary>
        /// Call a static Java setter on a class.
        /// </summary>
        /// <param name="fieldName">Specific which setter to call.</param>
        /// <param name="val">Update value.</param>
        /// <typeparam name="T">Return type.</typeparam>
        public void SetStatic<T>(string fieldName, T val)
        {
            nativeObj.SetStatic(fieldName, val);
        }

        /// <summary>
        /// Retrieves the raw <see cref="IntPtr"/> pointer to the Java object.
        /// </summary>
        public IntPtr GetRawObject()
        {
            return nativeObj.GetRawObject();
        }

        /// <summary>
        /// Retrieves the raw <see cref="IntPtr"/> pointer to the Java class.
        /// </summary>
        public IntPtr GetRawClass()
        {
            return nativeObj.GetRawClass();
        }
    }
}
#endif