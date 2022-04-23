using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace MonKey.Settings.Internal
{
    public abstract class EditorSingleton<T> : ScriptableObject
        where T : ScriptableObject, IMonKeySingleton
    {

        public static T Instance
        {
            get
            {
                if (!instance)
                    FindInstance();

                try
                {
                    if (!instance)
                    return CreateInstance<T>();
                }
                catch (Exception)
                {
                    return null;
                }
                return instance;
            }
        }

        private static T instance;

        // ReSharper disable once StaticMemberInGenericType
        protected static string SessionID = "";

        // ReSharper disable once StaticMemberInGenericType
        protected static bool DebugLog = false;
      
        public static void FindInstance()
        {
            if (instance)
                return;

            if (DebugLog)
            {
                Debug.Log("Looking for instance of " + typeof(T));
            }

            try
            {
                int id = SessionState.GetInt(SessionID, -1);
                if (id == -1)
                {
                    CreateNewInstance();
                }
                else
                {
                    if (DebugLog)
                    {
                        Debug.Log("Instance found! Hurray! " + typeof(T));
                        Debug.Log(EditorUtility.InstanceIDToObject(id));
                    }

                    instance = (T)EditorUtility.InstanceIDToObject(id);
                    if (!instance)
                        CreateNewInstance();
                }
            }
            catch (Exception )
            {
            }

        }

        private static void CreateNewInstance()
        {
            if (DebugLog)
            {
                Debug.Log("Creating new instance of " + typeof(T));
            }

            instance = CreateInstance<T>();
            SessionState.SetInt(SessionID, instance.GetInstanceID());
            instance.PostInstanceCreation();
            DontDestroyOnLoad(instance);
            instance.hideFlags = HideFlags.HideAndDontSave;
        }
    }
}