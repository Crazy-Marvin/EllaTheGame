using System;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.Internal;

namespace EasyMobile
{
    [Serializable]
    public class IronSourceSettings : AdNetworkSettings
    {
        /// <summary>
        /// Gets or sets the IronSource app identifier.
        /// </summary>
        public AdId AppId
        {
            get { return mAppId; }
            set { mAppId = value; }
        }

        /// <summary>
        /// Enables or disables advanced settings.
        /// </summary>
        public bool UseAdvancedSetting
        {
            get { return mUseAdvancedSetting; }
            set { mUseAdvancedSetting = value; }
        }

        /// <summary>
        /// Gets or sets the ad segments.
        /// </summary>
        public SegmentSettings Segments
        {
            get { return mSegments; }
            set { mSegments = value; }
        }

        [SerializeField]
        private AdId mAppId;
        [SerializeField]
        private bool mUseAdvancedSetting;
        [SerializeField]
        private SegmentSettings mSegments;

        [Serializable]
        public class SegmentSettings
        {
            public int age;
            public string gender = null;
            public int level;
            public bool isPaying;
            public long userCreationDate;
            public double iapt;
            public string segmentName = null;
            public StringStringSerializableDictionary customParams;

#if EM_IRONSOURCE
            public IronSourceSegment ToIronSourceSegment()
            {
                IronSourceSegment segment = new IronSourceSegment
                {
                    age = this.age,
                    gender = this.gender,
                    level = this.level,
                    isPaying = isPaying ? 1 : 0,
                    userCreationDate = this.userCreationDate,
                    iapt = this.iapt,
                    segmentName = this.segmentName,
                };

                if (customParams != null)
                {
                    foreach (var param in customParams)
                    {
                        segment.setCustom(param.Key, param.Value);
                    }
                }

                return segment;
            }
#endif
        }

        public enum IronSourceBannerType
        {
            /// <summary>
            /// 50 X screen width.
            /// Supports: Admob, AppLovin, Facebook, InMobi.
            /// </summary>
            Banner,

            /// <summary>
            /// 90 X screen width.
            /// Supports: Admob, Facebook.
            /// </summary>
            LargeBanner,

            /// <summary>
            /// 250 X screen width.
            /// Supports: Admob, AppLovin, Facebook, InMobi.
            /// </summary>
            RectangleBanner,

            /// <summary>
            /// 50 (screen height ≤ 720) X screen width, 90 (screen height > 720) X screen width.
            /// Supports: Admob, AppLovin, Facebook, InMobi.
            /// </summary>
            SmartBanner,
        }
    }
}
