using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

namespace Hammerplay.TinyArmy { 
    public class MenuManager : MonoBehaviour {

	    [SerializeField]
        private Button pauseButton;

        [SerializeField]
        private TextMeshProUGUI scoreText;

        [SerializeField]
        private Image pausePanel, resumePanel, selectionPanel;

        private void Start() {
            if (POQBridge.IsContinuedGame) {
                selectionPanel.gameObject.SetActive(false);
                GameManager.RestartGame();
            } else {
                selectionPanel.gameObject.SetActive(true);
            }
        }

        private void OnEnable() {
            GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
            GameManager.OnScoreChanged += GameManager_OnScoreChanged;
            pauseButton.onClick.AddListener(PauseGame);
        }


        private void OnDisable() {
            GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
            GameManager.OnScoreChanged -= GameManager_OnScoreChanged;
            pauseButton.onClick.RemoveAllListeners();
        }

        private void GameManager_OnScoreChanged(int score) {
            scoreText.text = score.ToString();
        }

        private void GameManager_OnGameStateChanged(GameManager.GameState gameState) {
            switch (gameState) {

                case GameManager.GameState.Paused:
                    pauseButton.gameObject.SetActive(false);
                    scoreText.gameObject.SetActive(false);
                    pausePanel.gameObject.SetActive(true);
                    break;

                case GameManager.GameState.Resume:
                    pausePanel.gameObject.SetActive(false);
                    resumePanel.gameObject.SetActive(true);
                    break;

                case GameManager.GameState.GameOver:
                    AdManager.DisplayInterstitialAd();
                    POQBridge.StartGameOver(scoreText.text, ReturnToMainMenu, ContinueWithGame, ContinueWithGame, ContinueFailed);
                    GameOver(true);
                    break;
                    
                case GameManager.GameState.InGame:
                    pausePanel.gameObject.SetActive(false);
                    pauseButton.gameObject.SetActive(true);
                    scoreText.gameObject.SetActive(true);
                    break;
                                       
                default:
                    break;
            }
        }

        private void ReturnToMainMenu() {
            Debug.Log("Return to main menu");
            GameEventMessage.SendEvent("MainMenu");
            SceneManager.LoadScene("Empty");
        }

        private void ContinueWithGame() {
            Debug.Log("Continuing with game");
            SceneManager.LoadScene("Game");
        }

        private void ContinueFailed() {
            Debug.Log("Continue failed");
            POQBridge.StartGameOver(scoreText.text, ReturnToMainMenu, ContinueWithGame, ContinueWithGame, ContinueFailed);
        }

        private void GameOver(bool gameOver) {
            pauseButton.gameObject.SetActive(!gameOver);
            pausePanel.gameObject.SetActive(!gameOver);
            resumePanel.gameObject.SetActive(!gameOver);
            scoreText.gameObject.SetActive(!gameOver);
        }

        private void PauseGame() {
            GameManager.Instance.ChangeGameState(GameManager.GameState.Paused);
        }
     }
}
