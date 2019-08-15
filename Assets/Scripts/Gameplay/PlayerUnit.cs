using DG.Tweening;
using Hammerplay.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace Hammerplay.TinyArmy {
	public class PlayerUnit : MonoBehaviour, IPoolable {

		[SerializeField]
		private Bullet BulletPrefab;

		[SerializeField]
		private float rateOfFire = 0.5f;

		[SerializeField]
		private float spawnDistance = 1;

		[SerializeField]
		private GameObject muzzleFlash;

		private float lastFireTime;

		[SerializeField]
		private GameObject blastPrefab;

		[SerializeField]
		private float blastScale = 2;

		[SerializeField]
		private Collider2D deathCollider;

		[SerializeField]
		private Collider2D flameCollider;

		[SerializeField]
		private GameObject flameGameObject;

		private SpriteRenderer _spriteRenderer;


		void Awake() {
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_spriteRenderer.sprite = GameManager.GetPlayerSprite();
		}

		private void SpawnBullet() {

			if (Time.time > rateOfFire + lastFireTime) {
				GameCamera.CameraShake(0.1f, 0.1f);

				IPoolable iPoolable = PoolManager.Instantiate(currentPowerup + "Bullet", transform.position + (transform.up * spawnDistance), Quaternion.identity);				
			
				Bullet bullet = iPoolable.GetGameObject().GetComponent<Bullet>();
				bullet.SetDamage(GetDamage(currentPowerup));
				bullet.SetLane(laneId);

				muzzleFlash.gameObject.SetActive(false);
				muzzleFlash.gameObject.SetActive(true);

				transform.DOMoveY(transform.position.y + 0.3f, 0.05f).SetLoops(2, LoopType.Yoyo).OnComplete(() => {
					transform.position = initialPosition;
				});

				lastFireTime = Time.time;
			}
		}

		private float GetDamage (PowerupType powerupType) {
			switch (powerupType) {
				case PowerupType.Pistol:
					return 1.0f;//GameManager.SessionInfo.PistolBullet;
				case PowerupType.MachineGun:
					return 1.5f;// GameManager.SessionInfo.MachineGunBullet;
				case PowerupType.MiniGun:
					return 2.0f;//GameManager.SessionInfo.MiniGunBullet;
				case PowerupType.Rocket:
					return 3.0f;//GameManager.SessionInfo.RocketBullet;
			}
			return 0;
		}

		private void OnTriggerEnter2D(Collider2D collision) {
			Enemy enemy = collision.gameObject.GetComponent<Enemy>();

			if (Flame) {

				if (enemy != null) {
					if (enemy.IsNuke)
						GameManager.GameOver("Flamed a Nuke");
					else
						enemy.ApplyDamage(10);

					return;
				}
			}
			else {

				if (enemy != null) {
					if (enemy.IsNuke) {
						//Destroy(enemy.gameObject);
						enemy.PoolDestroy();
						WaveSystem.NukeHandled();
						return;
					}

					GameManager.GameOver("Enemy reached end");
				}

				Powerup powerup = collision.gameObject.GetComponent<Powerup>();

				if (powerup != null) {
					powerup.ActivatePowerup();
				}
			}
		}

		private void OnEnable() {
			GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
			Powerup.OnPowerupActivated += Powerup_OnPowerupActivated;

			Lane.OnLapTapped += Lane_OnLapTapped;
			Lane.OnLaneTouching += Lane_OnLaneTouching;

            Enemy.OnBossSpawn += Enemy_OnBossSpawn;

        }

        private void Enemy_OnBossSpawn() {
            if(flame == true) {
                Flame = false;
            }
        }

        private void Lane_OnLapTapped(int laneId) {
			if (this.laneId != laneId)
				return;

			if (currentPowerup != PowerupType.Flamethrower)
				SpawnBullet();

			
			Flame = false;
		}

		private void Lane_OnLaneTouching(int laneId, bool touching) {
			if (this.laneId != laneId)
				return;

			if (currentPowerup != PowerupType.Flamethrower)
				return;

			Flame = touching;
		}

		private bool flame;

		public bool Flame {
			get { return flame; }
			set {
				flame = value;
				flameCollider.enabled = flame;
				flameGameObject.SetActive(flame);
			}
		}

		private void OnDisable() {
			GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
			// Lane.OnLaneTapped -= Lane_OnLaneTapped;
			Powerup.OnPowerupActivated -= Powerup_OnPowerupActivated;
			// Lane.OnLaneTapHold -= Lane_OnLaneTapHold;

			Lane.OnLapTapped -= Lane_OnLapTapped;
			Lane.OnLaneTouching -= Lane_OnLaneTouching;

            Enemy.OnBossSpawn -= Enemy_OnBossSpawn;
		}

		private void GameManager_OnGameStateChanged(GameManager.GameState gameState) {
			if (gameState == GameManager.GameState.GameOver)
				DestroyMe();
		}

		public PowerupType currentPowerup;
        // converts enum to proper weapon names.
        private string PowerUpNames(PowerupType powerup) {
            string weaponName = "";
            switch (powerup) {
                case PowerupType.Pistol:
                    weaponName = "Pistols";
                    break;
                case PowerupType.MachineGun:
                    weaponName = "Machine guns";
                    break;
                case PowerupType.MiniGun:
                    weaponName = "Mini guns";
                    break;
                case PowerupType.Rocket:
                    weaponName = "Rocket Launchers";
                    break;
                case PowerupType.Flamethrower:
                    weaponName = "Flame throwers";
                    break;
            }
            return weaponName;
        }

		private void Powerup_OnPowerupActivated(Powerup powerup) {

            Gratification._instance.ShowWeaponNotification(PowerUpNames(powerup.PowerupType));

			currentPowerup = powerup.PowerupType;
			rateOfFire = powerup.RateOfFire;
			StartCoroutine(DeactivatePowerup());
		}

		private IEnumerator DeactivatePowerup() {
			yield return new WaitForSeconds(GameManager.PowerupDuration);
			Flame = false;
			currentPowerup = PowerupType.Pistol;
			rateOfFire = 0.1f;
		}

		private void DestroyMe() {
			GameObject blastGO = Instantiate(blastPrefab, transform.position, transform.rotation) as GameObject;
			blastGO.transform.localScale = new Vector3(blastScale, blastScale, blastScale);

			PoolDestroy();
		}

		private Vector3 initialPosition;

		public void PoolInstantiate(Vector3 position, Quaternion rotation) {
			gameObject.SetActive(true);
			initialPosition = position;
			transform.position = position;
			transform.rotation = rotation;
		}

		public void PoolDestroy() {
			gameObject.SetActive(false);
		}

		public bool IsAlive() {
			return gameObject.activeSelf;
		}

		public GameObject GetGameObject() {
			return gameObject;
		}

		[SerializeField]
		private int laneId = -1;

		private void SetLaneId(int laneId) {
			this.laneId = laneId;
		}
	}

	 public enum SoldierType {
        Soldier1 = 0,
        Soldier2 = 1,
        Soldier3 = 2,
        Soldier4 = 3
    }

}
