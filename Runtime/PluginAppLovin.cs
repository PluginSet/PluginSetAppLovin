#if ENABLE_APPLOVIN
using System;
using System.Collections;
using System.Collections.Generic;
using PluginSet.Core;

namespace PluginSet.AppLovin
{
    [PluginRegister]
    public class PluginAppLovin : PluginBase, IStartPlugin, IBannerAdPlugin, IRewardAdPlugin, IInterstitialAdPlugin
    {
        private static readonly Logger Logger = LoggerManager.GetLogger("AppLovin");
        public override string Name => "AppLovin";

        public int StartOrder => PluginsStartOrder.SdkDefault;
        public bool IsRunning { get; private set; }

        private bool _inited = false;

        private string _maxSdkKey;
        private string _bannerAdUnitId;
        private string _rewardAdUnitId;
        private string _interstitialAdUnitId;
        
        private Action _onRewardAdLoadedSuccess;
        private Action<int> _onRewardAdLoadedFail;
        private Action<bool, int> _onRewardCallback;
        
        private bool _isLoadingRewardAd;
        private bool _isShowingRewardAd;
        
        private Action _onInterstitialAdLoadedSuccess;
        private Action<int> _onInterstitialAdLoadedFail;
        private Action<bool, int> _onInterstitialCallback;
        
        private bool _isLoadingInterstitialAd;
        private bool _isShowingInterstitialAd;
        
        protected override void Init(PluginSetConfig config)
        {
            var cfg = config.Get<PluginAppLovinConfig>();
            _maxSdkKey = cfg.MaxSdkKey;
            _bannerAdUnitId = cfg.BannerAdUnitId;
            _rewardAdUnitId = cfg.RewardAdUnitId;
            _interstitialAdUnitId = cfg.InterstitialAdUnitId;
            
            MaxSdkCallbacks.OnSdkInitializedEvent += InitializeMax;
        }

        public IEnumerator StartPlugin()
        {
            if (IsRunning)
                yield break;
            
            IsRunning = true;

            MaxSdk.SetSdkKey(_maxSdkKey);
            MaxSdk.InitializeSdk();
        }

        private void InitializeMax(MaxSdkBase.SdkConfiguration sdkConfiguration)
        {
            _inited = true;
            Logger.Debug($"Max SDK initialized, version {MaxSdk.Version}");
            
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        }

        public void DisposePlugin(bool isAppQuit = false)
        {
        }

#region Banner Ad
        public bool IsEnableShowBanner => _inited;
        
        private List<string> _showingBannerAdIds = new List<string>();
        public void ShowBannerAd(string adId, BannerPosition position = BannerPosition.BottomCenter, Dictionary<string, object> extensions = null)
        {
            if (string.IsNullOrEmpty(adId))
                adId = _bannerAdUnitId;

            if (_showingBannerAdIds.Contains(adId))
                return;
            
            MaxSdk.ShowBanner(adId);
        }

        public void HideBannerAd(string adId)
        {
            if (string.IsNullOrEmpty(adId))
                adId = _bannerAdUnitId;
            
            MaxSdk.HideBanner(adId);
            
            if (_showingBannerAdIds.Contains(adId))
                _showingBannerAdIds.Remove(adId);
        }

        public void HideAllBanners()
        {
            foreach (var id in _showingBannerAdIds)
            {
                MaxSdk.HideBanner(id);
            }
            _showingBannerAdIds.Clear();
        }
#endregion

#region Reward Ad
        public bool IsEnableShowRewardAd => true;
        public bool IsReadyToShowRewardAd => _inited && MaxSdk.IsRewardedAdReady(_rewardAdUnitId);
        public void LoadRewardAd(Action success = null, Action<int> fail = null)
        {
            if (!_inited)
            {
                fail?.Invoke(PluginConstants.InvalidCode);
                return;
            }
            
            if (IsReadyToShowRewardAd)
            {
                success?.Invoke();
                return;
            }
            
            if (_isLoadingRewardAd)
            {
                fail?.Invoke((int)AdErrorCode.IsLoading);
                return;
            }
            
            if (_isShowingRewardAd)
            {
                fail?.Invoke((int)AdErrorCode.IsShowing);
                return;
            }
            
            _onRewardAdLoadedSuccess = success;
            _onRewardAdLoadedFail = fail;
            _isLoadingRewardAd = true;
            
            Logger.Debug($"Load Reward Ad {_rewardAdUnitId}");
            MaxSdk.LoadRewardedAd(_rewardAdUnitId);
        }

