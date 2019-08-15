using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Hammerplay.Utils;
using DG.Tweening;
using DarkTonic.MasterAudio;

namespace Hammerplay.TinyArmy {
    public class PlayerStats : MonoBehaviour {

        [SerializeField]
        private Button backButton;

        [SerializeField]
        private TextMeshProUGUI numberOfGamesPlayedText;

        [SerializeField]
        private TextMeshProUGUI numberOfKillsText;

        [SerializeField]
        private TextMeshProUGUI highScoreText;

        [SerializeField]
        private TextMeshProUGUI currentRank;

        [SerializeField]
        private TextMeshProUGUI nextRank;

        [SerializeField]
        private Slider playerLevelSlider;

        [SerializeField]
        private Image rankImage;

        [SerializeField]
        private Sprite[] ranks;

        [Header("Dog Tags References")]
        [SerializeField]
        private Image dogTagImage;

        [SerializeField]
        private Sprite[] dogtags;

        [SerializeField]
        private Button dogTagStoreButton;

        [SerializeField]
        private GameObject dogTagStorePanel;

        [SerializeField]
        private Button dogTagPanelCloseButton;

        [Header("Gun Frame References")]
        [SerializeField]
        private Image gunFrameImage;

        [SerializeField]
        private Sprite[] gunFrames;

        [SerializeField]
        private Button gunFrameStoreButton;

        [SerializeField]
        private GameObject gunFrameStorePanel;

        [SerializeField]
        private Button gunFramePanelCloseButton;


        private int numberOfGamesPlayed;
        private int numberOfKills;
        private int highScore;

        private void OnEnable() {
            backButton.onClick.AddListener(BackButton);

            dogTagStoreButton.onClick.AddListener(() => {
                dogTagStorePanel.gameObject.SetActive(true);
                MasterAudio.PlaySound("click");
            });
            dogTagPanelCloseButton.onClick.AddListener(BackButton);

            gunFrameStoreButton.onClick.AddListener(() => {
                gunFrameStorePanel.gameObject.SetActive(true);
                MasterAudio.PlaySound("click");
            });
            gunFramePanelCloseButton.onClick.AddListener(BackButton);

            SetPlayerProperties();
        }

        private void OnDisable() {
            dogTagStoreButton.onClick.RemoveAllListeners();
            dogTagPanelCloseButton.onClick.RemoveAllListeners();

            gunFrameStoreButton.onClick.RemoveAllListeners();
            gunFramePanelCloseButton.onClick.RemoveAllListeners();

            backButton.onClick.RemoveAllListeners();
            playerLevelSlider.value = 0;
        }

        private void Update() {
            if (Input.GetKeyDown("escape")) {
                BackButton();
            }
        }

        private void BackButton() {
            MasterAudio.PlaySound("click");
            if (dogTagStorePanel.activeSelf || gunFrameStorePanel.activeSelf) {
                dogTagStorePanel.SetActive(false);
                gunFrameStorePanel.SetActive(false);
                SetPlayerProperties();
            }
            else {
                gameObject.SetActive(false);
            }
        }

        private void SetPlayerProperties() {
            rankImage.sprite = ranks[DataManager._instance.PlayerRankID];
            gunFrameImage.sprite = gunFrames[DataManager._instance.GunFramesID];
            dogTagImage.sprite = dogtags[DataManager._instance.DogTagID];

            currentRank.text = DataManager._instance.PlayerLevel.ToString();
            nextRank.text = (int.Parse(currentRank.text) + 1).ToString();
            numberOfGamesPlayedText.text ="Games Played : " + numberOfGamesPlayed.ToString();
            numberOfKillsText.text ="Total Kills : " + DataManager._instance.TotalKills.ToString();
            highScoreText.text = "High Score : " + DataManager._instance.HighScore.ToString();
            CalculateXP();
        }

        private void CalculateXP() {
            float sliderValue = (float)DataManager._instance.PlayerXP / (float)DataManager._instance.MaxXP;
            Debug.Log(sliderValue);
            playerLevelSlider.DOValue(sliderValue, 0.5f).SetEase(Ease.OutSine);
        }
    }
}
