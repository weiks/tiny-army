using Doozy.Engine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Hammerplay.TinyArmy { 
    public class PauseMenu : MonoBehaviour {

	[SerializeField]
        private Button resumeButton, exitButton;

        [SerializeField]
        private Image pausePanel;
       

        private void OnEnable() {
            resumeButton.onClick.AddListener(ResumeGame);
            exitButton.onClick.AddListener(() => {
                Time.timeScale = 1;
                PlayerPrefs.SetInt("CurrentScore", 0);
                GameEventMessage.SendEvent("MainMenu");
                SceneManager.LoadScene("Empty");
            });
        }

        private void OnDisable() {
            resumeButton.onClick.RemoveAllListeners();
            exitButton.onClick.RemoveAllListeners();
        }

        public void ResumeGame() {
            GameManager.Instance.ChangeGameState(GameManager.GameState.InGame);
            pausePanel.gameObject.SetActive(false);
        }
    }
}

