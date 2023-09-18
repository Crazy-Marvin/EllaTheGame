using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yodo1.MAS
{
    [System.Obsolete("`Yodo1U3dBannerAlign` has been deprecated. Please use `Yodo1U3dBannerAdPosition` instead.")]
    public class Yodo1U3dBannerAlign
    {
        public const int BannerLeft = 1;
        public const int BannerHorizontalCenter = 1 << 1;
        public const int BannerRight = 1 << 2;
        public const int BannerTop = 1 << 3;
        public const int BannerVerticalCenter = 1 << 4;
        public const int BannerBottom = 1 << 5;
    }

    public enum Yodo1U3dBannerAdPosition
    {
        BannerLeft = 1,
        BannerHorizontalCenter = 1 << 1,
        BannerRight = 1 << 2,
        BannerTop = 1 << 3,
        BannerVerticalCenter = 1 << 4,
        BannerBottom = 1 << 5,
    }
}