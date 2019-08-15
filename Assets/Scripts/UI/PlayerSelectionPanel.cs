using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DarkTonic.MasterAudio;

namespace Hammerplay.TinyArmy
{
    public class PlayerSelectionPanel : MonoBehaviour
    {

        public static PlayerSelectionPanel _instance;

        [SerializeField]
        private Image playerIconImage, playerLockedImage;

        [SerializeField]
        private Sprite[] playerIconSprites;

        [SerializeField]
        private string[] playerNames;

        [SerializeField]
        private Button leftSelectionButton;

        [SerializeField]
        private Button rightSelectionButton;

        [SerializeField]
        private TextMeshProUGUI playerName;

        [SerializeField]
        private Color lockedColor, unlockedColor;

        

        void OnEnable() {
            leftSelectionButton.onClick.AddListener(OnLeftSelection);
            rightSelectionButton.onClick.AddListener(OnRightSelection);


        }

        void OnDisable() {
            leftSelectionButton.onClick.RemoveAllListeners();
            rightSelectionButton.onClick.RemoveAllListeners();
        }

        private void Awake() {
            _instance = this;
        }

        private void OnLeftSelection() {
            MasterAudio.PlaySound("click");

            if (GameManager.SoliderIndex > 1) {
                GameManager.SoliderIndex--;
            }
            else {
                GameManager.SoliderIndex = playerIconSprites.Length;
            }
            if (GameManager.SoliderIndex > 1) { 
                if (POQBridge.IsInInventory(DataManager._instance.PlayerPrefabID[GameManager.SoliderIndex - 1])) {
                    playerLockedImage.gameObject.SetActive(false);
                } else {
                    playerLockedImage.gameObject.SetActive(true);
                }
            } else if (GameManager.SoliderIndex == 1) {
                playerLockedImage.gameObject.SetActive(false);
            }

            playerIconImage.sprite = playerIconSprites[GameManager.SoliderIndex - 1];
            playerName.text = playerNames[GameManager.SoliderIndex - 1];

        }

        public void ToggleSelectionButton() {
            if (DataManager._instance.PlayerStatus[GameManager.SoliderIndex - 1] == 0) {
                SetPriceAndId(GameManager.SoliderIndex - 1);
                //buyPanel.gameObject.SetActive(true);
                // playerIconImage.color = lockedColor;
            }
            else {
                // playerIconImage.color = unlockedColor;
                //buyPanel.gameObject.SetActive(false);
            }
        }

        private void SetPriceAndId(int index) {
            //buyPanel.Id = GameManager.SoliderIndex - 1;
            switch (index) {
                case 1:
                    //buyPanel.Price = 1;
                    break;
                case 2:
                    //buyPanel.Price =2;
                    break;
                case 3:
                    //buyPanel.Price =3;
                    break;
            }
        }

        private void OnRightSelection()
        {
            Debug.Log("Soldier Index " + GameManager.SoliderIndex);
            MasterAudio.PlaySound("click");

            if (GameManager.SoliderIndex < playerIconSprites.Length) {
                GameManager.SoliderIndex++;
            }
            else {
                GameManager.SoliderIndex = 1;
            }

            if (GameManager.SoliderIndex > 1) {
                if (POQBridge.IsInInventory(DataManager._instance.PlayerPrefabID[GameManager.SoliderIndex - 1])) {
                    playerLockedImage.gameObject.SetActive(false);
                } else {
                    playerLockedImage.gameObject.SetActive(true);
                }
            } else if(GameManager.SoliderIndex == 1){
                playerLockedImage.gameObject.SetActive(false);
            }

            playerIconImage.sprite = playerIconSprites[GameManager.SoliderIndex - 1];
            playerName.text = playerNames[GameManager.SoliderIndex - 1];

            //ggleSelectionButton();
        }
        private void OnStartGame() {
            gameObject.SetActive(false);
            GameManager.RestartGame();
        }
    }
}
