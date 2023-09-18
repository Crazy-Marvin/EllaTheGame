using UnityEngine;
using System.Collections;

namespace EasyMobile
{
    [System.Serializable]
    public class RatingRequestSettings
    {
        /// <summary>
        /// Gets or sets the content of the default rating dialog.
        /// </summary>
        /// <value>The default content of the rating dialog.</value>
        public RatingDialogContent DefaultRatingDialogContent
        {
            get { return mDefaultRatingDialogContent; }
            set { mDefaultRatingDialogContent = value; }
        }

        /// <summary>
        /// Gets or sets the minimum rating stars accepted.
        /// </summary>
        /// <value>The minimum accepted stars.</value>
        public uint MinimumAcceptedStars
        {
            get { return mMinimumAcceptedStars; }
            set { mMinimumAcceptedStars = value; }
        }

        /// <summary>
        /// Gets or sets the support email to receive feedback (used on Android popup only).
        /// </summary>
        /// <value>The support email.</value>
        public string SupportEmail
        {
            get { return mSupportEmail; }
            set { mSupportEmail = value; }
        }

        /// <summary>
        /// Gets or sets the iOS app identifier on the Apple App Store.
        /// </summary>
        /// <value>The ios app identifier.</value>
        public string IosAppId
        {
            get { return mIosAppId; }
            set { mIosAppId = value; }
        }

        /// <summary>
        /// Gets or sets maximum number of requests allowed each year,
        /// note that on iOS 10.3+ this value is governed by the OS and is always set to 3.
        /// </summary>
        /// <value>The annual cap.</value>
        public uint AnnualCap
        {
            get { return mAnnualCap; }
            set { mAnnualCap = value; }
        }

        /// <summary>
        /// Gets or sets the waiting time (in days) after app installation before the first rating request can be made.
        /// </summary>
        /// <value>The delay after installation.</value>
        public uint DelayAfterInstallation
        {
            get { return mDelayAfterInstallation; }
            set { mDelayAfterInstallation = value; }
        }

        /// <summary>
        /// Gets or sets the mininum interval required (in days) between two consecutive requests.
        /// </summary>
        /// <value>The cooling off period.</value>
        public uint CoolingOffPeriod
        {
            get { return mCoolingOffPeriod; }
            set { mCoolingOffPeriod = value; }
        }

        /// <summary>
        /// Whether to ignore all display constraints so the rating popup can be shown every time in development builds, 
        /// unless it was disabled before.
        /// </summary>
        /// <value><c>true</c> if ignore constraints in development; otherwise, <c>false</c>.</value>
        public bool IgnoreConstraintsInDevelopment
        {
            get { return mIgnoreContraintsInDevelopment; }
            set { mIgnoreContraintsInDevelopment = value; }
        }

        // Appearance
        [SerializeField]
        private RatingDialogContent mDefaultRatingDialogContent = RatingDialogContent.Default;

        // Behaviour
        [SerializeField]
        [Range(0, 5)]
        private uint mMinimumAcceptedStars = 4;
        [SerializeField]
        private string mSupportEmail = null;
        [SerializeField]
        private string mIosAppId = null;

        // Display constraints
        [SerializeField]
        [Range(3, 100)]
        private uint mAnnualCap = 12;
        [SerializeField]
        [Range(0, 365)]
        private uint mDelayAfterInstallation = 10;
        [SerializeField]
        [Range(0, 365)]
        private uint mCoolingOffPeriod = 10;
        [SerializeField]
        private bool mIgnoreContraintsInDevelopment = false;
    }
}

