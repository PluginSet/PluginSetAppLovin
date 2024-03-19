using PluginSet.Core;
using PluginSet.Core.Editor;
using UnityEditor;

namespace PluginSet.AppLovin.Editor
{
    [BuildTools]
    public static class BuildAppLovinTools
    {
        [OnSyncEditorSetting]
        public static void OnSyncEditorSetting(BuildProcessorContext context)
        {
            if (!context.BuildTarget.Equals(BuildTarget.Android) && !context.BuildTarget.Equals(BuildTarget.iOS))
                return;
            
            var buildParams = context.BuildChannels.Get<BuildAppLovinParams>();
            if (!buildParams.Enable)
                return;
            
            context.Symbols.Add("ENABLE_APPLOVIN");
            context.AddLinkAssembly("PluginSet.AppLovin");
            
            var appLovinConfig = context.BuildChannels.Get<AppLovinParams>();
            var pluginConfig = context.Get<PluginSetConfig>("pluginsConfig");
            var config = pluginConfig.AddConfig<PluginAppLovinConfig>("AppLovin");
            config.MaxSdkKey = appLovinConfig.SdkKey;
            config.BannerAdUnitId = buildParams.BannerAdUnitId;
            config.RewardAdUnitId = buildParams.RewardAdUnitId;
            config.InterstitialAdUnitId = buildParams.InterstitialAdUnitId;
            config.OpenAdUnitId = buildParams.AppOpenAdUnitId;
            
            Global.CopyDependenciesInLib("com.pluginset.applovin", "Dependencies", delegate(string name)
            {
                if (name.StartsWith("AdColony/"))
                    return !buildParams.IncludeAdColony;
                
                if (name.StartsWith("BidMachine/"))
                    return !buildParams.IncludeBidMachine;
                
                if (name.StartsWith("ByteDance/"))
                    return !buildParams.IncludeByteDance;
                
                if (name.StartsWith("Chartboost/"))
                    return !buildParams.IncludeChartboost;
                
                if (name.StartsWith("Criteo/"))
                    return !buildParams.IncludeCriteo;
                
                if (name.StartsWith("CSJ/"))
                    return !buildParams.IncludeCSJ;
                
                if (name.StartsWith("Facebook/"))
                    return !buildParams.IncludeFacebook;
                
                if (name.StartsWith("Fyber/"))
                    return !buildParams.IncludeFyber;
                
                if (name.StartsWith("Google/"))
                    return !buildParams.IncludeGoogle;
                
                if (name.StartsWith("GoogleAdManager/"))
                    return !buildParams.IncludeGoogleAdManager;
                
                if (name.StartsWith("HyprMX/"))
                    return !buildParams.IncludeHyprMX;
                
                if (name.StartsWith("InMobi/"))
                    return !buildParams.IncludeInMobi;
                
                if (name.StartsWith("IronSource/"))
                    return !buildParams.IncludeIronSource;
                
                if (name.StartsWith("Mintegral/"))
                    return !buildParams.IncludeMintegral;
                
                if (name.StartsWith("Line/"))
                    return !buildParams.IncludeLine;

                if (name.StartsWith("LinkedIn/"))
                    return !buildParams.IncludeLinkedIn;
                
                if (name.StartsWith("Maio/"))
                    return !buildParams.IncludeMaio;
                
                if (name.StartsWith("Mintegral/"))
                    return !buildParams.IncludeMintegral;

                if (name.StartsWith("MobileFuse/"))
                    return !buildParams.IncludeMobileFuse;
                
                if (name.StartsWith("MyTarget/"))
                    return !buildParams.IncludeMyTarget;
                
                if (name.StartsWith("Nend/"))
                    return !buildParams.IncludeNend;
                
                if (name.StartsWith("OguryPresage/"))
                    return !buildParams.IncludeOguryPresage;
                
                if (name.StartsWith("Smaato/"))
                    return !buildParams.IncludeSmaato;
                
                if (name.StartsWith("Tapjoy/"))
                    return !buildParams.IncludeTapjoy;
                
                if (name.StartsWith("TencentGDT/"))
                    return !buildParams.IncludeTencentGDT;
                
                if (name.StartsWith("UnityAds/"))
                    return !buildParams.IncludeUnityAds;
                
                if (name.StartsWith("Verve/"))
                    return !buildParams.IncludeVerve;
                
                if (name.StartsWith("Vungle/"))
                    return !buildParams.IncludeVungle;
                
                if (name.StartsWith("Yandex/"))
                    return !buildParams.IncludeYandex;

                if (name.StartsWith("Bigo/"))
                    return !buildParams.IncludeBigo;
                
                return false;
            });
        }

        [AndroidProjectModify]
        public static void OnAndroidProjectModify(BuildProcessorContext context, AndroidProjectManager projectManager)
        {
            var buildParams = context.BuildChannels.Get<BuildAppLovinParams>();
            if (!buildParams.Enable)
                return;

            if (buildParams.IncludeGoogle)
            {
                var appLovinConfig = context.BuildChannels.Get<AppLovinParams>();
                projectManager.LibraryManifest.SetMetaData("com.google.android.gms.ads.APPLICATION_ID", appLovinConfig.AdMobAndroidAppId);
            }

            if (buildParams.IncludeVungle)
            {
                var gradle1 = projectManager.LibraryGradle;
                var node1 = gradle1.ROOT.GetOrCreateNode("android/packagingOptions");
                node1.AppendContentNode("exclude 'META-INF/kotlinx-serialization-json.kotlin_module'");
                node1.AppendContentNode("exclude 'META-INF/kotlinx-serialization-core.kotlin_module'");
                node1.AppendContentNode("exclude 'META-INF/okio.kotlin_module'");
                
                var gradle2 = projectManager.LauncherGradle;
                var node2 = gradle2.ROOT.GetOrCreateNode("android/packagingOptions");
                node2.AppendContentNode("exclude 'META-INF/kotlinx-serialization-json.kotlin_module'");
                node2.AppendContentNode("exclude 'META-INF/kotlinx-serialization-core.kotlin_module'");
                node2.AppendContentNode("exclude 'META-INF/okio.kotlin_module'");
            }
        }

        [iOSXCodeProjectModify(int.MaxValue)]
        public static void OnIOSXCodeProjectModify(BuildProcessorContext context, PBXProjectManager projectManager)
        {
            var buildParams = context.BuildChannels.Get<BuildAppLovinParams>();
            if (!buildParams.Enable)
                return;

            if (buildParams.IncludeGoogle)
            {
                var appLovinConfig = context.BuildChannels.Get<AppLovinParams>();
                projectManager.PlistDocument.SetPlistValue("GADApplicationIdentifier", appLovinConfig.AdMobIosAppId);
            }

            if (buildParams.IncludeGoogleAdManager)
            {
                projectManager.PlistDocument.SetPlistValue("GADIsAdManagerApp", true);
            }
        }
    }
    
}
