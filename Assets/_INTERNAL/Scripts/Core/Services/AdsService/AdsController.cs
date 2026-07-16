using StartApp;
using System;
using System.Collections;
using UnityEngine;

namespace Core.Services.AdsService
{
    public class AdsController : MonoBehaviour
    {
        private static AdsController _instance;

        private InterstitialAd _rewardedAd;
        private Action _onRewardCallback;

        private bool _isDebug = true;

        public static AdsController Instance
        {
            get => _instance;
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAds();
        }

        private void OnDestroy()
        {
            if (_rewardedAd != null)
            {
                _rewardedAd.RaiseAdLoaded -= OnAdLoaded;
                _rewardedAd.RaiseAdVideoCompleted -= OnVideoCompleted;
                _rewardedAd.RaiseAdLoadingFailed -= OnAdLoadingFailed;
                _rewardedAd.RaiseAdClosed -= OnAdClosed;
            }
        }

        private void InitializeAds()
        {
            Debug.Log("[Ads] Инициализация система рекламы Start.io...");

            _rewardedAd = AdSdk.Instance.CreateInterstitial();

            _rewardedAd.RaiseAdLoaded += OnAdLoaded;
            _rewardedAd.RaiseAdVideoCompleted += OnVideoCompleted;
            _rewardedAd.RaiseAdLoadingFailed += OnAdLoadingFailed;
            _rewardedAd.RaiseAdClosed += OnAdClosed;

            if (_isDebug)
                AdSdk.Instance.SetTestAdsEnabled(true);

            PreloadRewardedAd();
        }

        public void PreloadRewardedAd()
        {
            if (_rewardedAd == null)
            {
                Debug.LogWarning("[Ads] Rewarded ad не инициализирован!");
                return;
            }

            if (_rewardedAd.IsReady())
            {
                Debug.Log("[Ads] Rewarded объявление уже загружено");
                return;
            }

            Debug.Log("[Ads] Начало загрузки rewarded объявления...");
            _rewardedAd.LoadAd(InterstitialAd.AdType.Rewarded);
        }

        public void ShowRewardedAd(Action onRewardCallback)
        {
            if (_rewardedAd == null)
            {
                Debug.LogWarning("[Ads] Rewarded ad не инициализирован!");
                return;
            }

            if (!_rewardedAd.IsReady())
            {
                Debug.LogWarning("[Ads] Rewarded объявление еще не загружено!");
                return;
            }

            _onRewardCallback = onRewardCallback;

            Debug.Log("[Ads] Показ rewarded объявления...");
            _rewardedAd.ShowAd();
        }

        public bool IsRewardedAdLoaded() => _rewardedAd != null && _rewardedAd.IsReady();

        private void OnAdLoaded(object sender, EventArgs e)
        {
            Debug.Log("[Ads] Rewarded объявление загружено и готово к показу");
        }

        private void OnVideoCompleted(object sender, EventArgs e)
        {
            Debug.Log("[Ads] Пользователь завершил просмотр рекламы");

            _onRewardCallback?.Invoke();
            _onRewardCallback = null;
        }

        private void OnAdLoadingFailed(object sender, MessageArgs e)
        {
            Debug.LogWarning($"[Ads] Ошибка загрузки rewarded объявления: {e.Message}");
        }

        private void OnAdClosed(object sender, EventArgs e)
        {
            Debug.Log("[Ads] Rewarded объявление закрыто");

            PreloadRewardedAd();
        }
    }
}