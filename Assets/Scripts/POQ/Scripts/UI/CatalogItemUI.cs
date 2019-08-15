using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hammerplay {

	public class CatalogItemUI : MonoBehaviour {

		[SerializeField]
		private Button buyButton;

		[SerializeField]
		private TextMeshProUGUI displayName;

		[SerializeField]
		private TextMeshProUGUI amountText;

		[SerializeField]
		private Image itemImage;

		private string itemId;
		public string itemItem {
			get { return itemId; }
		}

		private CatalogItem item;
		private CurrencyType currencyType;

		private bool hasEnoughBalance;
		private bool isInInventory;

		public void SetCatalogItem (CatalogItem item) {
			this.item = item;
			itemImage.sprite = Resources.Load<Sprite>(string.Format ("Items/{0}", item.ItemId));
			name = string.Format ("Item - {0}", item.ItemId);
			hasEnoughBalance = true;
			isInInventory = POQBridge.IsInInventory (item.ItemId);

			itemId = item.ItemId;

			displayName.text = item.DisplayName;
			if (item.VirtualCurrencyPrices.ContainsKey ("QR")) {
				hasEnoughBalance = POQBridge.Balance > item.VirtualCurrencyPrices["QR"];

				currencyType = CurrencyType.QR;
				amountText.text =
					string.Format ("Buy <sprite name=\"Quarter\">{0}", item.VirtualCurrencyPrices["QR"]);
			} else if (item.VirtualCurrencyPrices.ContainsKey ("RM")) {
				hasEnoughBalance = true;
				currencyType = CurrencyType.RM;
				amountText.text =
					string.Format ("Buy <sprite name=\"Dollar\">{0}", item.VirtualCurrencyPrices["RM"]);
			}

			if (isInInventory) {
				buyButton.interactable = false;
				return;
			}

			buyButton.interactable = hasEnoughBalance;

		}

		public void RefreshItem () {
			SetCatalogItem (this.item);
		}

		private void OnEnable () {
			buyButton.onClick.AddListener (StartPurchase);
		}

		private void OnDisable () {
			buyButton.onClick.RemoveAllListeners ();
		}

		private void StartPurchase () {
			if (POQBridge.IsGuest) {
				UIPopup uiPopup = UIPopup.GetPopup ("YesAndNo");
				uiPopup.Data.SetLabelsTexts ("Guest Mode", "Quarters purchased as Guest, cannot be used in another game or device.");
				uiPopup.Data.SetButtonsLabels ("Buy", "Cancel");
				uiPopup.Data.SetButtonsCallbacks (
					() => {
						uiPopup.Hide ();
						Buy ();
					},
					() => { uiPopup.Hide (); });
				uiPopup.Show ();
				return;
			}

			Buy ();
		}

		private void Buy () {
			POQBridge.BuyItem (item, currencyType);
		}

	}
}