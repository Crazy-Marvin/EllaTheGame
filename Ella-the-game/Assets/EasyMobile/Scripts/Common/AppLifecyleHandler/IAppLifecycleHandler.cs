using UnityEngine;
using System.Collections;

namespace EasyMobile.Internal
{
    public interface IAppLifecycleHandler
    {
        void OnApplicationFocus(bool isFocus);

        void OnApplicationPause(bool isPaused);

        void OnApplicationQuit();
    }
}