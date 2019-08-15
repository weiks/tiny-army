using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine;
using Doozy.Engine.Nody;
using Doozy.Engine.UI;
using PlayFab.ClientModels;
using QuartersSDK;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Hammerplay {

	public class POQBridgeUI : MonoBehaviour {

		[SerializeField]
		private Button guestLoginButton, loginButton, logoutButton, playButton, howToPlayButton;

		[SerializeField]
		private UIView loginView, mainMenu;

		[SerializeField]
		private GameObject userDetailsGO, quarterBalanceGO;

		[SerializeField]
		private TextMeshProUGUI usernameText, balanceText;

		[SerializeField]
		private CatalogItemUI catalogItemPrefab;

		[SerializeField]
		private Transform shopContentTransform;

		private void Start () {
			//GraphController graphController = GraphController.Get("BootGraphController");
			//GraphController.AddToScene (graphController);
		}

		private void OnEnable () {
			guestLoginButton.onClick.AddListener (GuestLogin);
			loginButton.onClick.AddListener (QuartersLogin);
			playButton.onClick.AddListener (StartGame);

			logoutButton.onClick.AddListener (Logout);
			howToPlayButton.onClick.AddListener (HowToPlay);
			//howToPlay.onClick.AddListener (StartWager);

			POQBridge.OnMessageUpdate += POQBridge_OnMessageUpdate;
			POQBridge.OnLoginStatusChanged += POQBridge_OnLoginStatusChanged;
			POQBridge.OnLogin += POQBridge_OnLogin;
			POQBridge.OnBalanceUpdated += POQBridge_OnBalanceUpdated;
			POQBridge.OnCatalogUpdated += POQBridge_OnCatalogUpdated;
			POQBridge.OnPurchaseStatusChanged += POQBridge_OnPurchaseStatusChanged;

			POQBridge.OnGameOver += POQBridge_OnGameOver;
		}

		private void OnDisable () {
			guestLoginButton.onClick.RemoveAllListeners ();
			loginButton.onClick.RemoveAllListeners ();
			logoutButton.onClick.RemoveAllListeners ();
			playButton.onClick.RemoveAllListeners ();
			howToPlayButton.onClick.RemoveAllListeners ();

			POQBridge.OnMessageUpdate -= POQBridge_OnMessageUpdate;
			POQBridge.OnLoginStatusChanged -= POQBridge_OnLoginStatusChanged;
			POQBridge.OnLogin -= POQBridge_OnLogin;
			POQBridge.OnBalanceUpdated -= POQBridge_OnBalanceUpdated;
			POQBridge.OnCatalogUpdated -= POQBridge_OnCatalogUpdated;
			POQBridge.OnPurchaseStatusChanged -= POQBridge_OnPurchaseStatusChanged;
			POQBridge.OnGameOver -= POQBridge_OnGameOver;

		}

		private void POQBridge_OnMessageUpdate (string message, bool isError) {
			UIPopup notificationPopup = UIPopup.GetPopup ("Notification");
			notificationPopup.Data.SetLabelsTexts ("Notification", message);
			notificationPopup.AutoHideAfterShow = true;
			notificationPopup.AutoHideAfterShowDelay = 1f;
			notificationPopup.Show (true);
		}

		private void POQBridge_OnLoginStatusChanged (LoginStatus loginStatus) {
			Debug.Log ("LoginStatus: " + loginStatus);
			if (loginStatus == LoginStatus.Fail) {
				GameEventMessage.SendEvent ("OnLoginFailed");
			}
		}

		private void GuestLogin () {
			POQBridge.QuartersLogin (true);
		}

		private void QuartersLogin () {
			POQBridge.QuartersLogin (false);
		}

		private void StartGame () {
			GameEventMessage.SendEvent ("StartGame");
			SceneManager.LoadScene ("Game");
		}

		private void HowToPlay () {
			UIPopup howToPlayPopup = UIPopup.GetPopup ("HowToPlay");
			howToPlayPopup.Show ();
		}

		private UIPopup wagerPopup;
		private TMP_InputField wagerInput;

		private void Logout () {
			if (POQBridge.IsGuest) {
				UIPopup uiPopup = UIPopup.GetPopup ("YesAndNo");
				uiPopup.Data.SetLabelsTexts ("Guest Mode", "You will lose all your Quarters, when you logout. Proceed?");
				uiPopup.Data.SetButtonsLabels ("Logout", "Cancel");
				uiPopup.Data.SetButtonsCallbacks (
					() => {
						POQBridge.QuartersLogout ();
						GameEventMessage.SendEvent ("OnLogout");
						uiPopup.Hide ();
					},
					() => { uiPopup.Hide (); });
				uiPopup.Show ();
				return;
			}
			POQBridge.QuartersLogout ();
			GameEventMessage.SendEvent ("OnLogout");
		}

		private void POQBridge_OnLogin (bool isFirstTime, bool isGuest, User user) {
			usernameText.text = POQBridge.PlayerProfile.DisplayName;
			GameEventMessage.SendEvent (isFirstTime ? "OnGuestLogin" : "OnLogin");
			//POQBridge.RefreshUserBalance ();
		}

		private void POQBridge_OnBalanceUpdated (long balance) {
			if (balance == -1) {
				balanceText.text = "Refreshing...";
			} else {
				balanceText.text = string.Format ("<sprite name=\"Quarter\"> {0}", balance);
				RefreshCatalogItems ();
			}
		}

		private Dictionary<string, CatalogItemUI> catalogItemsUI = new Dictionary<string, CatalogItemUI> ();

		private void POQBridge_OnCatalogUpdated (List<CatalogItem> items) {
			foreach (var item in items) {
				if (catalogItemsUI.ContainsKey (item.ItemId)) {
					// Update the item
					catalogItemsUI[item.ItemId].SetCatalogItem (item);
				} else {
					CatalogItemUI itemUI = Instantiate (catalogItemPrefab, shopContentTransform.position, Quaternion.identity, shopContentTransform);
					itemUI.SetCatalogItem (item);
					catalogItemsUI.Add (item.ItemId, itemUI);
				}
			}
		}

		public void RefreshCatalogItems () {
			foreach (var item in catalogItemsUI) {
				item.Value.RefreshItem ();
			}
		}

		private UIPopup purchasePopup;
		private void POQBridge_OnPurchaseStatusChanged (PurchaseStatus status) {
			switch (status) {
				case PurchaseStatus.Begin:
					purchasePopup = UIPopup.GetPopup ("Notification");
					purchasePopup.Data.SetLabelsTexts ("Purchase", status.ToString ());
					purchasePopup.Show (true);
					break;

				case PurchaseStatus.Success:
				case PurchaseStatus.Fail:

					purchasePopup.Data.SetLabelsTexts ("Purchase", status.ToString ());
					purchasePopup.AutoHideAfterShow = true;
					purchasePopup.AutoHideAfterShowDelay = 1;
					purchasePopup.Show (true);
					break;
			}
		}

		private UIPopup gameOverPopup;

		private void POQBridge_OnGameOver (string scoreText, bool hasUsedSavedLife, UnityAction homeCallBack, UnityAction continueWithQuarters, UnityAction continueWithAds) {
			gameOverPopup = UIPopup.GetPopup ("GameOver");

			GameObject continuePanel = gameOverPopup.Data.Images[1].gameObject;
			continuePanel.SetActive (!hasUsedSavedLife);

			gameOverPopup.Data.SetLabelsTexts ("Game Over", scoreText);
			gameOverPopup.Data.SetButtonsCallbacks (homeCallBack, continueWithQuarters, continueWithAds);

			Button continueWithQuartersButton = gameOverPopup.Data.Buttons[1].GetComponent<Button> ();
			Button continueWithAdButton = gameOverPopup.Data.Buttons[2].GetComponent<Button> ();

			continueWithQuartersButton.interactable = POQBridge.CanUseQuartersForLife;
			continueWithAdButton.interactable = AdManager.IsRewardAdLoaded;

			gameOverPopup.Show ();
		}

	}
}