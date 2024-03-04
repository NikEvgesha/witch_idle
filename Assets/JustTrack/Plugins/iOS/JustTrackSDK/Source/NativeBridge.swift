import Foundation
import JustTrackSDK

@objc public class NativeBridge: NSObject {
    @objc public static let shared: NativeBridge = NativeBridge()
    private let bridge: UnityBridge

    private override init() {
        bridge = UnityBridge(performSendMessage: { (receiver, message) in
            UnityFramework.getInstance()?.sendMessageToGO(withName: "JustTrackSDKNativeBridgeUnity", functionName: receiver, message: message)
        })
        super.init()
    }

    @objc public func notifyLoadStart() {
        JustTrack.notifyLoadStart()
    }

    @objc public func notifyLoadDone() {
        JustTrack.notifyLoadDone()
    }

    @objc public func initSdk(apiToken: String,
                              trackingId: String,
                              trackingProvider: String,
                              customUserId: String,
                              inactivityTimeFrameHours: Int,
                              reAttributionTimeFrameDays: Int,
                              reFetchReAttributionDelaySeconds: Int,
                              attributionRetryDelaySeconds: Int,
                              automaticInAppPurchaseTracking: Bool) -> String? {
        return bridge.initSdk(apiToken: apiToken,
                       trackingId: trackingId,
                       trackingProvider: trackingProvider,
                       customUserId: customUserId,
                       inactivityTimeFrameHours: inactivityTimeFrameHours,
                       reAttributionTimeFrameDays: reAttributionTimeFrameDays,
                       reFetchReAttributionDelaySeconds: reFetchReAttributionDelaySeconds,
                       attributionRetryDelaySeconds: attributionRetryDelaySeconds,
                       automaticInAppPurchaseTracking: automaticInAppPurchaseTracking)
    }

    @objc public func getRetargetingParameters() {
        bridge.getRetargetingParameters()
    }

    @objc public func getPreliminaryRetargetingParameters() -> String {
        bridge.getPreliminaryRetargetingParameters()
    }

    @objc public func set(customUserId: String) {
        bridge.set(customUserId: customUserId)
    }

    @objc public func set(automaticInAppPurchaseTracking: Bool) {
        bridge.set(automaticInAppPurchaseTracking: automaticInAppPurchaseTracking)
    }

    @objc public func set(firebaseAppInstanceId: String) {
        bridge.set(firebaseAppInstanceId: firebaseAppInstanceId)
    }

    @objc public func createAffiliateLink(channel: String?) {
        bridge.createAffiliateLink(channel: channel)
    }

    @objc public func publishEvent(name: String, category: String, element: String, action: String, dimensions: String) {
        bridge.publishEvent(name: name, category: category, element: element, action: action, dimensions: dimensions)
    }

    @objc public func publishEvent(name: String,
                                   category: String,
                                   element: String,
                                   action: String,
                                   dimensions: String,
                                   value: Double,
                                   unit: String) {
        bridge.publishEvent(name: name, category: category, element: element, action: action, dimensions: dimensions, value: value, unit: unit)
    }

    @objc public func publishEvent(name: String,
                                   category: String,
                                   element: String,
                                   action: String,
                                   dimensions: String,
                                   value: Double,
                                   currency: String) {
        bridge.publishEvent(name: name,
                            category: category,
                            element: element,
                            action: action,
                            dimensions: dimensions,
                            value: value,
                            currency: currency)
    }

    @objc public func forwardAdImpression(adFormat: String?,
                                          adSdkName: String?,
                                          adNetwork: String?,
                                          placement: String?,
                                          abTesting: String?,
                                          segmentName: String?,
                                          instanceName: String?,
                                          bundleId: String?,
                                          revenue: NSNumber?,
                                          currency: NSString?) -> Bool {
        return bridge.forwardAdImpression(adFormat: adFormat,
                                          adSdkName: adSdkName,
                                          adNetwork: adNetwork,
                                          placement: placement,
                                          abTesting: abTesting,
                                          segmentName: segmentName,
                                          instanceName: instanceName,
                                          bundleId: bundleId,
                                          revenue: revenue,
                                          currency: currency)
    }

    @objc public func integrateWithAdColony() {
        bridge.integrateWithAdColony()
    }

    @objc public func integrateWithAppLovin() {
        bridge.integrateWithAppLovin()
    }

    @objc public func integrateWithChartboost() {
        bridge.integrateWithChartboost()
    }

    @objc public func integrateWithUnityAds() {
        bridge.integrateWithUnityAds()
    }

    @objc public func integrateWithFirebase() {
        bridge.integrateWithFirebase()
    }

    @objc public func logDebug(message: String) {
        bridge.logDebug(message: message)
    }

    @objc public func logInfo(message: String) {
        bridge.logInfo(message: message)
    }

    @objc public func logWarning(message: String) {
        bridge.logWarning(message: message)
    }

    @objc public func logError(message: String) {
        bridge.logError(message: message)
    }

    @objc public func getAdvertiserIdInfo() -> String {
        bridge.getAdvertiserIdInfo()
    }

    @objc public func getTestGroupId() -> Int {
        return bridge.getTestGroupId()
    }

    @objc public func requestTrackingAuthorization() {
        bridge.requestTrackingAuthorization()
    }
}
