using PluginSet.Core.Editor;
using UnityEditor;

namespace PluginSet.AppLovin.Editor
{
    [InitializeOnLoad]
    public static class PluginsAppLovinFilter
    {
        static PluginsAppLovinFilter()
        {
            var fileter = PluginFilter.IsBuildParamsEnable<BuildAppLovinParams>();
            PluginFilter.RegisterFilter("com.pluginset.applovin/Plugins/iOS", fileter);
            PluginFilter.RegisterFilter("com.pluginset.applovin/Plugins/Android", fileter);
            
            PluginFilter.RegisterFilter("com.pluginset.applovin/Plugins/Android/Bigo", FilterBigo);
        }
        
        private static bool FilterBigo(string path, BuildProcessorContext context)
        {
            var buildParams = context.BuildChannels.Get<BuildAppLovinParams>();
            if (!buildParams.Enable)
                return true;

            if (!buildParams.IncludeBigo)
                return true;

            return false;
        }
    }
}