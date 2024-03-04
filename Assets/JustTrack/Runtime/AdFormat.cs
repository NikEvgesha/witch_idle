using System;

namespace JustTrack {
    public enum AdFormat {
        Banner,
        Interstitial,
        Rewarded,
        RewardedInterstitial,
        Native,
        AppOpen,
    }

    internal static class AdFormatInternalConversation {
        internal static string ToInternalString(AdFormat adFormat) {
            switch (adFormat) {
                case AdFormat.Banner:
                    return "banner";
                case AdFormat.Interstitial:
                    return "interstitial";
                case AdFormat.Rewarded:
                    return "rewarded";
                case AdFormat.RewardedInterstitial:
                    return "rewarded_interstitial";
                case AdFormat.Native:
                    return "native";
                case AdFormat.AppOpen:
                    return "app_open";
                default:
                    return "";
            }
        }
    }

    #if JUSTTRACK_IRONSOURCE_INTEGRATION
        internal static class AdFormatFromIronsourceConversion {
            internal static AdFormat? ToAdFormat(string adFormat) {
                switch (adFormat) {
                case "banner":
                    return AdFormat.Banner;
                case "interstitial":
                    return AdFormat.Interstitial;
                case "rewarded_video":
                    return AdFormat.Rewarded;
                }

                return null;
            }
        }
    #endif
}
