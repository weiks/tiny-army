using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using Hammerplay;
using UnityEngine;
using UnityEngine.Events;

//[DefaultExecutionOrder(-200)]
public class AdManager : MonoBehaviour {

    private static AdManager _instance;

    #region Variables
    private bool isRewardAdLoaded = false;

    [Header ("Android Ads Settings")]
    [SerializeField]
    private string androidAppId = "ca-app-pub-3940256099942544~3347511713";

    [SerializeField]
    private string androidBannerAdId = "ca-app-pub-3940256099942544/6300978111";

    [SerializeField]
    private string androidInterstitialAdId = "ca-app-pub-3940256099942544/1033173712";

    [SerializeField]
    private string androidRewardAdId = "ca-app-pub-3940256099942544/5224354917";

    [Header ("iOS Ads Settings")]
    [SerializeField]
    private string iOSAppId = "ca-app-pub-3940256099942544~1458002511";

    [SerializeField]
    private string iOSBannerAdId = "ca-app-pub-3940256099942544/2934735716";

    [SerializeField]
    private string iOSInterstitialAdId = "ca-app-pub-3940256099942544/4411468910";

    [SerializeField]
    private string iOSRewardAdId = "ca-app-pub-3940256099942544/1712485313";

    private BannerView bannerAd;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardAd;

    public static bool IsRewardAdLoaded {
        get {
            return _instance.isRewardAdLoaded;
        }
    }

    /* public static RewardedAd RewardAd {
        get { return _instance.rewardAd; }
    }*/

    #endregion

    #region Initialize Ads
    private void Awake () {

        _instance = this;
        InitAds ();
    }

    private void InitAds () {
#if UNITY_ANDROID
        string appId = androidAppId;
#elif UNITY_IPHONE
        string appId = iOSAppId;
#else
        string appId = "unexpected_platform";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize (appId);

        MobileAds.Initialize (androidAppId);
        RequestBanner ();
        RequestInterstitialAd ();
        RequestRewardAd ();
    }
    #endregion

    #region Banner Ads Events
    private void BannerEventsEnable () {
        //// Called when an ad request has successfully loaded.
        //bannerAd.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        bannerAd.OnAdFailedToLoad += Banner_OnAdLoadFail;
        //// Called when an ad is clicked.
        //bannerAd.OnAdOpening += HandleOnAdOpened;
        //// Called when the user returned from the app after an ad click.
        //bannerAd.OnAdClosed += HandleOnAdClosed;
        //// Called when the ad click caused the user to leave the application.
        bannerAd.OnAdLeavingApplication += BannerAdHandle_OnAdLeavingApplication;
    }

    private void Banner_OnAdLoadFail (object sender, AdFailedToLoadEventArgs e) {
        RequestBanner ();
    }

    private void BannerAdHandle_OnAdLeavingApplication (object sender, EventArgs e) {
        bannerAd.Destroy ();
    }
    #endregion

    #region Interstitial Ad Events
    private void InterstitialEventsEnable () {
        //// Called when an ad request has successfully loaded.
        interstitialAd.OnAdLoaded += HandleOnAdLoaded;
        //// Called when an ad request failed to load.
        interstitialAd.OnAdFailedToLoad += Interestitial_OnAdLoadFail;
        //// Called when an ad is shown.
        //interstitialAd.OnAdOpening += HandleOnAdOpened;
        //// Called when the ad is closed.
        interstitialAd.OnAdClosed += HandleOnAdClosed;
        //// Called when the ad click caused the user to leave the application.
        interstitialAd.OnAdLeavingApplication += InterstitialADHandle_OnAdLeavingApplication;
    }

    private void HandleOnAdLoaded (object sender, EventArgs e) {

    }

    private void HandleOnAdClosed (object sender, EventArgs e) {
        RequestInterstitialAd ();
    }

    private void Interestitial_OnAdLoadFail (object sender, AdFailedToLoadEventArgs e) {
        RequestInterstitialAd ();
    }

    private void InterstitialADHandle_OnAdLeavingApplication (object sender, EventArgs e) {
        interstitialAd.Destroy ();
    }
    #endregion

    #region Reward Ad Events
    private void RewardEventsEnable () {
        //// Called when an ad request has successfully loaded.
        rewardAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        rewardAd.OnAdFailedToLoad += RewardAd_OnAdLoadFail;
        //// Called when an ad is shown.
        //rewardAd.OnAdOpening += HandleRewardedAdOpening;
        //// Called when an ad request failed to show.
        //rewardAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        //// Called when the user should be rewarded for interacting with the ad.
        rewardAd.OnUserEarnedReward += HandleUserEarnedReward;
        //// Called when the ad is closed.
        rewardAd.OnAdClosed += RewardAd_OnAdClosed;
    }

    private void HandleRewardedAdLoaded (object sender, EventArgs e) {
        Debug.Log ("Reward Ad Ready");
        isRewardAdLoaded = true;
    }

    private void HandleUserEarnedReward (object sender, Reward e) {
        if (onRewardSuccess != null)
            onRewardSuccess.Invoke ();

        RequestRewardAd ();
    }

