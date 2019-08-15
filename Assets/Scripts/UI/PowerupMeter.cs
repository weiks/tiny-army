using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Hammerplay.TinyArmy {
	public class PowerupMeter : MonoBehaviour {

		[SerializeField]
		private Image timerImage;

		[SerializeField]
		private GameObject foreground;

		[SerializeField]
		private GameObject background;

		private bool isPowerupActivated;

		private void OnEnable() {
			Powerup.OnPowerupActivated += Powerup_OnPowerupActivated;
			GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
		}

		private void OnDisable() {
			Powerup.OnPowerupActivated -= Powerup_OnPowerupActivated;
			GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
		}

		private void Start() {
			CheckStatus();
		}

		private void Powerup_OnPowerupActivated(Powerup powerup) {
			isPowerupActivated = true;
			CheckStatus();
			timerImage.fillAmount = 1;
			timerImage.DOFillAmount(0, GameManager.PowerupDuration).SetEase(Ease.Linear).OnComplete(() => {

                if(OnPowerupComplete != null) {
                    OnPowerupComplete();
                }
				isPowerupActivated = false;
				CheckStatus();
			});
			//timerImage.color = Color.green;
			//timerImage.DOColor(Color.red, GameManager.PowerupDuration).SetEase(Ease.Linear);
		}


		private GameManager.GameState gameState;

		private void GameManager_OnGameStateChanged(GameManager.GameState gameState) {
			this.gameState = gameState;
			CheckStatus();
		}

		private void CheckStatus() {

			timerImage.enabled = (gameState == GameManager.GameState.InGame && isPowerupActivated);
			foreground.SetActive(gameState == GameManager.GameState.InGame && isPowerupActivated);
			background.SetActive(gameState == GameManager.GameState.InGame && isPowerupActivated);
		}

		// Update is called once per frame
		private void Update() {
			/*if (followObject != null)
            {
                transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, followObject.position);
            }*/
		}

        public delegate void PowerupComplete();
        public static event PowerupComplete OnPowerupComplete;

    }
}
