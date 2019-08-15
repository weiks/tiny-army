using Hammerplay.Utils;
using System;
using System.Collections;
using UnityEngine;


namespace Hammerplay.TinyArmy {
	public class WaveSystem : MonoBehaviour {

		[SerializeField]
		private int waveIndex;

		[SerializeField]
		private float waveInterval = 2;

		[SerializeField]
		private Wave[] waves;

		[SerializeField]
		private string[] enemyNames;

        private int wavesUntilBoss = 6;
        [SerializeField]
        private int intervalBetweenBoss = 6;

        private bool canSpawnCivilian = false;

		
		// Update is called once per frame
		private void Update() {
			if (!isSpawningEnemy || isChangingWave || currentWave == null || isBossOnScene)
				return;

			if (Time.time > currentWave.Interval + lastSpawnTime) {
                canSpawnCivilian = true;

                if (wavesUntilBoss == 0) {
                    StartCoroutine(NextBoss());
                    return;
                }
                for (int i = 0; i < currentWave.laneEnemyTypes.Length; i++) {
					Vector3 spawnPosition = GameManager.GetEnemeySpawnPosition((GameManager.Lane)i);

					/*int enemyType = (currentWave.laneEnemyTypes[i] == LaneEnemyType.Random) ?
										UnityEngine.Random.Range(0, (isNukeOnScene) ? 3 : (powerupTotalWaveIndex == 10) ? 4 : 3) : (int)currentWave.laneEnemyTypes[i];*/

					int enemyType = (currentWave.laneEnemyTypes[i] == LaneEnemyType.Random) ? UnityEngine.Random.Range(0, (isNukeOnScene) ? 3 : 4) : (int)currentWave.laneEnemyTypes[i];

                    if (enemyType == (int)LaneEnemyType.Nuke && !isNukeOnScene) {
                        isNukeOnScene = true;
                        nukeLane = i;

                        Enemy enemy = PoolManager.Instantiate(enemyNames[enemyType], spawnPosition, transform.rotation) as Enemy;
                        enemy.SetSpeed(currentWave.speed + waveScaleSpeed);
                        enemy.SetLaneId(i);
                    }
                    if (!(isNukeOnScene && i == nukeLane)) {
                        Enemy enemy = PoolManager.Instantiate(enemyNames[enemyType], spawnPosition, transform.rotation) as Enemy;
                        enemy.SetSpeed(currentWave.speed + waveScaleSpeed);
                        enemy.SetLaneId(i);
                    }

                }
				currentCount++;
				lastSpawnTime = Time.time;

				if (currentCount == currentWave.count) {
					 StartCoroutine(NextWave());
				}
			}

			if (!isPowerupDone && powerupTotalWaveIndex == powerupWaveIndex && !isBossOnScene) {
				isPowerupDone = true;
                StartCoroutine(SpawnPowerupInvoke());
			}
		}

		private IEnumerator SpawnPowerupInvoke () {
            yield return new WaitForSeconds(8);
			Debug.Log("Spawn powerup: " + powerupOrder[powerupIndex].ToString());
			Vector3 spawnPosition = GameManager.GetEnemeySpawnPosition(GameManager.Lane.Lane2);
			PoolManager.Instantiate(powerupOrder[powerupIndex].ToString(), spawnPosition, Quaternion.identity);
		}

		private bool isPowerupDone;
		private int powerupWaveIndex = 3;

		[SerializeField]
		private PowerupType[] powerupOrder;
		private int powerupIndex;

		[SerializeField]
		private int powerupTotalWaveIndex = 0;

		[SerializeField]
		private int totalWaveIndex = 0;

        private IEnumerator NextBoss() {
            wavesUntilBoss = intervalBetweenBoss;
            isBossOnScene = true;


            var bossSpeed = 0.1f;
            var i = UnityEngine.Random.Range(0, 3);

            yield return new WaitForSeconds(0.5f);

            GameManager.IsInputEnabled = false;
            yield return new WaitForSeconds(0.8f);

            //randomize the order of the boss
            Enemy enemy = PoolManager.Instantiate(enemyNames[4], GameManager.GetEnemeySpawnPosition((GameManager.Lane)i), transform.rotation) as Enemy;
            enemy.SetSpeed((bossSpeed * currentWave.speed) + waveScaleSpeed);
            enemy.SetLaneId(i);
            enemy.IsBoss = true;
            enemy.BossCinematicIntro();
        }

