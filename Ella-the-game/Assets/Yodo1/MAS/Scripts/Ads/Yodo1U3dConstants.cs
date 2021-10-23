using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Yodo1U3dConstants
{
    public const string LIB_NAME = "__Internal";//对外扩展接口的库名

    public enum Yodo1AdsType
    {
        Yodo1AdsTypeNone = -1,
        Yodo1AdsTypeBanner = 1001,//banner ad
        Yodo1AdsTypeInterstitial = 1002,//intertitial ad
        Yodo1AdsTypeVideo = 1003,//reward video ad
        Yodo1AdsTypeRewardGame = 1007,//reward game
    };
    //banner ad align
    public enum BannerAdAlign
    {
        BannerAdAlignLeft = 1 << 0,
        BannerAdAlignHorizontalCenter = 1 << 1,
        BannerAdAlignRight = 1 << 2,
        BannerAdAlignTop = 1 << 3,
        BannerAdAlignVerticalCenter = 1 << 4,
        BannerAdAlignBotton = 1 << 5,
    };

    /// <summary>
    /// Ad event.
    /// </summary>
	public enum AdEvent
    {
        /// <summary>
        /// The ad has been closed.
        /// </summary>
		AdEventClose = 0,
        /// <summary>
        /// Reward video ad has been played completely and rewards will be given to player.
        /// </summary>
		AdEventFinish = 1,
        /// <summary>
        /// Click event has been triggered.
        /// </summary>
		AdEventClick = 2,
        /// <summary>
        /// The ad has been cached successfully. 
        /// </summary>
        [Obsolete("AdEventLoaded is obsolete", true)]
        AdEventLoaded = 3,
        /// <summary>
        /// The ad has been displayed.
        /// </summary>
		AdEventShowSuccess = 4,
        /// <summary>
        /// The ad display fail.
        /// </summary>
		AdEventShowFail = 5,
        /// <summary>
        /// The ad event purchase.
        /// </summary>
        [Obsolete("AdEventPurchase is obsolete", true)]
        AdEventPurchase = 6,
        /// <summary>
        /// The ad cache fail.
        /// </summary>
        [Obsolete("AdEventLoadFail is obsolete", true)]
        AdEventLoadFail = -1,
    };
}