        public void ShowRewardAd(Action<bool, int> dismiss = null)
        {
            if (!_inited)
            {
                dismiss?.Invoke(false, PluginConstants.InvalidCode);
                return;
            }
            
            if (!IsReadyToShowRewardAd)
            {
                dismiss?.Invoke(false, (int)AdErrorCode.NotLoaded);
                return;
            }
            
            if (_isShowingRewardAd)
            {
                dismiss?.Invoke(false, (int)AdErrorCode.IsShowing);
                return;
            }
            
            _isShowingRewardAd = true;
            _onRewardCallback = dismiss;
            
            Logger.Debug($"Show Reward Ad {_rewardAdUnitId}");
            MaxSdk.ShowRewardedAd(_rewardAdUnitId);
        }
#endregion

#region Interstitial Ad
        public bool IsEnableShowInterstitialAd => true;
        public bool IsReadyToShowInterstitialAd => _inited && MaxSdk.IsInterstitialReady(_interstitialAdUnitId);
        public void LoadInterstitialAd(Action success = null, Action<int> fail = null)
        {
            if (!_inited)
            {
                fail?.Invoke(PluginConstants.InvalidCode);
                return;
            }
            
            if (IsReadyToShowInterstitialAd)
            {
                success?.Invoke();
                return;
            }
            
            if (_isLoadingInterstitialAd)
            {
                fail?.Invoke((int)AdErrorCode.IsLoading);
                return;
            }
            
            if (_isShowingInterstitialAd)
            {
                fail?.Invoke((int)AdErrorCode.IsShowing);
                return;
            }
            
            _onInterstitialAdLoadedSuccess = success;
            _onInterstitialAdLoadedFail = fail;
            _isLoadingInterstitialAd = true;
            
            Logger.Debug($"Load Interstitial Ad {_interstitialAdUnitId}");
            MaxSdk.LoadInterstitial(_interstitialAdUnitId);
        }

        public void ShowInterstitialAd(Action<bool, int> dismiss = null)
        {
            if (!_inited)
            {
                dismiss?.Invoke(false, PluginConstants.InvalidCode);
                return;
            }
            
            if (!IsReadyToShowInterstitialAd)
            {
                dismiss?.Invoke(false, (int)AdErrorCode.NotLoaded);
                return;
            }
            
            if (_isShowingInterstitialAd)
            {
                dismiss?.Invoke(false, (int)AdErrorCode.IsShowing);
                return;
            }
            
            _isShowingInterstitialAd = true;
            _onInterstitialCallback = dismiss;
            
            Logger.Debug($"Show Interstitial Ad {_interstitialAdUnitId}");
            MaxSdk.ShowInterstitial(_interstitialAdUnitId);
        }
#endregion

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Logger.Debug($"[MAX REWARDED AD] Loaded, adUnitId = {adUnitId}");
            
            _isLoadingRewardAd = false;
            var callback = _onRewardAdLoadedSuccess;
            _onRewardAdLoadedSuccess = null;
            _onRewardAdLoadedFail = null;
            callback?.Invoke();
        }

        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Logger.Debug($"[MAX REWARDED AD] Failed to load, adUnitId = {adUnitId}");
            
            _isLoadingRewardAd = false;
            var callback = _onRewardAdLoadedFail;
            _onRewardAdLoadedSuccess = null;
            _onRewardAdLoadedFail = null;
            callback?.Invoke((int)AdErrorCode.NotLoaded);
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Logger.Debug($"[MAX REWARDED AD] Displayed, adUnitId = {adUnitId}");
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        { }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        { }

        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Logger.Debug($"[MAX REWARDED AD] Dismissed, adUnitId = {adUnitId}");
            
            _isShowingRewardAd = false;
            if (_onRewardCallback != null)
                _onRewardCallback.Invoke(false, (int)AdErrorCode.Dismissed);
            _onRewardCallback = null;
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            Logger.Debug($"[MAX REWARDED AD] Failed to display, adUnitId = {adUnitId}");
            
            _isShowingRewardAd = false;
            if (_onRewardCallback != null)
                _onRewardCallback.Invoke(false, (int)AdErrorCode.ShowFail);
            _onRewardCallback = null;
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            Logger.Debug($"[MAX REWARDED AD] Received reward, adUnitId = {adUnitId}");
            
            if (_onRewardCallback != null)
                _onRewardCallback.Invoke(true, (int)AdErrorCode.Success);
            _onRewardCallback = null;
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Logger.Debug($"[MAX INTERSTITIAL AD] Loaded, adUnitId = {adUnitId}");
            
            _isLoadingInterstitialAd = false;
            var callback = _onInterstitialAdLoadedSuccess;
            _onInterstitialAdLoadedSuccess = null;
            _onInterstitialAdLoadedFail = null;
            callback?.Invoke();
        }

        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Logger.Debug($"[MAX INTERSTITIAL AD] Failed to load, adUnitId = {adUnitId}");
            
            _isLoadingInterstitialAd = false;
            var callback = _onInterstitialAdLoadedFail;
            _onInterstitialAdLoadedSuccess = null;
            _onInterstitialAdLoadedFail = null;
            callback?.Invoke((int)AdErrorCode.NotLoaded);
        }

        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Logger.Debug($"[MAX INTERSTITIAL AD] Displayed, adUnitId = {adUnitId}");
        }

        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        { }

        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Logger.Debug($"[MAX INTERSTITIAL AD] Dismissed, adUnitId = {adUnitId}");
            
            _isShowingInterstitialAd = false;
            if (_onInterstitialCallback == null) return;
            
            _onInterstitialCallback.Invoke(true, (int)AdErrorCode.Success);
            _onInterstitialCallback = null;
        }

        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            Logger.Debug($"[MAX INTERSTITIAL AD] Failed to display, adUnitId = {adUnitId}");
            
            _isShowingInterstitialAd = false;
            if (_onInterstitialCallback == null) return;

            _onInterstitialCallback.Invoke(false, (int)AdErrorCode.ShowFail);
            _onInterstitialCallback = null;
        }
    }
}
#endif