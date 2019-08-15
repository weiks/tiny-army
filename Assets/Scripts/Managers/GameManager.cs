using DG.Tweening;
using Hammerplay.Utils;

using UnityEngine;
using DarkTonic.MasterAudio;
using QuartersSDK;
using System.Collections;
using System.Collections.Generic;
using System;
using Doozy.Engine;

namespace Hammerplay.TinyArmy
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager Instance {
            get { return _instance; }
        }
        private void Awake() {
            _instance = this;
            highScore = PlayerPrefs.GetInt("highScore", 0);
            Application.targetFrameRate = 60;
        }

        public float testFloat;

        [SerializeField]
        private Wave wave = new Wave();

        [SerializeField]
        private GameState startupGameState;

        [SerializeField]
        private Sprite[] playerSprites;

        public static bool canSpawnPowerUp = false;

        public static Sprite GetPlayerSprite() {
            return _instance.playerSprites[_instance.soliderIndex - 1];
        }

        [SerializeField]
        private int soliderIndex = 1;

        public static int SoliderIndex {
            get { return _instance.soliderIndex; }
            set { _instance.soliderIndex = value; }
        }

        [SerializeField]
        private int mapIndex = 1;

        [SerializeField]
        private bool isInputEnabled = true;

        public static int MapIndex {
            get { return _instance.mapIndex; }
            set { _instance.mapIndex = value; }
        }

        // private TinyArmySessionInfo sessionInfo;

        // public static TinyArmySessionInfo SessionInfo {
        // 	get { return _instance.sessionInfo; }
        // }
        public void GameManager_OnBalanceSuccess(QuartersSDK.User.Account.Balance balance) {

        }

        private void Start() {

            GameEventMessage.SendEvent("OnGameLoaded");
            MasterAudio.PlaySound("MainMenu_BGM");

            // sessionInfo = Session.Instance.GetSessionInfo<TinyArmySessionInfo>();
            if (LevelManager.Instance.menu == true) {
                //POQUI.Instance.RefreshQuartersBalance();
                LevelManager.Instance.menu = false;
                //AdManager.Instance.DisplayBannerAd();
                //MenuManager._instance.MainMenuPanel.SetActive(false);
                ChangeGameState(GameState.StartMenu);
                //wMenuManager._instance.EnterGame();
            }
            else if (LevelManager.Instance.restart == true) {
                //POQUI.Instance.RefreshQuartersBalance();

                LevelManager.Instance.restart = false;

                DataManager._instance.PlayerID = LevelManager.Instance.playerID;
                DataManager._instance.BattleFieldID = LevelManager.Instance.battleFieldID;

                mapIndex = DataManager._instance.BattleFieldID;
                soliderIndex = DataManager._instance.PlayerID;

                ChangeGameState(GameState.Start);
            }
            else if (LevelManager.Instance.isContinue == true) {
                //POQUI.Instance.RefreshQuartersBalance();

                LevelManager.Instance.isContinue = false;
                // Load the level and the player status.
                DataManager._instance.PlayerID = LevelManager.Instance.playerID;
                DataManager._instance.BattleFieldID = LevelManager.Instance.battleFieldID;

                mapIndex = DataManager._instance.BattleFieldID;
                soliderIndex = DataManager._instance.PlayerID;
                // set score to the previous score
                Score = LevelManager.Instance.score;
                // watch ads here.
                ChangeGameState(GameState.Start);
            }
            else {
                //AdManager.Instance.DisplayBannerAd();
                ChangeGameState(startupGameState);
            }

            if (POQBridge.IsContinuedGame) {
                SoliderIndex = PlayerPrefs.GetInt("SoldierIndex");
                MapIndex = PlayerPrefs.GetInt("MapIndex");
                Score = PlayerPrefs.GetInt("CurrentScore");
            } else {
                PlayerPrefs.SetInt("CurrentScore", 0);
                SoliderIndex = 1;
                MapIndex = 1;
            }
        }

        private void OnEnable() {
            // Session.Instance.GameEnd += Instance_GameEnd;
            Enemy.OnEnemyKilled += Enemy_OnEnemyKilled;
        }

        private void OnDisable() {
            // Session.Instance.GameEnd -= Instance_GameEnd;
            Enemy.OnEnemyKilled -= Enemy_OnEnemyKilled;
        }

        internal static void ExitGame() {
        }

        private void Enemy_OnEnemyKilled(LaneEnemyType enemyType) {
            humanKills = (enemyType == LaneEnemyType.Human) ? humanKills + 1 : humanKills;
            bikeKills = (enemyType == LaneEnemyType.Bike) ? bikeKills + 1 : bikeKills;
            planeKills = (enemyType == LaneEnemyType.Plane) ? planeKills + 1 : planeKills;
            nukeKills = (enemyType == LaneEnemyType.Nuke) ? nukeKills + 1 : nukeKills;
        }

        private bool isTimeOutFromMPL;

        private bool shouldSubmitScore;

        private IEnumerator IChangeGameState(GameState gameState) {
            yield return new WaitForSeconds(0.01f);
            ChangeGameState(gameState);
        }

        [SerializeField]
        private PlayerUnit playerUnitPrefab;

        [SerializeField]
        private float[] laneSpawnPoints;

        [ContextMenu("Spawn Player Units")]
        private void SpawnPlayerUnits() {
            StartCoroutine(ISpawnPlayerUnits(false));
        }

        private void SpawnGraveStones() {
            StartCoroutine(ISpawnPlayerUnits(true));
        }

        private IEnumerator ISpawnPlayerUnits(bool isRip) {
            PoolManager.ClearOnScreen("PlayerUnit");

            for (int i = 0; i < 3; i++) {
                Vector3 playerUnitPositon = GetPlayerSpawnPosition((Lane)i);

                yield return new WaitForSeconds(0.25f);

                IPoolable playerUnit = PoolManager.Instantiate(isRip ? "GraveStone" : "PlayerUnit", playerUnitPositon, Quaternion.identity);
                playerUnit.GetGameObject().transform.position = new Vector3(playerUnit.GetGameObject().transform.position.x, playerUnit.GetGameObject().transform.position.y - 3, playerUnit.GetGameObject().transform.position.x);
                playerUnit.GetGameObject().transform.DOMove(playerUnitPositon, 0.3f).OnComplete(() => {
                    if (i == 3 && !isRip) {
                        InGameMode();
                    }
                });

                if (!isRip) {
                    playerUnit.GetGameObject().SendMessage("SetLaneId", i);
                }
            }
        }

        [ContextMenu("Restart Game")]
        public static void RestartGame() {
            MasterAudio.StopAllOfSound("InGame_BGM");
            _instance.ChangeGameState(GameState.Start);
        }

        [ContextMenu("Pause Game")]
        public static void PauseGame() {
            _instance.ChangeGameState(GameState.Paused);
        }

        [ContextMenu("Resume Game")]
        public static void ResumeGame() {
            _instance.ChangeGameState(GameState.Resume);
        }

        public static void InGameMode() {
            _instance.ChangeGameState(GameState.InGame);
        }

        public static void ShowGameOverPanel(string reason) {
            MasterAudio.StopAllOfSound("InGame_BGM");
            _instance.gameOverReason = reason;
            _instance.shouldSubmitScore = true;

            //Set score in Level manager
            LevelManager.Instance.score = Score;
            _instance.ChangeGameState(GameState.GameOver);
        }
        public static void GameOver(string reason) {
            // Call a tween.
            if (LevelManager.Instance.rewardOnce == true) {
                //Enable the PreGameOVeRPanel
                //MenuManager._instance.PreGameOverPanel.SetActive(true);
                ShowGameOverPanel(reason);
                //PreGameOverPanel.Instance.ShowPreGameOverPanel();
                //MenuManager._instance.PreGameOverPanel.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutBounce).SetDelay(1.5f);
            }
            else {
                ShowGameOverPanel(reason);
            }

        }

        private string gameOverReason;

        public static string GameOverReason {
            get { return _instance.gameOverReason; }
        }

        private int score = 0;

        public static int Score {
            set {
                int currentScore = _instance.score;
                _instance.score = value;
                if (_instance.score > _instance.highScore) {
                    _instance.highScore = _instance.score;
                    PlayerPrefs.SetInt("highScore", _instance.highScore);
                }

                if (OnScoreChanged != null)
                    OnScoreChanged(_instance.score);
            }
            get { return _instance.score; }
        }

        public delegate void ScoreHandler(int score);
        public static event ScoreHandler OnScoreChanged;

        private int highScore = 0;

        public static int HighScore {
            get { return _instance.highScore; }
        }

        private long gameStartTime, gameEndTime;
        private int humanKills, bikeKills, planeKills, nukeKills;

        public static Vector3 GetEnemeySpawnPosition(Lane lane) {
            Vector3 returnVector3 = new Vector3();
            returnVector3 = Camera.main.ViewportToWorldPoint(Vector3.one);
            returnVector3.x = _instance.laneSpawnPoints[(int)lane];

            returnVector3.z = 0;

            return returnVector3;
        }

        public static Vector3 GetPlayerSpawnPosition(Lane lane) {
            Vector3 returnVector3 = new Vector3();
            returnVector3 = Camera.main.ViewportToWorldPoint(Vector3.zero);
            returnVector3.x = _instance.laneSpawnPoints[(int)lane];
            returnVector3.y += 0.3f;
            returnVector3.z = 0;

            return returnVector3;
        }

        [SerializeField]
        private GameState currentGameState;

        [SerializeField]
        private List<GameObject> backgrounds;

        [SerializeField]
        private float powerupDuration = 8;

        public static float PowerupDuration {
            get { return _instance.powerupDuration; }
        }

        public static GameState CurrentGameState {
            get { return _instance.currentGameState; }
        }

        public static bool IsInputEnabled {
            get {
                return _instance.isInputEnabled;
            }

            set {
                _instance.isInputEnabled = value;
            }
        }

        DateTime currentDate;
        private void OnApplicationPause(bool pause) {
            if (pause && currentGameState == GameState.InGame) {
                PauseGame();
            }
            else {
                Debug.Log("Back in the game");
            }
        }

        private void CallEndGameBanner() {
            Debug.LogError("CALL END GAME BANNER");
            LevelManager.Instance.isBeatMyBestMode = false;

            currentDate = DateTime.Now;
            gameEndTime = currentDate.Ticks;

            SpawnGraveStones();
            GameCamera.CameraShake(0.5f, 0.4f);

        }

        public void GameOver() {
            if (LevelManager.Instance.rewardOnce == false) {
                Debug.LogError("REWARD ONCE : FALSE");
                CallEndGameBanner();
            }
            else {
                return;
            }
        }

        public void ChangeGameState(GameState gameState) {

            if (gameState == currentGameState) {
                if (gameState == GameState.GameOver && !LevelManager.Instance.rewardOnce) {
                    // Do nothing.//
                }
                else {
                    return;
                }
            }

            currentGameState = gameState;

            if (OnGameStateChanged != null) {
                OnGameStateChanged(gameState);
            }


            switch (gameState) {
                case GameState.Start:
                    AdManager.HideBannerAd();
                    currentDate = DateTime.Now;
                    gameStartTime = currentDate.Ticks;

                    backgrounds.ForEach(x => x.gameObject.SetActive(false));
                    backgrounds[mapIndex - 1].SetActive(true); //UnityEngine.Random.Range(0, backgrounds.Count)
                    PoolManager.ClearOnScreen("GraveStone");

                    _instance.SpawnPlayerUnits();

                    if (LevelManager.Instance.isScoreDirty == true) {
                        score = 0;
                        LevelManager.Instance.isScoreDirty = false;
                    }
                    MasterAudio.StopAllOfSound("MainMenu_BGM");
                    MasterAudio.PlaySound("InGame_BGM");
                    break;

                case GameState.Paused:
                    AdManager.DisplayBannerAd();
                    Time.timeScale = 0;
                    break;

                case GameState.Resume:
                    AdManager.HideBannerAd();
                    break;

                case GameState.InGame:
                    AdManager.HideBannerAd();
                    Time.timeScale = 1;
                    break;

                case GameState.GameOver:
                    PlayerPrefs.SetInt("CurrentScore", Score);
                    //Debug.LogError("GameOver Event called : rewardOnce : true");
                    if (LevelManager.Instance.rewardOnce == false) {
                       //Debug.LogError("GameOver Event called : rewardOnce : false");
                        GameOver();
                        break;
                    }
                    break;
            }
        }

        public delegate void GameStateHandler(GameState gameState);
        public static event GameStateHandler OnGameStateChanged;

        public enum Lane { Lane1, Lane2, Lane3 }
        public enum GameState { StartMenu, Start, InGame, Paused, Resume, GameOver }
    }
}

