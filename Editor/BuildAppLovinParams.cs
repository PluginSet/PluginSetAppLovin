using System;
using PluginSet.Core;
using PluginSet.Core.Editor;
using UnityEngine;

namespace PluginSet.AppLovin.Editor
{
    [BuildChannelsParams("AppLovin", "AppLovin SDK 配置")]
    public class BuildAppLovinParams: ScriptableObject
    {
        [Tooltip("是否启用AppLovin")]
        public bool Enable;
        
        [VisibleCaseBoolValue("Enable", true)]
        [Tooltip("Banner广告ID")]
        public string BannerAdUnitId;
        
        [VisibleCaseBoolValue("Enable", true)]
        [Tooltip("视频广告ID")]
        public string RewardAdUnitId;
        
        [VisibleCaseBoolValue("Enable", true)]
        [Tooltip("插屏广告ID")]
        public string InterstitialAdUnitId;
        
        [VisibleCaseBoolValue("Enable", true)]
        [Tooltip("开屏广告ID")]
        public string AppOpenAdUnitId;

        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeAdColony;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeBidMachine;

        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeByteDance;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeChartboost;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeCriteo;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeCSJ;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeFacebook;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeFyber;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeGoogle;
        
        [VisibleCaseBoolValue("IncludeGoogle", true)]
        [DisableEdit]
        [Tooltip("Google广告AppId（安卓*请在下方AppLovinConfig中编辑*）")]
        public string GoogleAdMobAndroidAppId;
        
        [VisibleCaseBoolValue("IncludeGoogle", true)]
        [DisableEdit]
        [Tooltip("Google广告AppId（IOS*请在下方AppLovinConfig中编辑*）")]
        public string GoogleAdMobIosAppId;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeGoogleAdManager;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeHyprMX;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeInMobi;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeIronSource;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeLine;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeLinkedIn;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeMaio;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeMintegral;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeMobileFuse;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeMyTarget;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeNend;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeOguryPresage;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeSmaato;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeTapjoy;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeTencentGDT;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeUnityAds;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeVerve;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeVungle;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeYandex;

        private void OnValidate()
        {
            if (IncludeGoogle)
            {
                GoogleAdMobIosAppId = AppLovinSettings.Instance.AdMobIosAppId;
                GoogleAdMobAndroidAppId = AppLovinSettings.Instance.AdMobAndroidAppId;
            }
        }
    }
}
