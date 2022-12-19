using UnityEngine;
using System.Collections;

namespace EasyMobile
{
    [System.Serializable]
    public class GameServicesItem
    {
        public string Name { get { return _name; } }

        public string IOSId { get { return _iosId; } }

        public string AndroidId { get { return _androidId; } }

        public string Id
        {
            get
            {
                #if UNITY_IOS
                return _iosId;
                #elif UNITY_ANDROID
                return _androidId;
                #else
                return null;
                #endif
            }
        }

        [SerializeField]
        string _name;
        [SerializeField]
        string _iosId;
        [SerializeField]
        string _androidId;

        public GameServicesItem(string name, string iosId, string androidId)
        {
            this._name = name;
            this._iosId = iosId;
            this._androidId = androidId;
        }
    }

    [System.Serializable]
    public class Leaderboard : GameServicesItem
    {
        public Leaderboard(string name, string iosId, string androidId)
            : base(name, iosId, androidId)
        {
        }
    }

    [System.Serializable]
    public class Achievement : GameServicesItem
    {
        public Achievement(string name, string iosId, string androidId)
            : base(name, iosId, androidId)
        {
        }
    }
}
