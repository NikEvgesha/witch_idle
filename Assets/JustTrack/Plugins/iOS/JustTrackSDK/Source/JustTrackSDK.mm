#include "JustTrackSDKUnity.h"
// MARK: Other bridging headers
// here we would insert the bridging headers from other unity packages (like NiceVibrations)
// MARK: Swift bridging header
#import <UnityFramework/UnityFramework-Swift.h>

static char *_justtrack_sdk_rp_alloc_string(NSString *s) {
    if (s == NULL) {
        return NULL;
    }

    return strdup([s UTF8String]);
}

extern "C" {
    void _justtrack_sdk_rp_free_string(char *str) {
        free(str);
    }

    void _justtrack_sdk_rp_notify_load_start() {
        [[NativeBridge shared] notifyLoadStart];
    }

    void _justtrack_sdk_rp_notify_load_done() {
        [[NativeBridge shared] notifyLoadDone];
    }

    char* _justtrack_sdk_rp_init(const char *apiToken, const char *trackingId, const char *trackingProvider, const char *customUserId, int inactivityTimeFrameHours, int reAttributionTimeFrameDays, int reFetchAttributionDelaySeconds, int attributionRetryDelaySeconds, bool automaticInAppPurchaseTracking) {
        NSString *nsApiToken = [NSString stringWithUTF8String:apiToken];
        NSString *nsTrackingId = [NSString stringWithUTF8String:trackingId];
        NSString *nsTrackingProvider = [NSString stringWithUTF8String:trackingProvider];
        NSString *nsCustomUserId = [NSString stringWithUTF8String:customUserId];

        NSString *userId = [[NativeBridge shared] initSdkWithApiToken: nsApiToken
                                        trackingId: nsTrackingId
                                  trackingProvider: nsTrackingProvider
                                      customUserId: nsCustomUserId
                          inactivityTimeFrameHours: (NSInteger) inactivityTimeFrameHours
                        reAttributionTimeFrameDays: (NSInteger) reAttributionTimeFrameDays
                  reFetchReAttributionDelaySeconds: (NSInteger) reFetchAttributionDelaySeconds
                      attributionRetryDelaySeconds: (NSInteger) attributionRetryDelaySeconds
                    automaticInAppPurchaseTracking: automaticInAppPurchaseTracking];

        return _justtrack_sdk_rp_alloc_string(userId);
    }

    void _justtrack_sdk_rp_get_retargeting_parameters() {
        [[NativeBridge shared] getRetargetingParameters];
    }

    char *_justtrack_sdk_rp_get_preliminary_retargeting_parameters() {
        NSString *s = [[NativeBridge shared] getPreliminaryRetargetingParameters];

        return _justtrack_sdk_rp_alloc_string(s);
    }

    void _justtrack_sdk_rp_set_custom_user_id(const char *customUserId) {
        NSString *nsCustomUserId = [NSString stringWithUTF8String:customUserId];

        [[NativeBridge shared] setWithCustomUserId: nsCustomUserId];
    }

    void _justtrack_sdk_rp_set_automatic_in_app_purchase_tracking(bool automaticInAppPurchaseTracking) {
        [[NativeBridge shared] setWithAutomaticInAppPurchaseTracking: automaticInAppPurchaseTracking];
    }

    void _justtrack_sdk_rp_set_firebase_app_instance_id(const char *firebaseAppInstanceId) {
        NSString *nsFirebaseAppInstanceId = [NSString stringWithUTF8String:firebaseAppInstanceId];

        [[NativeBridge shared] setWithFirebaseAppInstanceId:nsFirebaseAppInstanceId];
    }

    void _justtrack_sdk_rp_create_affiliate_link(const char *channel) {
        NSString *nsChannel = channel != NULL ? [NSString stringWithUTF8String:channel] : NULL;

        [[NativeBridge shared] createAffiliateLinkWithChannel: nsChannel];
    }

    void _justtrack_sdk_rp_publish_event(const char *name, const char *category, const char *element, const char *action, const char *dimensions) {
        NSString *nsName = [NSString stringWithUTF8String:name];
        NSString *nsCategory = [NSString stringWithUTF8String:category];
        NSString *nsElement = [NSString stringWithUTF8String:element];
        NSString *nsAction = [NSString stringWithUTF8String:action];
        NSString *nsDimensions = [NSString stringWithUTF8String:dimensions];

        [[NativeBridge shared] publishEventWithName: nsName
                                           category: nsCategory
                                            element: nsElement
                                             action: nsAction
                                         dimensions: nsDimensions];
    }

    void _justtrack_sdk_rp_publish_unit_event(const char *name, const char *category, const char *element, const char *action, const char *dimensions, double value, const char *unit) {
        NSString *nsName = [NSString stringWithUTF8String:name];
        NSString *nsCategory = [NSString stringWithUTF8String:category];
        NSString *nsElement = [NSString stringWithUTF8String:element];
        NSString *nsAction = [NSString stringWithUTF8String:action];
        NSString *nsDimensions = [NSString stringWithUTF8String:dimensions];
        NSString *nsUnit = [NSString stringWithUTF8String:unit];

        [[NativeBridge shared] publishEventWithName: nsName
                                           category: nsCategory
                                            element: nsElement
                                             action: nsAction
                                         dimensions: nsDimensions
                                              value: value
                                               unit: nsUnit];
    }

    void _justtrack_sdk_rp_publish_money_event(const char *name, const char *category, const char *element, const char *action, const char *dimensions, double value, const char *currency) {
        NSString *nsName = [NSString stringWithUTF8String:name];
        NSString *nsCategory = [NSString stringWithUTF8String:category];
        NSString *nsElement = [NSString stringWithUTF8String:element];
        NSString *nsAction = [NSString stringWithUTF8String:action];
        NSString *nsDimensions = [NSString stringWithUTF8String:dimensions];
        NSString *nsCurrency = [NSString stringWithUTF8String:currency];

        [[NativeBridge shared] publishEventWithName: nsName
                                           category: nsCategory
                                            element: nsElement
                                             action: nsAction
                                         dimensions: nsDimensions
                                              value: value
                                           currency: nsCurrency];
    }

    void _justtrack_sdk_rp_integrate_with_ad_colony() {
        [[NativeBridge shared] integrateWithAdColony];
    }

    void _justtrack_sdk_rp_integrate_with_app_lovin() {
        [[NativeBridge shared] integrateWithAppLovin];
    }

    void _justtrack_sdk_rp_integrate_with_chartboost() {
        [[NativeBridge shared] integrateWithChartboost];
    }

    void _justtrack_sdk_rp_integrate_with_unity_ads() {
        [[NativeBridge shared] integrateWithUnityAds];
    }

    void _justtrack_sdk_rp_integrate_with_firebase() {
        [[NativeBridge shared] integrateWithFirebase];
    }

    bool _justtrack_sdk_rp_forward_ad_impression(const char *adFormat, const char *adSdkName, const char *adNetwork, const char *placement, const char *abTesting, const char *segmentName, const char *instanceName, const char *bundleId, double revenue, const char *currency) {
        NSString *nsAdFormat = adFormat != NULL ? [NSString stringWithUTF8String:adFormat] : NULL;
        NSString *nsAdSdkName = adSdkName != NULL ? [NSString stringWithUTF8String:adSdkName] : NULL;
        NSString *nsAdNetwork = adNetwork != NULL ? [NSString stringWithUTF8String:adNetwork] : NULL;
        NSString *nsPlacement = placement != NULL ? [NSString stringWithUTF8String:placement] : NULL;
        NSString *nsABTesting = abTesting != NULL ? [NSString stringWithUTF8String:abTesting] : NULL;
        NSString *nsSegmentName = segmentName != NULL ? [NSString stringWithUTF8String:segmentName] : NULL;
        NSString *nsInstanceName = instanceName != NULL ? [NSString stringWithUTF8String:instanceName] : NULL;
        NSString *nsBundleId = bundleId != NULL ? [NSString stringWithUTF8String:bundleId] : NULL;
        NSNumber *nsRevenue = [NSNumber numberWithDouble:revenue];
        NSString *nsCurrency = currency != NULL ? [NSString stringWithUTF8String:currency] : NULL;

        return [[NativeBridge shared] forwardAdImpressionWithAdFormat: nsAdFormat
                                                            adSdkName: nsAdSdkName
                                                            adNetwork: nsAdNetwork
                                                            placement: nsPlacement
                                                            abTesting: nsABTesting
                                                          segmentName: nsSegmentName
                                                         instanceName: nsInstanceName
                                                             bundleId: nsBundleId
                                                              revenue: nsRevenue
                                                             currency: nsCurrency];
    }

    void _justtrack_sdk_rp_log_debug(const char *message) {
        NSString *nsMessage = [NSString stringWithUTF8String:message];

        [[NativeBridge shared] logDebugWithMessage:nsMessage];
    }

    void _justtrack_sdk_rp_log_info(const char *message) {
        NSString *nsMessage = [NSString stringWithUTF8String:message];

        [[NativeBridge shared] logInfoWithMessage:nsMessage];
    }

    void _justtrack_sdk_rp_log_warning(const char *message) {
        NSString *nsMessage = [NSString stringWithUTF8String:message];

        [[NativeBridge shared] logWarningWithMessage:nsMessage];
    }

    void _justtrack_sdk_rp_log_error(const char *message) {
        NSString *nsMessage = [NSString stringWithUTF8String:message];

        [[NativeBridge shared] logErrorWithMessage:nsMessage];
    }

    char *_justtrack_sdk_rp_get_advertiser_id_info() {
        NSString *s = [[NativeBridge shared] getAdvertiserIdInfo];

        return _justtrack_sdk_rp_alloc_string(s);
    }

    int _justtrack_sdk_rp_get_test_group_id() {
        return (int) [[NativeBridge shared] getTestGroupId];
    }

    void _justtrack_sdk_rp_request_tracking_authorization() {
        return [[NativeBridge shared] requestTrackingAuthorization];
    }
}
