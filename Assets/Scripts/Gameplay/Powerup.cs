using Hammerplay.Utils;
using UnityEngine;

namespace Hammerplay.TinyArmy {

	public class Powerup : MonoBehaviour, IPoolable {

		[SerializeField]
		private PowerupType powerupType;

		public PowerupType PowerupType {
			get { return powerupType; }
		}

		[SerializeField]
		private float rateOfFire = 0.5f;

		public float RateOfFire {
			get { return rateOfFire; }
		}

		public void ActivatePowerup() {
			if (OnPowerupActivated != null) {
				OnPowerupActivated(this);
			}

			PoolDestroy();
		}

		public delegate void PowerupHandler(Powerup powerup);
		public static event PowerupHandler OnPowerupActivated;


		[SerializeField]
		private float speed = 1;

		private void OnEnable() {
			GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
		}

		private void OnDisable() {
			GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
		}

		private void GameManager_OnGameStateChanged(GameManager.GameState gameState) {
			if (gameState == GameManager.GameState.GameOver) {

				Blast blast = PoolManager.Instantiate("Blast", transform.position, Quaternion.identity) as Blast;
				//blast.transform.localScale = new Vector3(blastScale, blastScale, blastScale);

				PoolDestroy();
			}
		}

		private void Update() {
			transform.Translate(-transform.up * (speed) * Time.deltaTime);
		}

		public GameObject GetGameObject() {
			return this.gameObject;
		}

		public bool IsAlive() {
			return gameObject.activeSelf;
		}

		public void PoolDestroy() {
			gameObject.SetActive(false);
		}

		public void PoolInstantiate(Vector3 position, Quaternion rotation) {
			gameObject.SetActive(true);
			transform.position = position;
			transform.rotation = rotation;
		}
	}

	public enum PowerupType { Pistol, MachineGun, MiniGun, Flamethrower, Rocket }
}
