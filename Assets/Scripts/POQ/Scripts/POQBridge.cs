using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
//using PlayFab.ServerModels;

using QuartersSDK;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

#if UNITY_IOS
//using UnityEngine.SocialPlatforms.GameCenter;
#endif

#if UNITY_ANDROID
using GoogleMobileAds.Api;
#endif

namespace Hammerplay {

    public class POQBridge : MonoBehaviour, IStoreListener {

        [SerializeField]
        private string playFabTitleId = "";

        [SerializeField]
        private int priceForExtraLife = 100;

        public static bool CanUseQuartersForLife {
            get { return _instance.balance > _instance.priceForExtraLife; }
        }

        [SerializeField]
        private int beatMyBestPrice = 100;

        private string playFabId = "";

        [SerializeField]
        private bool deauthorizeByDefault;

        [SerializeField]
        private bool isNeverAuthorized = false;

        [SerializeField]
        private bool isGuest = false;

        public static bool IsGuest {
            get { return _instance.isGuest; }
        }

        [SerializeField]
        private User currentUser;

        public static User CurrentUser {
            get { return _instance.currentUser; }
        }

        private long balance;

        public static long Balance {
            get { return _instance.balance; }
        }

        private bool isStartupSequence = true;

        private IEnumerator Start () {
            for (int i = 0; i < itemNames.Count; i++)
            {
                awardAmounts.Add(string.Format("{0}_{1}", storeNamespace, itemNames[i]), itemPrices[i]);
            }
            

            PlayFabSettings.TitleId = playFabTitleId;
            yield return new WaitForSeconds (0.5f);
            isStartupSequence = true;
#if UNITY_EDITOR
            AuthenticateEditor ();
#else
            Authenticate ();
#endif
        }

        private void AuthenticateEditor () {
            var request = new LoginWithCustomIDRequest { CustomId = "EditorUser", CreateAccount = true };
            PlayFabClientAPI.LoginWithCustomID (
                request,
                OnPlayFabSignInSuccess,
                OnPlayFabError
            );
        }

        /* private void Authenticate () {
            LogMessage ("Starting with Authentication");

#if UNITY_ANDROID
            LogMessage ("Activating PlayGamesPlatform");
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
                .Builder ()
                .RequestServerAuthCode (false)
                .RequestIdToken ()
                .AddOauthScope ("profile")
                .Build ();

            PlayGamesPlatform.InitializeInstance (config);
            PlayGamesPlatform.Activate ();
#endif
            LogMessage ("Activating Social Authentication");
            Social.localUser.Authenticate ((bool success, string message) => {
                if (success == true) {
#if UNITY_ANDROID
                    LogMessage ("Social Authentication Success (Android)");
                    PlayfabSignInWithGoogle ();
#elif UNITY_IOS
                    UpdateLoginStatus ("Social Authentication Success (iOS)");
                    PlayFabSignInWithGameCenter ();
#endif
                } else {
                    LogMessage ("Social Authentication failed: " + message, true);
                }
            });
        }*/

        private void Authenticate () {
            LogMessage ("Starting with Authentication");
#if UNITY_ANDROID
            PlayfabSignInWithGoogle ();
#elif UNITY_IOS
            PlayfabSignInWithIOS ();
#endif

        }

#if UNITY_ANDROID
        private void PlayfabSignInWithGoogle () {
            //LogMessage ("[PlayFab] Sign In with Google");

            //var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode ();

            /* PlayFabClientAPI.LoginWithGoogleAccount (new LoginWithGoogleAccountRequest () {
                    TitleId = PlayFabSettings.TitleId,
                        ServerAuthCode = serverAuthCode,
                        CreateAccount = true,
                        InfoRequestParameters = new GetPlayerCombinedInfoRequestParams () {
                            GetPlayerProfile = true
                        }
                },
                OnPlayFabSignInSuccess,
                OnPlayFabError);*/

            PlayFabClientAPI.LoginWithAndroidDeviceID (new LoginWithAndroidDeviceIDRequest () {
                CreateAccount = true,
                    AndroidDeviceId = SystemInfo.deviceUniqueIdentifier
            }, OnPlayFabSignInSuccess, OnPlayFabError);

        }
#endif

#if UNITY_IOS
        // PlayFab sign in for IOS
        private void PlayfabSignInWithIOS () {

            // PlayFabClientAPI.LoginWithGameCenter (new LoginWithGameCenterRequest () {
            //         TitleId = PlayFabSettings.TitleId,
            //             CreateAccount = true,
            //             InfoRequestParameters = new GetPlayerCombinedInfoRequestParams () {
            //                 GetPlayerProfile = true
            //             },
            //             PlayerId = Social.Active.localUser.id
            //     },
            //     OnPlayFabSignInSuccess,
            //     OnPlayFabError);

            PlayFabClientAPI.LoginWithIOSDeviceID (new LoginWithIOSDeviceIDRequest () {
                CreateAccount = true,
                    DeviceId = SystemInfo.deviceUniqueIdentifier,
                    DeviceModel = SystemInfo.deviceModel,
                    OS = SystemInfo.operatingSystem,
            }, OnPlayFabSignInSuccess, OnPlayFabError);
        }
#endif

