using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;
using System;

namespace Hammerplay.TinyArmy {
    public class CashStore : MonoBehaviour {

        [SerializeField]
        private Button backButton;

        [SerializeField]
        private Button removeAds;

        [SerializeField]
        private Image removeAdsQuartersIcon;

        [SerializeField]
        private Text removeAdsText;

        [SerializeField]
        private Button smallPackButton;

        [SerializeField]
        private Button mediumPackButton;

        [SerializeField]
        private Button largePackButton;

        [SerializeField]
        private int removeQuartersPrice = 5;

        public void RefreshUI(){
            if (DataManager._instance.playerData.ShouldPlayAds == 0) {
                removeAds.interactable = false;
                removeAdsQuartersIcon.gameObject.SetActive(false);
                removeAdsText.text = "Done";
            }
            else {
                removeAds.interactable = true;
                removeAdsQuartersIcon.gameObject.SetActive(true);
                removeAdsText.text = "\t\t" + removeQuartersPrice;
            }
        }

        private void OnEnable() {
            //AdManager.RemoveAdsEvent += RefreshUI;
            backButton.onClick.AddListener(BackButton);
            removeAds.onClick.AddListener(RemoveAds);
            smallPackButton.onClick.AddListener(Buy100Coins);
            mediumPackButton.onClick.AddListener(Buy500Coins);
            largePackButton.onClick.AddListener(Buy1000Coins);
            RefreshUI();
        }

        private void OnDisable() {
            //AdManager.RemoveAdsEvent -= RefreshUI;

            backButton.onClick.RemoveAllListeners();
            removeAds.onClick.RemoveAllListeners();
            smallPackButton.onClick.RemoveAllListeners();
            mediumPackButton.onClick.RemoveAllListeners();
            largePackButton.onClick.RemoveAllListeners();
        }

        private void RemoveAds() {
            // POQ call
            //POQUI.Instance.RemoveAds(removeQuartersPrice);
            // disable the button on successfull remove.

        }

        private void Buy100Coins() {
                // DataManager._instance.Coins +=100;
            //IAPManager._instance.Buy100Coins();
        }

        private void Buy500Coins(){
            // DataManager._instance.Coins += 500;
            //IAPManager._instance.Buy500Coins();
        }

        private void Buy1000Coins(){
            // DataManager._instance.Coins += 1000;
            //IAPManager._instance.Buy1000Coins();
        }

        private void Update() {
            if (Input.GetKeyDown("escape")) {
                BackButton();
            }
        }

        private void BackButton() {
            MasterAudio.PlaySound("click");
            gameObject.SetActive(false);
        }
    }
}
