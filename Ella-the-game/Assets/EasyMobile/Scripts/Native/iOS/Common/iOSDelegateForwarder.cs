#if UNITY_IOS
using UnityEngine;
using System;
using EasyMobile.iOS;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.Internal.iOS
{
    internal abstract class iOSDelegateForwarder<T> : iOSObjectProxy where T : class
    {
        public T Listener { get; set; }

        protected iOSDelegateForwarder(IntPtr selfPtr)
            : base(selfPtr)
        {
        }

        protected virtual void InvokeOnListener(Action<T> action)
        {
            if (action != null && Listener != null)
                action(Listener);
        }

        protected virtual U InvokeOnListener<U>(Func<T, U> action)
        {
            if (action != null && Listener != null)
                return action(Listener);
            else
                return default(U);
        }
    }
}
#endif