    private void RewardAd_OnAdClosed (object sender, EventArgs e) {
        if (onRewardFail != null)
            onRewardFail.Invoke ();

        RequestRewardAd ();
    }

    private void RewardAd_OnAdLoadFail (object sender, AdErrorEventArgs e) {
        isRewardAdLoaded = false;
        RequestRewardAd ();
    }
    #endregion

    #region Remove Handlers
    private void BannerEventsDisable () {
        bannerAd.OnAdFailedToLoad -= Banner_OnAdLoadFail;
        bannerAd.OnAdLeavingApplication -= BannerAdHandle_OnAdLeavingApplication;
    }

    private void InterstitialEventsDisable () {
        interstitialAd.OnAdLoaded -= HandleOnAdLoaded;
        interstitialAd.OnAdFailedToLoad -= Interestitial_OnAdLoadFail;
        interstitialAd.OnAdClosed -= HandleOnAdClosed;
        interstitialAd.OnAdLeavingApplication -= InterstitialADHandle_OnAdLeavingApplication;
    }

    private void RewardEventsDisable () {
        if (rewardAd != null) {
            rewardAd.OnAdLoaded -= HandleRewardedAdLoaded;
            rewardAd.OnAdFailedToLoad -= RewardAd_OnAdLoadFail;
            rewardAd.OnUserEarnedReward -= HandleUserEarnedReward;
            rewardAd.OnAdClosed -= RewardAd_OnAdClosed;
        }
    }
    #endregion

    #region Request Ads
    private void RequestBanner () {

#if UNITY_ANDROID
        string adUnitId = androidBannerAdId;
#elif UNITY_IPHONE
        string adUnitId = iOSBannerAdId;
#else
        string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        bannerAd = new BannerView (adUnitId, AdSize.Banner, AdPosition.Bottom);

        // For live app
#if PRODUCTION
        AdRequest adRequest = new AdRequest.Builder ().Build ();
#endif

        // For testing
#if DEV
        AdRequest adRequest = new AdRequest.Builder ()
            .AddTestDevice ("2077ef9a63d2b398840261c8221a0c9b")
            .Build ();
#endif
        BannerEventsEnable ();

        bannerAd.LoadAd (adRequest);

        bannerAd.Hide ();

    }

    private void RequestInterstitialAd () {

#if UNITY_ANDROID
        string adUnitId = androidInterstitialAdId;
#elif UNITY_IPHONE
        string adUnitId = iOSInterstitialAdId;
#else
        string adUnitId = "unexpected_platform";
#endif

        if (interstitialAd != null)
            interstitialAd.Destroy ();

        // Create a 320x50 banner at the top of the screen.
        interstitialAd = new InterstitialAd (adUnitId);

        // For live app
#if PRODUCTION
        AdRequest adRequest = new AdRequest.Builder ().Build ();
#endif

        // For testing
#if DEV
        AdRequest adRequest = new AdRequest.Builder ()
            .AddTestDevice ("2077ef9a63d2b398840261c8221a0c9b")
            .Build ();
#endif
        InterstitialEventsEnable ();

        interstitialAd.LoadAd (adRequest);
    }

    private void RequestRewardAd () {

#if UNITY_ANDROID
        string adUnitId = androidRewardAdId;
#elif UNITY_IPHONE
        string adUnitId = iOSRewardAdId;
#else
        string adUnitId = "unexpected_platform";
#endif
        RewardEventsDisable ();
        isRewardAdLoaded = false;
        // Create a 320x50 banner at the top of the screen.
        rewardAd = new RewardedAd (adUnitId);

        // For live app
#if PRODUCTION
        AdRequest adRequest = new AdRequest.Builder ().Build ();
#endif

        // For testing
#if DEV
        AdRequest adRequest = new AdRequest.Builder ()
            .AddTestDevice ("2077ef9a63d2b398840261c8221a0c9b")
            .Build ();
#endif

        RewardEventsEnable ();
        rewardAd.LoadAd (adRequest);
    }
    #endregion

    #region Public Methods

    public static void DisplayBannerAd () {
        if (_instance.bannerAd != null) {
            if (POQBridge.IsGuest) {
                if (POQBridge.IsInInventory ("removeads")) {
                    HideBannerAd ();
                } else {
                    _instance.bannerAd.Show ();
                }
            } else {
                HideBannerAd ();
            }
        }
    }

    public void DisplayBannerAdsInternal () {
        DisplayBannerAd ();
    }

    public static void DisplayInterstitialAd () {

        if (_instance.interstitialAd.IsLoaded () && !POQBridge.IsInInventory ("removeads"))
            _instance.interstitialAd.Show ();

    }

    private UnityAction onRewardSuccess, onRewardFail;

    public static void DisplayRewardAd (UnityAction onRewardSuccess, UnityAction onRewardFail) {

        if (_instance.rewardAd.IsLoaded ()) {
            _instance.rewardAd.Show ();
        }

        _instance.onRewardSuccess = onRewardSuccess;
        _instance.onRewardFail = onRewardFail;

    }

    public static void HideBannerAd () {
        _instance.bannerAd.Hide ();
    }
    #endregion
}