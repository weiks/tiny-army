using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DarkTonic.MasterAudio;

namespace Hammerplay.TinyArmy {
    public class MapSelectionPanel : MonoBehaviour {

        public static MapSelectionPanel _instance;
        [SerializeField]
        private Image mapIconImage, playerLockedImage;

        [SerializeField]
        private Sprite[] mapIconSprites;

        [SerializeField]
        private string[] mapNames;

        [SerializeField]
        private Button leftSelectionButton;

        [SerializeField]
        private Button rightSelectionButton;

        [SerializeField]
        private TextMeshProUGUI mapName;

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

        private void Awake()
        {
            _instance = this;
        }

        private void OnLeftSelection() {
            MasterAudio.PlaySound("click");
            if (GameManager.MapIndex > 1) {
                GameManager.MapIndex--;
            }
            else {
                GameManager.MapIndex = mapIconSprites.Length;
            }

            if (GameManager.MapIndex > 1) { 
                if (POQBridge.IsInInventory(DataManager._instance.MapPrefabID[GameManager.MapIndex - 1])) {
                    playerLockedImage.gameObject.SetActive(false);
                } else {
                    playerLockedImage.gameObject.SetActive(true);
                }
            } else if (GameManager.MapIndex == 1) {
                playerLockedImage.gameObject.SetActive(false);
            }

            mapIconImage.sprite = mapIconSprites[GameManager.MapIndex - 1];
            mapName.text = mapNames[GameManager.MapIndex - 1];

            ToggleSelectionButton();
        }

        public void ToggleSelectionButton() {
            
            if (DataManager._instance.BattleFieldStatus[GameManager.MapIndex - 1] == 0){
                // mapIconImage.color = lockedColor;
                SetPrice(GameManager.MapIndex - 1);
            }
            else {
                // mapIconImage.color = unlockedColor;
            }
        }

        private void SetPrice(int index) {
            switch (index) {
                case 1:
                    //buyPanel.Price = 1;
                    break;
                case 2:
                    //buyPanel.Price = 200;
                    break;
                case 3:
                    //buyPanel.Price = 3;
                    break;
            }
        }


        private void OnRightSelection()
        {
            Debug.Log("Map Index " + GameManager.MapIndex);
            MasterAudio.PlaySound("click");
            if (GameManager.MapIndex < mapIconSprites.Length) {
                GameManager.MapIndex++;
            }
            else {
                GameManager.MapIndex = 1;
            }


            if (GameManager.MapIndex > 1) { 
                if (POQBridge.IsInInventory(DataManager._instance.MapPrefabID[GameManager.MapIndex - 1])) {
                    playerLockedImage.gameObject.SetActive(false);
                } else {
                    playerLockedImage.gameObject.SetActive(true);
                }
            } else if(GameManager.MapIndex == 1){
                playerLockedImage.gameObject.SetActive(false);
            }


            mapIconImage.sprite = mapIconSprites[GameManager.MapIndex - 1];
            mapName.text = mapNames[GameManager.MapIndex - 1];

            ToggleSelectionButton();
        }

    }
}
