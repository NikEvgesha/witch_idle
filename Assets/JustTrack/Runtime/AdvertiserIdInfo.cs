using UnityEngine;

namespace JustTrack {
    /**
    * The result of reading an advertiser id can be one of three possibilities:
    * * The advertiser id was successfully read
    * * The user limited ad tracking and the OS enforces this
    *   (late 2021: Android 12, early 2022: all Android devices; iOS <14: The user opted out, iOS 14+: the user didn't opt in)
    * * Reading the advertiser id failed
    *
    * In the first case there will be an advertiser id available. In the second case, the id will be null
    * and IsLimitedAdTracking will be true. In the last case the advertiser id will also be
    * null, but ad tracking will not be reported as limited.
    */
    public class AdvertiserIdInfo {
        #if CSHARP_8_0_OR_NEWER
        private AdvertiserIdInfo(string? pAdvertiserId, bool pIsLimitedAdTracking) {
        #else
        private AdvertiserIdInfo(string pAdvertiserId, bool pIsLimitedAdTracking) {
        #endif
            this.AdvertiserId = pAdvertiserId;
            this.IsLimitedAdTracking = pIsLimitedAdTracking;
        }

        /**
        * Contains the advertiser id of the user or null if it could not be read (user limited tracking
        * and the OS enforces it or an error occurred).
        */
        #if CSHARP_8_0_OR_NEWER
        public string? AdvertiserId { get; private set; }
        #else
        public string AdvertiserId { get; private set; }
        #endif
        /**
        * Did the user limit ad tracking? This can also be reported as true while the advertiser id is
        * available. In that case the OS does not enforce the limit yet.
        */
        public bool IsLimitedAdTracking { get; private set; }

        #if UNITY_ANDROID
            internal static AdvertiserIdInfo FromAndroidObject(AndroidJavaObject pInfo) {
                return new AdvertiserIdInfo(
            #if CSHARP_8_0_OR_NEWER
                    pInfo.Call<string?>("getAdvertiserId"),
            #else
                    pInfo.Call<string>("getAdvertiserId"),
            #endif
                    pInfo.Call<bool>("isLimitedAdTracking")
                );
            }
        #endif
        #if UNITY_IOS
            #if CSHARP_8_0_OR_NEWER
            internal static AdvertiserIdInfo CreateAdvertiserIdInfo(string? pAdvertiserId, bool pIsLimitedAdTracking) {
            #else
            internal static AdvertiserIdInfo CreateAdvertiserIdInfo(string pAdvertiserId, bool pIsLimitedAdTracking) {
            #endif
                return new AdvertiserIdInfo(pAdvertiserId, pIsLimitedAdTracking);
            }
        #endif
        #if UNITY_EDITOR
            #if CSHARP_8_0_OR_NEWER
            internal static AdvertiserIdInfo CreateFakeAdvertiserIdInfo(string? pAdvertiserId, bool pIsLimitedAdTracking) {
            #else
            internal static AdvertiserIdInfo CreateFakeAdvertiserIdInfo(string pAdvertiserId, bool pIsLimitedAdTracking) {
            #endif
                return new AdvertiserIdInfo(pAdvertiserId, pIsLimitedAdTracking);
            }
        #endif
    }
}
