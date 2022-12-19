using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EasyMobile.Editor
{
    public class EM_ProjectSettings
    {
        private static EM_ProjectSettings _instance = null;

        public static EM_ProjectSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EM_ProjectSettings();
                }

                return _instance;
            }
        }

        private const string _filePath = EM_Constants.PluginSettingsFilePath;
        private bool _isDirty = false;
        private Dictionary<string, string> _settings = new Dictionary<string, string>();

        private EM_ProjectSettings()
        {
            if (FileIO.FileExists(_filePath))
            {
                string[] lines = FileIO.ReadAllLines(_filePath);

                foreach (string l in lines)
                {
                    string[] p = l.Split(new char[] { '=' }, 2);
                    if (p.Length >= 2)
                    {
                        _settings[p[0].Trim()] = p[1].Trim();
                    }
                }
            }
        }

        public string Get(string key, string defaultValue)
        {
            if (_settings.ContainsKey(key))
            {
#if UNITY_2018_3_OR_NEWER
                return UnityEngine.Networking.UnityWebRequest.UnEscapeURL(_settings[key]);
#else
                return WWW.UnEscapeURL(_settings[key]);
#endif
            }
            else
            {
                return defaultValue;
            }
        }

        public string Get(string key)
        {
            return Get(key, string.Empty);
        }

        public bool GetBool(string key, bool defaultValue)
        {
            return Get(key, defaultValue ? "true" : "false").Equals("true");
        }

        public bool GetBool(string key)
        {
            return Get(key, "false").Equals("true");
        }

        public int GetInt(string key, int defaultValue)
        {
            int ret;
            return Int32.TryParse(Get(key), out ret) ? ret : defaultValue;
        }

        public void Set(string key, int val, bool save = true)
        {
            Set(key, val.ToString(), save);
        }

        public void Set(string key, bool val, bool save = true)
        {
            Set(key, val ? "true" : "false", save);
        }

        public void Set(string key, string val, bool save = true)
        {
#if UNITY_2018_3_OR_NEWER
            string escaped = UnityEngine.Networking.UnityWebRequest.EscapeURL(val);
#else
            string escaped = WWW.EscapeURL(val);
#endif
            _settings[key] = escaped;
            _isDirty = true;

            if (save)
                Save();
        }

        public void Save()
        {
            // Stop if there's nothing to save
            if (!_isDirty)
            {
                return;
            }

            List<string> newLines = new List<string>();

            foreach (string key in _settings.Keys)
            {
                newLines.Add(key + "=" + _settings[key]);
            }

            FileIO.WriteAllLines(_filePath, newLines.ToArray());
            _isDirty = false;
        }
    }
}
