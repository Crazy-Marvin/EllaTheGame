using System;

namespace EasyMobile
{
    public class BannerAdSize
    {
        public bool IsSmartBanner { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public static readonly BannerAdSize Banner = new BannerAdSize(320, 50);
        public static readonly BannerAdSize MediumRectangle = new BannerAdSize(300, 250);
        public static readonly BannerAdSize IABBanner = new BannerAdSize(468, 60);
        public static readonly BannerAdSize Leaderboard = new BannerAdSize(728, 90);
        public static readonly BannerAdSize SmartBanner = new BannerAdSize(true);

        public BannerAdSize(int width, int height)
        {
            IsSmartBanner = false;
            this.Width = width;
            this.Height = height;
        }

        private BannerAdSize(bool isSmartBanner)
        {
            this.IsSmartBanner = isSmartBanner;
            this.Width = 0;
            this.Height = 0;
        }

        public override bool Equals(object obj)
        {
            var other = obj as BannerAdSize;

            if (other == null)
            {
                return false;
            }

            if (this.IsSmartBanner)
                return other.IsSmartBanner;
            else
                return !other.IsSmartBanner && this.Width == other.Width && this.Height == other.Height;
        }

        public static bool operator ==(BannerAdSize a, BannerAdSize b)
        {
            if (ReferenceEquals(a, null))
                return ReferenceEquals(b, null);

            return a.Equals(b);
        }

        public static bool operator !=(BannerAdSize a, BannerAdSize b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + IsSmartBanner.GetHashCode();
                hash = hash * 23 + Width.GetHashCode();
                hash = hash * 23 + Height.GetHashCode();
                return hash;
            }
        }
    }
}