        private IEnumerator NextWave() {
            
            wavesUntilBoss--;
 
			totalWaveIndex++;
            isChangingWave = true;
			waveIndex++;
			waveIndex = Mathf.Clamp(waveIndex, 0, waves.Length - 1);
			powerupTotalWaveIndex++;
			if (powerupTotalWaveIndex == 11)
				powerupTotalWaveIndex = 0;

			switch (powerupTotalWaveIndex) {
				case 1:
					//isPowerupDone = false;
					powerupWaveIndex = UnityEngine.Random.Range(1, 3);
					powerupIndex = 0;
					break;

				case 4:
					//isPowerupDone = false;
					powerupWaveIndex = UnityEngine.Random.Range(4, 5);
					powerupIndex = 1;
					break;

				case 6:
					//isPowerupDone = false;
					powerupWaveIndex = UnityEngine.Random.Range(6, 8);
					powerupIndex = 2;
					break;

				case 9:
					//isPowerupDone = false;
					powerupWaveIndex = UnityEngine.Random.Range(9, 10);
					powerupIndex = 3;
					break;
			}
            //randomizing powerup!!
            var newIndex = UnityEngine.Random.Range(0, 5);
            while(newIndex != powerupIndex) {
                newIndex = UnityEngine.Random.Range(0, 5);
            }
            powerupIndex = newIndex;

			currentCount = 0;
			currentWave = waves[waveIndex];
			yield return new WaitForSeconds(waveInterval);

			isChangingWave = false;
			waveScaleSpeed = 1;
			if (totalWaveIndex >= 11) {
				waveScaleSpeed += 0.1f * (totalWaveIndex / 25f);
			}
		}

		private float waveScaleSpeed = 1;

		private bool isNukeOnScene;

        [SerializeField]
        private bool isBossOnScene;

        private int nukeLane = -1;

		private Wave currentWave;

		[SerializeField]
		private int currentCount;

		private float lastSpawnTime;

		[SerializeField]
		private bool isSpawningEnemy = false;


		[SerializeField]
		private bool isChangingWave;

		private void OnEnable() {
			GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
            Enemy.OnEnemyKilled += Enemy_BossKilled;
            PowerupMeter.OnPowerupComplete += Powerup_OnComplete;
		}

        private void Powerup_OnComplete() {
            isPowerupDone = false;
        }

        private void Enemy_BossKilled(LaneEnemyType enemyType) {
            if(enemyType == LaneEnemyType.Boss) {
                isBossOnScene = false;
            }
        }

        private void OnDisable() {
			GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
            Enemy.OnEnemyKilled -= Enemy_BossKilled;
            PowerupMeter.OnPowerupComplete -= Powerup_OnComplete;

        }

        private void GameManager_OnGameStateChanged(GameManager.GameState gameState) {
			switch (gameState) {
				case GameManager.GameState.StartMenu:

					break;

				case GameManager.GameState.Start:
					for (int i = 0; i < enemyNames.Length; i++) {
						PoolManager.ClearOnScreen(enemyNames[i]);
					}
					currentCount = 0;
					waveIndex = 0;
					isNukeOnScene = false;
					nukeLane = -1;
					break;

				case GameManager.GameState.InGame:
					currentWave = waves[waveIndex];
					isSpawningEnemy = true;
					break;

				case GameManager.GameState.GameOver:
					isSpawningEnemy = false;
					break;
			}
		}

		private static WaveSystem _instance;

        public bool IsBossOnScene {
            get {
                return isBossOnScene;
            }

            set {
                isBossOnScene = value;
            }
        }

        private void Awake() {
			_instance = this;

            //setting the first boss way
            wavesUntilBoss = intervalBetweenBoss;

        }


		public static void NukeHandled() {
			_instance.StartCoroutine(_instance.EnableNukeBack());
		}

		private IEnumerator EnableNukeBack () {
			nukeLane = -1;
			yield return new WaitForSeconds(UnityEngine.Random.Range(5, 10));
			isNukeOnScene = false;
		}
	}

	[System.Serializable]
	public class Wave {
		public int count = 5;

		private float interval = 0.75f;

		public float Interval {
			get { return interval; }
		}

		public float speed = 5;
		public LaneEnemyType[] laneEnemyTypes = new LaneEnemyType[3];

	}

	public enum LaneEnemyType { Human, Bike, Plane, Nuke, Random, Boss }	
}
