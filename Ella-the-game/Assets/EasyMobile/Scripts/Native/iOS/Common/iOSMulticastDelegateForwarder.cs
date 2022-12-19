#if UNITY_IOS
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using EasyMobile.Internal;
using EasyMobile.iOS;
using EasyMobile.iOS.Foundation;

namespace EasyMobile.Internal.iOS
{
    internal abstract class iOSMulticastDelegateForwarder<T> : iOSObjectProxy where T : class
    {
        protected List<T> mListeners = new List<T>();

        protected iOSMulticastDelegateForwarder(IntPtr selfPtr)
            : base(selfPtr)
        {
        }

        public virtual int ListenerCount
        {
            get { return mListeners.Count; }
        }

        public virtual void RegisterListener(T listener)
        {
            if (listener != null && !mListeners.Contains(listener))
                mListeners.Add(listener);
        }

        public virtual bool HasListener(T listener)
        {
            return mListeners.Contains(listener);
        }

        public virtual void UnregisterListener(T listener)
        {
            mListeners.Remove(listener);
        }

        public virtual void UnregisterAllListeners()
        {
            mListeners.Clear();
        }

        protected virtual void InvokeOnAllListeners(Action<T> action)
        {
            if (action != null)
            {
                for (int i = 0; i < mListeners.Count; i++)
                    action(mListeners[i]);
            }
        }
    }
}
#endif