        private void OnPlayFabSignInSuccess (LoginResult loginResult) {
            Debug.Log (loginResult.ToJson ());
            playFabId = loginResult.PlayFabId;
            LogMessage ("[PlayFab] Successful sign in");
            GetPlayerProfile ();

        }

        private void OnPlayFabError (PlayFabError error) {
            LogMessage ("[PlayFab]: " + error, true);
            Debug.LogError (error.GenerateErrorReport ());
        }

        private void GetPlayerProfile () {
            PlayFabClientAPI.GetPlayerProfile (new GetPlayerProfileRequest () {
                    PlayFabId = playFabId
                },
                GetPlayerProfileSuccess,
                OnPlayFabError
            );
        }

        private PlayerProfileModel playerProfile;

        public static PlayerProfileModel PlayerProfile {
            get { return _instance.playerProfile; }
        }

        private void GetPlayerProfileSuccess (GetPlayerProfileResult result) {
            playerProfile = result.PlayerProfile;
            FetchCatalogItems ();
        }

        private void FetchCatalogItems () {
            PlayFabClientAPI.GetCatalogItems (
                new GetCatalogItemsRequest (),
                FetchCatalogItemsSuccess,
                OnPlayFabError
            );
        }

        private List<CatalogItem> catalog;

        private void FetchCatalogItemsSuccess (GetCatalogItemsResult catalogItemsResult) {
            LogMessage ("Fetched Catalog Items");

            catalog = catalogItemsResult.Catalog;

            if (OnCatalogUpdated != null) {
                OnCatalogUpdated (catalog);
            }

            if (isStartupSequence) {
#if UNITY_IOS
                var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance (AppStore.AppleAppStore));
#endif
#if UNITY_ANDROID 
                var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance (AppStore.GooglePlay));

#endif
                // Register each item from the catalog
                foreach (var item in catalog) {
                    builder.AddProduct (item.ItemId, ProductType.Consumable);
                }
                UnityPurchasing.Initialize (this, builder);
                FetchUserInventory ();
            }
        }

        public delegate void CatalogHandler (List<CatalogItem> items);
        public static event CatalogHandler OnCatalogUpdated;

        private void FetchUserInventory () {
            PlayFabClientAPI.GetUserInventory (
                new GetUserInventoryRequest (),
                FetchUserInventorySuccess,
                OnPlayFabError
            );
        }

        private List<ItemInstance> inventory;

        public static bool IsInInventory (string itemId) {
            if (_instance.inventory == null)
                return false;

            for (int i = 0; i < _instance.inventory.Count; i++) {
                if (_instance.inventory[i].ItemId == itemId)
                    return true;
            }

            return false;
        }

        private void FetchUserInventorySuccess (GetUserInventoryResult result) {
            inventory = result.Inventory;

            AdManager.DisplayBannerAd ();

            if (OnCatalogUpdated != null) {
                OnCatalogUpdated (catalog);
            }

            if (isStartupSequence)
                AuthenticateQuarters ();
            else {
                ConsumeQuarterPurchasedInInventory ();
            }

        }

        private void AuthenticateQuarters () {

            // Only for dev purposes
            if (deauthorizeByDefault) {
                Quarters.Instance.Deauthorize ();
                return;
            }

            QuartersSession session = new QuartersSession ();

            //quarters
            if (!session.IsAuthorized) {
                //first session
                Debug.Log ("First time, unauthorized");
                isNeverAuthorized = true;
                isGuest = true;
                Quarters.Instance.AuthorizeGuest (QuartersLoginSuccess, QuartersLoginFailure);
            } else {
                //not first session
                if (session.IsGuestSession) {
                    Debug.Log ("Guest login");
                    //following session with guest mode display dialog
                    isNeverAuthorized = true;
                    isGuest = true;
                    Quarters.Instance.Authorize (QuartersLoginSuccess, QuartersLoginFailure);
                    // Didn't work because it's already authorized as guest.
                    //Quarters.Instance.AuthorizeGuest (QuartersLoginSuccess, QuartersLoginFailure);
                } else {
                    //email user
                    Debug.Log ("Previous logged in as user");
                    isNeverAuthorized = false;
                    isGuest = false;
                    Quarters.Instance.Authorize (QuartersLoginSuccess, QuartersLoginFailure);
                }
            }
        }

        public static void QuartersLogin (bool loginAsGuest) {
            _instance.Login (loginAsGuest);
        }

        public static void QuartersLogout () {
            Quarters.Instance.Deauthorize ();
        }

        private void Login (bool loginAsGuest) {
            isNeverAuthorized = false;

            SetLoginStatus (LoginStatus.Started);
            if (loginAsGuest) {
                QuartersSession session = new QuartersSession ();

                if (session.IsAuthorized)
                    Quarters.Instance.Authorize (QuartersLoginSuccess, QuartersLoginFailure);
                else
                    Quarters.Instance.AuthorizeGuest (QuartersLoginSuccess, QuartersLoginFailure);

                isGuest = loginAsGuest;
            } else {
                // If guest is already authorized, unauthorizing it.
                Quarters.Instance.Deauthorize ();
                // Tried to use signup here, but got 401 unauthorized
                Quarters.Instance.Authorize (QuartersLoginSuccess, QuartersLoginFailure, true);
                isGuest = false;
            }
        }

        private void QuartersLoginSuccess () {
            SetLoginStatus (LoginStatus.Success);
            GetUserDetails ();
            RefreshUserBalance ();

            isStartupSequence = false;
            FetchUserInventory ();
        }

        private void QuartersLoginFailure (string error) {
            Debug.LogErrorFormat ("Login Failure: {0}", error);
            SetLoginStatus (LoginStatus.Fail);
        }

        public void GetUserDetails () {
            Quarters.Instance.GetUserDetails (GetUserDetailsSuccess, GetUserDetailsFailure);
        }

        private void GetUserDetailsSuccess (User user) {
            currentUser = user;

            if (OnLogin != null)
                OnLogin (isNeverAuthorized, isGuest, user);
        }

        private void GetUserDetailsFailure (string error) {
            Debug.LogErrorFormat ("Quarters User Details: {0}", error);
        }

        public static void RefreshUserBalance () {
            if (OnBalanceUpdated != null)
                OnBalanceUpdated (-1);

            _instance.GetUserBalance ();
        }

        private void GetUserBalance () {
            Quarters.Instance.GetAccountBalance (OnAccountBalanceSuccess, OnAccountBalanceFailure);
        }

        private void OnAccountBalanceSuccess (User.Account.Balance balance) {
            this.balance = balance.quarters;

            if (OnBalanceUpdated != null)
                OnBalanceUpdated (balance.quarters);
        }

        private void OnAccountBalanceFailure (string error) {
            Debug.LogErrorFormat ("Quarters Account Balance: {0}", error);
        }

        public static void BuyItem (CatalogItem item, CurrencyType currentType) {
            _instance.Buy (item, currentType);
        }

        private void Buy (CatalogItem item, CurrencyType currencyType) {
            UpdatePurchaseStatus (PurchaseStatus.Begin);

            int price = (int) item.VirtualCurrencyPrices[currencyType.ToString ()];
            string itemDescription = item.DisplayName;

            switch (currencyType) {
                case CurrencyType.QR:
                    PayQuarters (price, itemDescription,
                        () => {
                            RefreshUserBalance ();
                            PlayFabPurchase (item, currencyType);
                        },
                        () => {
                            UpdatePurchaseStatus (PurchaseStatus.Fail);
                        }
                    );

                    break;

                case CurrencyType.RM:
                    queuedItem = item;
                    BuyProductID (item.ItemId);
                    break;
            }

        }

        private CatalogItem queuedItem;

        private void PlayFabPurchase (CatalogItem item, CurrencyType currencyType) {

            PlayFabClientAPI.PurchaseItem (new PurchaseItemRequest {
                // In your game, this should just be a constant matching your primary catalog
                CatalogVersion = item.CatalogVersion,
                    ItemId = item.ItemId,
                    Price = 0,
                    VirtualCurrency = "DM"
            }, PurchaseItemSuccess, PurchaseItemFail);

        }

        private void PurchaseItemSuccess (PurchaseItemResult result) {
            UpdatePurchaseStatus (PurchaseStatus.Success);
            FetchUserInventory ();
        }

        private void PurchaseItemFail (PlayFabError error) {
            Debug.LogError (error);
            UpdatePurchaseStatus (PurchaseStatus.Fail);
        }

        private void UpdatePurchaseStatus (PurchaseStatus purchaseStatus) {
            Debug.Log (purchaseStatus);
            if (OnPurchaseStatusChanged != null)
                OnPurchaseStatusChanged (purchaseStatus);
        }

        // This session handles iOS and Andoroid IAP part.
        private void BuyProductID (string productId) {
            // If IAP service has not been initialized, fail hard
            if (!IsInitialized) throw new Exception ("IAP Service is not initialized!");

            // Pass in the product id to initiate purchase
            m_StoreController.InitiatePurchase (productId);
        }

        private IStoreController m_StoreController;

        public bool IsInitialized {
            get {
                return m_StoreController != null;
            }
        }

        public void OnInitialized (IStoreController controller, IExtensionProvider extensions) {
            m_StoreController = controller;
        }

        public void OnInitializeFailed (InitializationFailureReason error) {
            Debug.LogError ("IStoreController : " + error);
        }

        public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e) {
            // NOTE: this code does not account for purchases that were pending and are
            // delivered on application start.
            // Production code should account for such case:
            // More: https://docs.unity3d.com/ScriptReference/Purchasing.PurchaseProcessingResult.Pending.html

            if (!IsInitialized) {
                return PurchaseProcessingResult.Complete;
            }

            // Test edge case where product is unknown
            if (e.purchasedProduct == null) {
                Debug.LogWarning ("Attempted to process purchasewith unknown product. Ignoring");
                return PurchaseProcessingResult.Complete;
            }

            // Test edge case where purchase has no receipt
            if (string.IsNullOrEmpty (e.purchasedProduct.receipt)) {
                Debug.LogWarning ("Attempted to process purchase with no receipt: ignoring");
                return PurchaseProcessingResult.Complete;
            }

            Debug.Log ("Processing transaction: " + e.purchasedProduct.transactionID);

            // Deserialize receipt



            // Invoke receipt validation
            // This will not only validate a receipt, but will also grant player corresponding items
            // only if receipt is valid.
#if UNITY_IOS
            Debug.Log("Purchased Product Receipt" + e.purchasedProduct.receipt);
            ApplePurchase appleReceipt = ApplePurchase.FromJson(e.purchasedProduct.receipt);
            Debug.Log("Purchased Product Payload" + appleReceipt.Payload);

            PlayFabClientAPI.ValidateIOSReceipt(new ValidateIOSReceiptRequest() {
                // Pass in currency code in ISO format
                CurrencyCode = e.purchasedProduct.metadata.isoCurrencyCode,
                // Convert and set Purchase price
                PurchasePrice = (int)(e.purchasedProduct.metadata.localizedPrice * 100),

                ReceiptData = appleReceipt.Payload

            }, (result) => {
                    Debug.Log ("Validation successful!");
                    FetchUserInventory ();
                },
                error => Debug.Log ("Validation failed: " + error.GenerateErrorReport ())
            );

            return PurchaseProcessingResult.Complete;
#endif

            // Invoke receipt validation
            // This will not only validate a receipt, but will also grant player corresponding items
            // only if receipt is valid.
#if UNITY_ANDROID 
            var googleReceipt = GooglePurchase.FromJson (e.purchasedProduct.receipt);

            PlayFabClientAPI.ValidateGooglePlayPurchase (new ValidateGooglePlayPurchaseRequest () {
                    // Pass in currency code in ISO format
                    CurrencyCode = e.purchasedProduct.metadata.isoCurrencyCode,
                        // Convert and set Purchase price
                        PurchasePrice = (uint) (e.purchasedProduct.metadata.localizedPrice * 100),
                        // Pass in the receipt
                        ReceiptJson = googleReceipt.PayloadData.json,
                        // Pass in the signature
                        Signature = googleReceipt.PayloadData.signature
                }, (result) => {
                    Debug.Log ("Validation successful!");
                    FetchUserInventory ();
                },
                error => Debug.Log ("Validation failed: " + error.GenerateErrorReport ())
            );

            return PurchaseProcessingResult.Complete;
#endif
        }

        [ContextMenu ("Consume Quarters Purchased In Inventory")]
        private void ConsumeQuarterPurchasedInInventory () {
            foreach (var itemInstance in inventory) {
                if (itemInstance.RemainingUses > 0) {
                    ConsumePurchased (itemInstance);
                    AwardQuarters (awardAmounts[itemInstance.ItemId]);
                }
            }
        }

        [SerializeField]
        private string storeNamespace = "com.poq.productname";

        [SerializeField]
        private List<string> itemNames;

        [SerializeField]
        private List<int> itemPrices;

        [SerializeField]
        private Dictionary<string, int> awardAmounts = new Dictionary<string, int>();
        

        private void ConsumePurchased (ItemInstance itemInstance) {
            PlayFabClientAPI.ConsumeItem (new ConsumeItemRequest {
                    ConsumeCount = (int) itemInstance.RemainingUses,
                        ItemInstanceId = itemInstance.ItemInstanceId
                },
                ConsumeItemSuccess,
                OnPlayFabError
            );
        }

        private void ConsumeItemSuccess (ConsumeItemResult result) {
            Debug.Log ("Successfully consumed: " + result.ToJson ());
        }

        private void PurchaseGoogleSuccess (ValidateGooglePlayPurchaseResult result) {
            UpdatePurchaseStatus (PurchaseStatus.Success);
            RefreshUserBalance ();
            FetchUserInventory ();
        }

        private void PurchaseGoogleFailure () {
            UpdatePurchaseStatus (PurchaseStatus.Fail);
        }

        public void OnPurchaseFailed (Product product, PurchaseFailureReason failureReason) {
            Debug.LogErrorFormat ("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason);
        }

        public delegate void PurchaseStatusHandler (PurchaseStatus status);
        public static event PurchaseStatusHandler OnPurchaseStatusChanged;

        private void AwardQuarters (int amount) {
            Quarters.Instance.AwardQuarters (amount, delegate (string transactionHash) {
                LogMessage ("Quarters awarded: " + transactionHash);
                RefreshUserBalance ();

            }, delegate (string error) {
                LogMessage ("OnAwardQuartersFailed: " + error);
            });
        }

        private void PayQuarters (int amount, string description, UnityAction onSuccess, UnityAction onFailure) {
            if (balance < amount) {
                LogMessage ("Insufficient Quarters", true);
                return;
            }
            TransferAPIRequest transferRequest = new TransferAPIRequest (amount,
                description,
                (transcationHash) => {
                    Debug.Log ("Paid quarters successfully: " + transcationHash);
                    if (onSuccess != null)
                        onSuccess.Invoke ();
                },
                (error) => {
                    Debug.LogError ("Quarters Transaction fail: " + error);
                    if (onFailure != null) {
                        onFailure.Invoke ();
                    }
                });

            Quarters.Instance.CreateTransfer (transferRequest);
        }

        private void PayForExtraLife (UnityAction onSuccess, UnityAction onFailure) {
            if (balance < priceForExtraLife) {
                LogMessage ("Insufficient Quarters", true);
                ContinueFailedInternal ();
                return;
            }

            TransferAPIRequest transferRequest = new TransferAPIRequest (priceForExtraLife,
                "Extra Life",
                (transcationHash) => {
                    Debug.Log ("Paid quarters successfully: " + transcationHash);
                    isContinuedGame = true;

                    if (onSuccess != null)
                        onSuccess.Invoke ();
                },
                (error) => {
                    LogMessage ("Quarters Transaction fail: " + error);
                    ContinueFailedInternal ();
                });

            Quarters.Instance.CreateTransfer (transferRequest);
        }

        private bool isContinuedGame = false;

        public static bool IsContinuedGame {
            get { return _instance.isContinuedGame; }
        }

        private UnityAction onHomeCallback, onContinueWithQuarters, onContinueWithAds, onContinueFailed;

        public static void StartGameOver (string score, UnityAction onHomeCallback, UnityAction onContinueWithQuarters, UnityAction onContinueWithAds, UnityAction onContinueFailed) {
            _instance.ShowGameOver (score, onHomeCallback, onContinueWithQuarters, onContinueWithAds, onContinueFailed);
        }

        public void ShowGameOver (string score, UnityAction onHomeCallback, UnityAction onContinueWithQuarters, UnityAction onContinueWithAds, UnityAction onContinueFailed) {
            if (OnGameOver != null)
                OnGameOver (score, isContinuedGame, HomeCallBackInternal, ContineWithQuartersInternal, ContineWithAdsInternal);

            this.onHomeCallback = onHomeCallback;
            this.onContinueWithQuarters = onContinueWithQuarters;
            this.onContinueWithAds = onContinueWithAds;
            this.onContinueFailed = onContinueFailed;
        }

        private void HomeCallBackInternal () {
            Debug.Log ("Home has called back");
            isContinuedGame = false;

            if (onHomeCallback != null)
                onHomeCallback.Invoke ();
        }

        private void ContineWithQuartersInternal () {
            Debug.Log ("Continue with Quarters");
            PayForExtraLife (onContinueWithQuarters, ContinueFailedInternal);
        }

        private void ContineWithAdsInternal () {
            Debug.Log ("Start watching ad");
            AdManager.DisplayRewardAd (ContineWithAdSuccess, onContinueFailed);
        }

        private void ContineWithAdSuccess () {
            isContinuedGame = true;
            if (onContinueWithAds != null)
                onContinueWithAds.Invoke ();
        }

        private void ContinueFailedInternal () {
            if (onContinueFailed != null)
                onContinueFailed.Invoke ();
        }

        public delegate void GameOverHandler (
            string score,
            bool hasUsedSavedLife,
            UnityAction onHomeCallback,
            UnityAction onContinueWithQuarters,
            UnityAction onContinueWithAds
        );

        public static event GameOverHandler OnGameOver;

        private static POQBridge _instance;

        private void Awake () {
            if (_instance != null)
                Destroy (gameObject);

            _instance = this;
        }

        public delegate void MessageHandler (string message, bool isError);
        public static event MessageHandler OnMessageUpdate;

        private void LogMessage (string message, bool isError = false) {
            if (isError)
                Debug.LogError (message);
            else
                Debug.Log (message);

            if (OnMessageUpdate != null) {
                OnMessageUpdate (message, isError);
            }
        }

        private void SetLoginStatus (LoginStatus loginStatus) {
            if (OnLoginStatusChanged != null)
                OnLoginStatusChanged (loginStatus);
        }
        public delegate void LoginStatusHandler (LoginStatus loginStatus);
        public static event LoginStatusHandler OnLoginStatusChanged;

        public delegate void LoginHandler (bool isNeverAuthorized, bool isGuest, User user);
        public static event LoginHandler OnLogin;

        public delegate void BalanceHandler (long balance);
        public static event BalanceHandler OnBalanceUpdated;

    }

    public enum LoginStatus { None, Started, Success, Fail }
    public enum CurrencyType { QR, RM }
    public enum PurchaseStatus { None, Begin, Success, Fail }

    // The following classes are used to deserialize JSON results provided by IAP Service
    // Please, note that Json fields are case-sensetive and should remain fields to support Unity Deserialization via JsonUtilities
    public class JsonData {
        // Json Fields, ! Case-sensetive

        public string orderId;
        public string packageName;
        public string productId;
        public long purchaseTime;
        public int purchaseState;
        public string purchaseToken;
    }

    public class PayloadData {
        public JsonData JsonData;

        // Json Fields, ! Case-sensetive
        public string signature;
        public string json;

        public static PayloadData FromJson (string json) {
            var payload = JsonUtility.FromJson<PayloadData> (json);
            payload.JsonData = JsonUtility.FromJson<JsonData> (payload.json);
            return payload;
        }
    }

    public class GooglePurchase {
        public PayloadData PayloadData;

        // Json Fields, ! Case-sensetive
        public string Store;
        public string TransactionID;
        public string Payload;

        public static GooglePurchase FromJson (string json) {
            var purchase = JsonUtility.FromJson<GooglePurchase> (json);
            purchase.PayloadData = PayloadData.FromJson (purchase.Payload);
            return purchase;
        }
    }

    public class ApplePurchase
    {
        public string Store;
        public string TransactionID;
        public string Payload;

        public static ApplePurchase FromJson (string json)
        {
            var purchase = JsonUtility.FromJson<ApplePurchase>(json);
            return purchase;
        }
    }
}