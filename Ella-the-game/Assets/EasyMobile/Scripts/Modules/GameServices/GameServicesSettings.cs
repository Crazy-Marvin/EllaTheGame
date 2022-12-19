using UnityEngine;
using System.Collections;

namespace EasyMobile
{
    [System.Serializable]
    public partial class GameServicesSettings
    {
        /// <summary>
        /// Whether the Game Services module should initialize itself automatically.
        /// </summary>
        /// <value><c>true</c> if is auto init; otherwise, <c>false</c>.</value>
        public bool IsAutoInit { get { return mAutoInit; } set { mAutoInit = value; } }

        /// <summary>
        /// Whether the Game Services module should initialize itself when the user has logged out in the previous session.
        /// </summary>
        /// <value><c>true</c> if is auto init; otherwise, <c>false</c>.</value>
        public bool IsAutoInitAfterUserLogout { get { return mAutoInitAfterUserLogout; } set { mAutoInitAfterUserLogout = value; } }

        /// <summary>
        /// The delay (in seconds) after the Easy Mobile runtime has been initialized that this module initializes itself automatically.
        /// </summary>
        /// <value>The auto init delay.</value>
        public float AutoInitDelay { get { return mAutoInitDelay; } set { mAutoInitDelay = value; } }

        /// <summary>
        /// The total number of times the login popup can be displayed if the user has not logged in.
        /// When this value is reached, the init process will stop thus not showing the login popup anymore (avoid annoying the user).
        /// Set to 0 to ignore this limit. Note that this setting is applied to auto initialization and <see cref="GameServices.ManagedInit"/> only.
        /// </summary>
        /// <value>The android max login requests.</value>
        public int AndroidMaxLoginRequests { get { return mAndroidMaxLoginRequests; } }

        /// <summary>
        /// Whether to show debug log from the Google Play Games plugin.
        /// </summary>
        /// <value><c>true</c> if ggps debug log enabled; otherwise, <c>false</c>.</value>
        public bool GgpsDebugLogEnabled { get { return mGpgsDebugLogEnabled; } set { mGpgsDebugLogEnabled = value; } }

        /// <summary>
        /// Gets or sets the GPGS popup gravity.
        /// </summary>
        /// <value>The gpgs popup gravity.</value>
        public GpgsGravity GpgsPopupGravity { get { return mGpgsPopupGravity; } set { mGpgsPopupGravity = value; } }

        /// <summary>
        /// [Google Play Games] Whether to request a server authentication code during initialization.
        /// </summary>
        public bool GpgsShouldRequestServerAuthCode { get { return mGpgsShouldRequestServerAuthCode; } set { mGpgsShouldRequestServerAuthCode = value; } }

        /// <summary>
        /// [Google Play Games] Whether to force refresh while requesting a server authentication code during initialization.
        /// </summary>
        public bool GpgsForceRefreshServerAuthCode { get { return mGpgsForceRefreshServerAuthCode; } set { mGpgsForceRefreshServerAuthCode = value; } }

        /// <summary>
        /// [Google Play Games] The OAuth scopes to be added during initialization.
        /// </summary>
        public string[] GpgsOauthScopes { get { return mGpgsOauthScopes; } set { mGpgsOauthScopes = value; } }

        /// <summary>
        /// Gets or sets the leaderboards.
        /// </summary>
        /// <value>The leaderboards.</value>
        public Leaderboard[] Leaderboards { get { return mLeaderboards; } set { mLeaderboards = value; } }

        /// <summary>
        /// Gets or sets the achievements.
        /// </summary>
        /// <value>The achievements.</value>
        public Achievement[] Achievements { get { return mAchievements; } set { mAchievements = value; } }

        // Auto-init config
        [SerializeField]
        private bool mAutoInit = true;
        [SerializeField]
        private bool mAutoInitAfterUserLogout = false;
        [SerializeField]
        [Range(0, 120)]
        private float mAutoInitDelay = 0;
        [SerializeField]
        [Range(0, 30)]
        private int mAndroidMaxLoginRequests = 0;

        // GPGS setup.
        [SerializeField]
        private bool mGpgsDebugLogEnabled = false;
        [SerializeField]
        private GpgsGravity mGpgsPopupGravity = GpgsGravity.Top;
        [SerializeField]
        private bool mGpgsShouldRequestServerAuthCode = false;
        [SerializeField]
        private bool mGpgsForceRefreshServerAuthCode = false;
        [SerializeField]
        private string[] mGpgsOauthScopes = null;

        // Leaderboards & Achievements
        [SerializeField]
        private Leaderboard[] mLeaderboards = null;
        [SerializeField]
        private Achievement[] mAchievements = null;

        // GPGS setup resources.
        // These fields are only used in our editor, hence the warning suppression.
#pragma warning disable 0414
        [SerializeField]
        private string mAndroidXmlResources = string.Empty;
#pragma warning restore 0414

        public enum GpgsGravity
        {
            Top,
            Bottom,
            Left,
            Right,
            CenterHorizontal
        }
    }
}

