using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Hammerplay.Utils;
using TMPro;

namespace Hammerplay.TinyArmy
{
    public class Enemy : MonoBehaviour, IPoolable
    {

        [SerializeField]
        private LaneEnemyType enemyType;

        [SerializeField]
        private float health = 1;

        [SerializeField]
        private float speed = 5;

        [SerializeField]
        private float minSpeedScale = 1;

        [SerializeField]
        private float maxSpeedScale = 1;

        private float speedScale;

        [SerializeField]
        private int score = 1;

        public void SetSpeed(float speed) {
            this.speed = speed;
        }

        [SerializeField]
        private float blastScale = 1;

        [SerializeField]
        private bool isNuke;

        [SerializeField]
        private bool isBoss;
        [SerializeField]
        private bool isCutscene = true;

        // Timer
        private float laneChangeInterval = 1f;
        private float timeSinceSwap;


        public bool IsNuke {
            get { return isNuke; }
        }

        public bool IsBoss {
            get {
                return isBoss;
            }

            set {
                isBoss = value;
            }
        }

        public float Speed {
            get {
                return speed;
            }

            set {
                speed = value;
            }
        }

        private int laneId;

        public void SetLaneId(int laneId) {
            this.laneId = laneId;
        }

        private float initialHealth;

        private SpriteRenderer spriteRenderer;

        private void Awake() {
            initialHealth = health;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void ApplyDamage(float damage) {

            health -= damage;

            if (OnEnemyDamage != null)
                OnEnemyDamage(initialHealth, health);


            spriteRenderer.DOColor(Color.red, 0.05f).SetLoops(2, LoopType.Yoyo).OnComplete(() => {
                if (health <= 0) {
                    DestroyEnemy();
                }

                if (isNuke) {
                    GameManager.GameOver("Shot a Nuke");
                }
            });

        }

        private void DestroyEnemy() {
            GameManager.Score += score;
            DataManager._instance.TotalKills++;

            if (OnEnemyKilled != null)
                OnEnemyKilled(enemyType);

            //GameObject blastGO = Instantiate(blastPrefab, transform.position, transform.rotation) as GameObject;
            Blast blast = PoolManager.Instantiate("Blast", transform.position, Quaternion.identity) as Blast;
            blast.transform.localScale = new Vector3(blastScale, blastScale, blastScale);

            PoolDestroy();
            //Destroy(this.gameObject);
        }

        IEnumerator BossChangeLane(float duration) {
            yield return new WaitForSeconds(duration);
            int newLaneId;

            switch (laneId) {
                case 0:
                    newLaneId = 1;
                    break;
                case 2:
                    newLaneId = 1;
                    break;
                case 1:
                    newLaneId = Random.Range(0, 3);
                    while (newLaneId == 1) {
                        newLaneId = Random.Range(0, 3);
                    }
                    break;
                default:
                    newLaneId = 0;
                    break;
            }

            var newPosX = GameManager.GetEnemeySpawnPosition((GameManager.Lane)newLaneId).x;
            transform.position = new Vector3(newPosX, transform.position.y);

            //set LaneID
            laneId = newLaneId;

        }

        private void Update() {
            if (IsBoss) {
                if (!isCutscene) {
                    transform.Translate(-transform.up * (speed * speedScale) * Time.deltaTime);
                    // randomly transport to either of the lanes
                    if(Time.time - timeSinceSwap >= laneChangeInterval) {
                        StartCoroutine(BossChangeLane(0));
                        timeSinceSwap = Time.time;
                    }
                }
            }
            else {
                transform.Translate(-transform.up * (speed * speedScale) * Time.deltaTime);
            }
            //spriteRenderer.sortingOrder = Mathf.Abs (Mathf.RoundToInt(transform.position.y * 10)) ;
        }

        private void OnEnable() {
            GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
            Bullet.OnSplashDamage += Bullet_OnSplashDamage;
        }

        private void Bullet_OnSplashDamage(int laneId, float splashRadius, Vector2 position) {
            if (this.laneId == laneId && enemyType != LaneEnemyType.Nuke && Vector2.Distance(transform.position, position) < splashRadius) {
                ApplyDamage(2);
            }
        }

        private void OnDisable() {
            GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
            Bullet.OnSplashDamage -= Bullet_OnSplashDamage;
        }

        private void GameManager_OnGameStateChanged(GameManager.GameState gameState) {
            if (gameState == GameManager.GameState.GameOver) {

                Blast blast = PoolManager.Instantiate("Blast", transform.position, Quaternion.identity) as Blast;
                blast.transform.localScale = new Vector3(blastScale, blastScale, blastScale);

                PoolDestroy();
            }
        }

        public void PoolInstantiate(Vector3 position, Quaternion rotation) {
            health = initialHealth;
            spriteRenderer.color = Color.white;

            speedScale = Random.Range(minSpeedScale, maxSpeedScale);

            gameObject.SetActive(true);

            //Setting animator
            if (enemyType == LaneEnemyType.Human) {
                string stateName = string.Format("Human-{0}", Random.Range(1, 3).ToString("00"));
                GetComponent<Animator>().Play(stateName);
            }
            else if (enemyType == LaneEnemyType.Plane) {
                string stateName = string.Format("Plane-{0}", Random.Range(1, 3).ToString("00"));
                GetComponent<Animator>().Play(stateName);
            }
            else if (enemyType == LaneEnemyType.Bike) {
                string stateName = string.Format("Bike-{0}", Random.Range(1, 2).ToString("00"));
                GetComponent<Animator>().Play(stateName);
            }
            else if (enemyType == LaneEnemyType.Boss) {
                string stateName = string.Format("Bike-{0}", 2.ToString("00"));
                GetComponent<Animator>().Play(stateName);
            }

            transform.position = position;
            transform.rotation = rotation;
        }
        public void BossCinematicIntro() {


            // On boss spawn
            if(OnBossSpawn != null) {
                OnBossSpawn();
            }
            isCutscene = true;

            Gratification._instance.ShowBossNotification("BOSS!");

            var childIndex = (laneId == 2) ? 1 : 0;

            transform.DOMoveY(4.75f, 1.5f).SetEase(Ease.Linear).
            OnComplete(() => {
                if (true) {
                    // fade in sprite
                    transform.GetChild(childIndex).gameObject.GetComponent<SpriteRenderer>().DOFade(1, 0.25f)
                .OnComplete(() => {
                        // fade in Text
                        transform.GetChild(childIndex).GetChild(0).gameObject.GetComponent<TextMeshPro>().DOFade(1, 0.25f).OnComplete(() => {
                        BossClosure(childIndex);
                    });
                });
                }
            });
        }

        public void BossClosure(int childIndex) {
            // fade out text
            transform.GetChild(childIndex).GetChild(0).gameObject.GetComponent<TextMeshPro>().DOFade(0, 0.1f).SetDelay(3f).OnComplete(() => {
                // fade out sprite
                transform.GetChild(childIndex).gameObject.GetComponent<SpriteRenderer>().DOFade(0, 0.1f).OnComplete(() => {
                    GameManager.IsInputEnabled = true;
                    isCutscene = false;
                    timeSinceSwap = Time.time;
                });
            });
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

        public delegate void EnemyKilledHandler(LaneEnemyType enemyType);
        public static event EnemyKilledHandler OnEnemyKilled;

        public delegate void EnemyDamageHandler(float initialHealth, float health);
        public static event EnemyDamageHandler OnEnemyDamage;

        public delegate void BossSpawnHandler();
        public static event BossSpawnHandler OnBossSpawn;
    }
}
