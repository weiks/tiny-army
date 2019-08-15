using Hammerplay.Utils;
using System.Collections;
using UnityEngine;

namespace Hammerplay.TinyArmy {
	public class Bullet : MonoBehaviour, IPoolable {

		[SerializeField]
		private float speed = 5;

		[SerializeField]
		private float damage = 1;

		public void SetDamage (float damage) {
			this.damage = damage;
		}

		[SerializeField]
		private bool splashDamage;

		[SerializeField]
		private float splashRadius;

		[SerializeField]
		private int laneId;

		public void SetLane (int laneId) {
			this.laneId = laneId;
		}

		private void OnTriggerEnter2D(Collider2D collision) {
			// Use a tag and add destroy.
			Enemy enemy = collision.gameObject.GetComponent<Enemy>();
			if (enemy != null) {
				if (splashDamage) {
					enemy.ApplyDamage(damage);

					if (OnSplashDamage != null) {
						OnSplashDamage(laneId, splashRadius, transform.position);
					}
				} else {
					enemy.ApplyDamage(damage);
				}
				PoolDestroy();
			}
		}

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
			transform.Translate(transform.up * speed * Time.deltaTime);
		}

		private void OnBecameInvisible() {
			//Destroy(gameObject);
			PoolDestroy();
		}

		public void PoolInstantiate(Vector3 position, Quaternion rotation) {
			
			gameObject.SetActive(true);

			StartCoroutine(ResetTrail());
			transform.position = position;
			transform.rotation = rotation;
		}

		private IEnumerator ResetTrail() {
			GetComponent<TrailRenderer>().time = -10;
			yield return new WaitForSeconds(0.01f);
			GetComponent<TrailRenderer>().time = 0.1f;
		}

		public void PoolDestroy() {
			StopAllCoroutines();
			gameObject.SetActive(false);
		}

		public bool IsAlive() {
			return gameObject.activeSelf;
		}

		public GameObject GetGameObject() {
			return gameObject;
		}

		public delegate void SplashDamageHandler(int laneId, float splashRadius, Vector2 position) ;
		public static event SplashDamageHandler OnSplashDamage;
	}
}
