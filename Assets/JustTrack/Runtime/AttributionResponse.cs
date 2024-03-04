using System;
using UnityEngine;

namespace JustTrack {
    /**
    * A representation of the attribution returned by the server.
    */
    public class AttributionResponse {
        private AttributionResponse(string pUserId, string pInstallId, AttributionCampaign pCampaign, string pUserType, string pType, AttributionChannel pChannel, AttributionNetwork pNetwork, string pSourceId, string pSourceBundleId, string pSourcePlacement, string pAdsetId, AttributionRecruiter pRecruiter, DateTime pCreatedAt) {
            this.UserId = pUserId;
            this.InstallId = pInstallId;
            this.Campaign = pCampaign;
            this.UserType = pUserType;
            this.Type = pType;
            this.Channel = pChannel;
            this.Network = pNetwork;
            this.SourceId = pSourceId;
            this.SourceBundleId = pSourceBundleId;
            this.SourcePlacement = pSourcePlacement;
            this.AdsetId = pAdsetId;
            this.Recruiter = pRecruiter;
            this.CreatedAt = pCreatedAt;
        }

        public string UserId { get; private set; }
        public string InstallId { get; private set; }
        public AttributionCampaign Campaign { get; private set; }
        public string UserType { get; private set; }
        public string Type { get; private set; }
        public AttributionChannel Channel { get; private set; }
        public AttributionNetwork Network { get; private set; }
        public string SourceId { get; private set; }
        public string SourceBundleId { get; private set; }
        public string SourcePlacement { get; private set; }
        public string AdsetId { get; private set; }
        public AttributionRecruiter Recruiter { get; private set; }
        public DateTime CreatedAt { get; private set; }

        #if UNITY_ANDROID
            internal static AttributionResponse FromAndroidObject(AndroidJavaObject pResponseObject) {
                using var userId = pResponseObject.Call<AndroidJavaObject>("getUserId");
                using var installId = pResponseObject.Call<AndroidJavaObject>("getInstallId");
                using var campaign = pResponseObject.Call<AndroidJavaObject>("getCampaign");
                using var channel = pResponseObject.Call<AndroidJavaObject>("getChannel");
                using var network = pResponseObject.Call<AndroidJavaObject>("getNetwork");
                using var recruiter = pResponseObject.Call<AndroidJavaObject>("getRecruiter");
                using var createdAt = pResponseObject.Call<AndroidJavaObject>("getCreatedAt");

                return new AttributionResponse(
                    userId.Call<string>("toString"),
                    installId.Call<string>("toString"),
                    AttributionCampaign.FromAndroidObject(campaign),
                    pResponseObject.Call<string>("getUserType"),
                    pResponseObject.Call<string>("getType"),
                    AttributionChannel.FromAndroidObject(channel),
                    AttributionNetwork.FromAndroidObject(network),
                    pResponseObject.Call<string>("getSourceId"),
                    pResponseObject.Call<string>("getSourceBundleId"),
                    pResponseObject.Call<string>("getSourcePlacement"),
                    pResponseObject.Call<string>("getAdsetId"),
                    AttributionRecruiter.FromAndroidObject(recruiter),
                    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + new TimeSpan(createdAt.Call<long>("getTime") * 10000)
                );
            }
        #endif
        #if UNITY_IOS
            internal static AttributionResponse CreateResponse(string userId, string installId, string userType, string type, int campaignId, string campaignName, string campaignType, int channelId, string channelName, bool channelIncent, int networkId, string networkName, string sourceId, string sourceBundleId, string sourcePlacement, string adsetId, string recruiterAdvertiserId, string recruiterUserId, string recruiterPackageId, string recruiterPlatform, DateTime createdAt) {
                AttributionRecruiter recruiter = null;
                if (recruiterAdvertiserId != null && recruiterUserId != null && recruiterPackageId != null && recruiterPlatform != null) {
                    recruiter = AttributionRecruiter.CreateRecruiter(recruiterAdvertiserId, recruiterUserId, recruiterPackageId, recruiterPlatform);
                }

                return new AttributionResponse(
                    userId,
                    installId,
                    AttributionCampaign.CreateCampaign(campaignId, campaignName, campaignType),
                    userType,
                    type,
                    AttributionChannel.CreateChannel(channelId, channelName, channelIncent),
                    AttributionNetwork.CreateNetwork(networkId, networkName),
                    sourceId,
                    sourceBundleId,
                    sourcePlacement,
                    adsetId,
                    recruiter,
                    createdAt
                );
            }
        #endif
        #if UNITY_EDITOR
            internal static AttributionResponse CreateFakeResponse() {
                return new AttributionResponse(
                    "00000000-0000-0000-0000-000000000000",
                    "00000000-0000-0000-0000-000000000000",
                    AttributionCampaign.CreateFakeCampaign(1, "fake campaign", "acquisition"),
                    "acquisition",
                    "fake",
                    AttributionChannel.CreateFakeChannel(1, "Organic", false),
                    AttributionNetwork.CreateFakeNetwork(1, "Direct"),
                    "fake source id",
                    "fake source bundle id",
                    "fake source placemenet",
                    "fake adset id",
                    AttributionRecruiter.CreateFakeRecruiter(),
                    DateTime.UtcNow
                );
            }
        #endif
    }
}