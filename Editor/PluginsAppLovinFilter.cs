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
        }
    }
}