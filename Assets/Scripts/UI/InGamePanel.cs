using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using DarkTonic.MasterAudio;

namespace Hammerplay.TinyArmy {
	public class InGamePanel : MonoBehaviour {

		[SerializeField]
		private Button pauseButton;

		[SerializeField]
		private GameObject pausedTitle, resumePanel;

		[SerializeField]
		private TextMeshProUGUI scoreText;

		[SerializeField]
		private TextMeshProUGUI remainingTimeText, resumeCountDownText;

		[SerializeField]
		private Button resumeButton, retryButton, homeButton, quitButton;

        private void OnEnable() {
            GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
            GameManager.OnScoreChanged += GameManager_OnScoreChanged;
            pauseButton.onClick.AddListener(() => {
                GameManager.PauseGame();
                MasterAudio.PlaySound("click");
            });

            resumeButton.onClick.AddListener(()=> {
            GameManager.ResumeGame();
            MasterAudio.PlaySound("click");
             });

			retryButton.onClick.AddListener(()=> {

                LevelManager.Instance.restart = true;
                LevelManager.Instance.isScoreDirty = true;

                LevelManager.Instance.retriesTillAd--;

                LevelManager.Instance.battleFieldID = GameManager.MapIndex;
                LevelManager.Instance.playerID = GameManager.SoliderIndex;

                LevelManager.Instance.rewardOnce = true;

                SceneManager.LoadScene(0);
                Time.timeScale = 1;
                MasterAudio.PlaySound("click");
            });

            homeButton.onClick.AddListener(() => {
                LevelManager.Instance.menu = true;
                LevelManager.Instance.isScoreDirty = true;
                SceneManager.LoadScene(0);
                Time.timeScale = 1;
                MasterAudio.PlaySound("click");
            });
            quitButton.onClick.AddListener(() => Application.Quit());
        }

		private void OnDisable() {
			GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
			GameManager.OnScoreChanged -= GameManager_OnScoreChanged;
			resumeButton.onClick.RemoveAllListeners();
			pauseButton.onClick.RemoveAllListeners();
			retryButton.onClick.RemoveAllListeners();
		}


		private void GameManager_OnGameStateChanged(GameManager.GameState gameState) {
            switch (gameState) {


				case GameManager.GameState.InGame:
					pauseButton.gameObject.SetActive(true);
					resumeButton.enabled = false;
					pausedTitle.SetActive(false);
					resumePanel.SetActive(false);
					break;

				case GameManager.GameState.Paused:
					pauseButton.gameObject.SetActive(false);
					resumeButton.enabled = true;
					pausedTitle.SetActive(true);
					resumePanel.SetActive(false);
					break;

				case GameManager.GameState.Resume:
					pauseButton.gameObject.SetActive(false);
					resumeButton.enabled = false;
					pausedTitle.SetActive(false);
					resumePanel.SetActive(true);

					resumeCountDown = 3;
					StartCoroutine(ResumeCountDown());
					break;
			}
		}

		private int resumeCountDown;

		private IEnumerator ResumeCountDown () {
			
			while (resumeCountDown > 0) {
				resumeCountDownText.text = resumeCountDown.ToString();
				yield return new WaitForSecondsRealtime(1);
				resumeCountDown--;
				
			}

			GameManager.InGameMode();
		}


		private float punchAmount = 0.3f;

		private bool isPunching;

		private void GameManager_OnScoreChanged(int score) {

			scoreText.text = score.ToString();
			if (isPunching)
				return;

			isPunching = true;
			scoreText.transform.DOPunchScale(new Vector3(punchAmount, punchAmount, punchAmount), 0.2f).OnComplete(() => {
				isPunching = false;
			}); ;
		}

        private void Update() {
           
		}
	}
}
