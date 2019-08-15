using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DarkTonic.MasterAudio;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using System;

namespace Hammerplay.TinyArmy {
	public class GameOverPanel : MonoBehaviour {

		private CanvasGroup canvasGroup;

		[SerializeField]
		private Transform scoreTitle, scoreText;

        [SerializeField]
        private Transform highScoreTitle, highScoreText;

        [SerializeField]
		private TextMeshProUGUI notificationTitle, notificationMessage;

        [SerializeField]
        private TextMeshProUGUI currentLevel, nextLevel;

        [SerializeField]
        private Slider playerLevelSlider;

		[SerializeField]
		private Button retryButton,continueButton,homeButton,quitButton;

        private int score;

        private int highScore;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
            score = GameManager.Score;
            DataManager._instance.CheckHighScore(score);
            highScore = GameManager.HighScore;
        }

        private void OnEnable() {

            //AdManager.Instance.RewardAd.OnUserEarnedReward += GameOver_OnAdUserEarnedReward;
            //AdManager.Instance.RewardAd.OnAdClosed += GameOver_OnAdClosed;


            retryButton.onClick.AddListener(() => {
                //Setting flags to determine the reload.
                //AdManager.Instance.HideBannerAd();

                LevelManager.Instance.restart = true;
                LevelManager.Instance.isScoreDirty = true;

                LevelManager.Instance.retriesTillAd--;

                LevelManager.Instance.battleFieldID = GameManager.MapIndex;
                LevelManager.Instance.playerID = GameManager.SoliderIndex;

                LevelManager.Instance.rewardOnce = true;
                MasterAudio.PlaySound("click");

                SceneManager.LoadScene(0);
            });

            homeButton.onClick.AddListener(() => {
                LevelManager.Instance.menu = true;
                LevelManager.Instance.isScoreDirty = true;

                LevelManager.Instance.battleFieldID = GameManager.MapIndex;
                LevelManager.Instance.playerID = GameManager.SoliderIndex;

                SceneManager.LoadScene(0);
                LevelManager.Instance.rewardOnce = true;
                MasterAudio.PlaySound("click");
            });

            quitButton.onClick.AddListener(() => Application.Quit());

            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            retryButton.gameObject.SetActive(false);

            scoreTitle.localScale = scoreText.localScale = Vector3.zero;
            highScoreTitle.localScale = highScoreText.localScale = Vector3.zero;

            float timeDelay = LevelManager.Instance.rewardOnce == false ? 0.25f : 2.5f;
            canvasGroup.DOFade(1, 0.1f).SetDelay(timeDelay).OnComplete(() => {
                MasterAudio.PlaySound("woosh", pitch: 1);
                scoreTitle.DOScale(Vector3.one, 0.2f).OnComplete(() => {
                    MasterAudio.PlaySound("woosh", pitch: 1.5f);
                    scoreText.DOScale(Vector3.one, 0.2f).OnComplete(() => {
                        MasterAudio.PlaySound("woosh", pitch: 2);
                        highScoreTitle.DOScale(Vector3.one, 0.2f).OnComplete(() => {
                            MasterAudio.PlaySound("woosh", pitch: 2);
                            highScoreText.DOScale(Vector3.one, 0.2f).OnComplete(() => {
                                MasterAudio.PlaySound("woosh", pitch: 2);
                                SetPlayerStats();

                                // display ad
                                if (LevelManager.Instance.retriesTillAd == 0) {

                                    LevelManager.Instance.retriesTillAd = LevelManager.TRIES_TILL_AD;
                                    //AdManager.Instance.DisplayInterstitialAd();
                                }

                                // canvasGroup.interactable = true;
                                // retryButton.gameObject.SetActive(true);
                            });
                        });
                    });
                });
            });
        }

        private void GameOver_OnAdClosed(object sender, EventArgs e) {

        }

        private void GameOver_OnAdUserEarnedReward(object sender, Reward e) {
            SceneManager.LoadScene(0);
        }

        private void SetPlayerStats() {
            UpdateRank();
            playerLevelSlider.gameObject.SetActive(true);
            float sliderValue = (float)DataManager._instance.PlayerXP / (float)DataManager._instance.MaxXP;
            Debug.Log(sliderValue);
            playerLevelSlider.DOValue(sliderValue, 0.5f).SetEase(Ease.OutSine).OnComplete(CalculateScore);
        }

        private void UpdateRank() {
            currentLevel.text = DataManager._instance.PlayerLevel.ToString();
            nextLevel.text = (int.Parse(currentLevel.text) + 1).ToString();
        }

        private void CalculateScore() {
            int localXP = (DataManager._instance.MaxXP - DataManager._instance.PlayerXP);
            if (localXP < score) {
                score -= localXP;
                playerLevelSlider.DOValue(1, 0.5f).SetEase(Ease.OutSine).SetDelay(0.1f).OnComplete(() => {
                    DataManager._instance.RankUp();
                    CalculateScore();
                    playerLevelSlider.value = 0;
                    UpdateRank();
                    notificationMessage.gameObject.SetActive(true);
                });
            }

            else if (localXP > score) {
                notificationMessage.gameObject.SetActive(false);
                DataManager._instance.PlayerXP += (int)score;
                playerLevelSlider.DOValue((float)DataManager._instance.PlayerXP / (float)DataManager._instance.MaxXP, 0.5f).SetEase(Ease.OutSine)
                    .OnComplete(() => {
                        canvasGroup.interactable = true;
                        retryButton.gameObject.SetActive(true);
                        homeButton.gameObject.SetActive(true);
                        quitButton.gameObject.SetActive(true);

                        UpdateRank();
                    });;
                DataManager._instance.SaveData();
            }

            else if (localXP == score) {
                playerLevelSlider.DOValue(1, 0.5f).SetEase(Ease.OutSine).OnComplete(() => {
                    DataManager._instance.RankUp();
                    canvasGroup.interactable = true;
                    retryButton.gameObject.SetActive(true);
                    playerLevelSlider.value = 0;
                    notificationMessage.gameObject.SetActive(true);
                    UpdateRank();
                });
                DataManager._instance.SaveData();
            }
        }

        private void OnDisable() {
			retryButton.onClick.RemoveAllListeners();
            //AdManager.Instance.RewardAd.OnUserEarnedReward -= GameOver_OnAdUserEarnedReward;

        }
    }
}
