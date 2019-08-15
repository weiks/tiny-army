using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DarkTonic.MasterAudio;

#if UNITY_ANDROID
#endif

using PlayFab;
using PlayFab.ClientModels;

using Hammerplay.Utils;

namespace Hammerplay.TinyArmy
{
    public class DataManager : MonoBehaviour
    {

        public static DataManager _instance;

        const string SAVE_NAME = "tiny_army_save";

        bool isSaving;
        bool isCloudDataLoaded = false;

        [SerializeField]
        private int saveVersion;

        [SerializeField]
        private int coins;

        [SerializeField]
        private GameObject dailyRewards;

        public GameObject DailyRewards {
            get {
                return dailyRewards;
            }

            set {
                dailyRewards = value;
            }
        }

        public int Coins {
            get { return coins; }
            set {
                coins = value;
                MasterAudio.PlaySoundAndForget("coin");
                //MenuManager._instance.TotalCoins.text = coins.ToString();
                SaveData();
            }
        }


        [Header("Debug Log")]
        [SerializeField]
        private int playerXP;

        public int PlayerXP {
            get { return playerXP; }
            set {
                playerXP = value;
            }
        }

        [SerializeField]
        private int maxXP;

        public int MaxXP {
            get { return maxXP; }
            set { maxXP = value; }
        }

        [SerializeField]
        private int xpMultiplier;

        [SerializeField]
        private int playerLevel;

        public int PlayerLevel {
            get { return playerLevel; }
            set { playerLevel = value; }
        }

        [SerializeField]
        private int highScore;

        public int HighScore {
            get { return highScore; }
            set {
                highScore = value;

            }
        }

        [SerializeField]
        private int totalKills;

        public int TotalKills {
            get { return totalKills; }
            set { totalKills = value; }
        }

        // Game Data
        [SerializeField]
        private int[] saveData;

        [SerializeField]
        private int[] loadData;

        [SerializeField]
        private bool isLogin;

        public bool IsLogin {
            get { return isLogin; }
            set { isLogin = value; }
        }

        [SerializeField]
        private int playerRankID;

        public int PlayerRankID {
            get { return playerRankID; }
            set { playerRankID = value; }
        }

        [SerializeField]
        private int gunFramesID;

        public int GunFramesID {
            get { return gunFramesID; }
            set { gunFramesID = value; }
        }

        [SerializeField]
        private int[] gunFramesStatus;

        public int[] GunFramesStatus {
            get { return gunFramesStatus; }
            set { gunFramesStatus = value; }
        }

        [SerializeField]
        private int dogTagID;

        public int DogTagID {
            get { return dogTagID; }
            set { dogTagID = value; }
        }

        [SerializeField]
        private int[] dogTagStatus;

        public int[] DogTagStatus {
            get { return dogTagStatus; }
            set { dogTagStatus = value; }
        }

        [SerializeField]
        private int playerID;

        public int PlayerID {
            get { return playerID; }
            set { playerID = value; }
        }

        [SerializeField]
        private int[] playerStatus;

        public int[] PlayerStatus {
            get { return playerStatus; }
            set { playerStatus = value; }
        }


        [SerializeField]
        private int battleFieldID;

        public int BattleFieldID {
            get { return battleFieldID; }
            set { battleFieldID = value; }
        }

        [SerializeField]
        private int[] battleFieldStatus;

        public int[] BattleFieldStatus {
            get { return battleFieldStatus; }
            set { battleFieldStatus = value; }
        }

        [Header("Save data")]
        public PlayerData playerData;

        private bool shouldDeleteData = false;

        [SerializeField]
        private User user;

        private string IOS_RATE_URL = "";

        [SerializeField]
        private string []playerPrefabID;

        [SerializeField]
        private string[] mapPrefabID;

        public string[] MapPrefabID {
            get { return mapPrefabID; }
            set { mapPrefabID = value; }
        }

        public string[] PlayerPrefabID {
            get { return playerPrefabID; }
            set { playerPrefabID = value; }
        }

        void Awake() {
            _instance = this;
        }

        private void Start() {
            CheckForUnlockedPlayers();
            // dailyRewards.gameObject.SetActive(true);
            // Get Data from the cloud.

            // loading from local
            // PlayerPrefs.DeleteAll();
        }

        private void CheckForUnlockedPlayers() {
            for (int i = 1; i < playerPrefabID.Length; i++) {
                if (POQBridge.IsInInventory(playerPrefabID[i])) {
                    PlayerStatus[i] = 1;
                }
            }

            for (int i = 1; i < mapPrefabID.Length; i++) {
                if (POQBridge.IsInInventory(mapPrefabID[i])) {
                    BattleFieldStatus[i] = 1;
                }
            }

        }


        public void UpdateUserData() {
#if UNITY_ANDROID
            //if (SocialController._instance.playfabID != "") {
            //    PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() {
            //        Data = new Dictionary<string, string>() {
            //        {"SaveData", JsonUtil.DataToString<PlayerData>(playerData)}
            //    }
            //    },
            //result => {
            //    Debug.Log("Successfully updated user data");
            //},
            //error => {
            //    Debug.Log("Got error setting user data Ancestor to Arthur");
            //    Debug.Log(error.GenerateErrorReport());
            //});

            //    Debug.LogError(JsonUtil.DataToString<PlayerData>(playerData));
            //}
#endif
        }

        public void RetriveUserData() {
#if UNITY_ANDROID
            //if (SocialController._instance.playfabID != "") {
            //    PlayFabClientAPI.GetUserData(new GetUserDataRequest() {
            //        PlayFabId = SocialController._instance.playfabID,
            //        Keys = null
            //    }, result => {
            //        Debug.Log("Got user data:");
            //        if (result.Data == null || !result.Data.ContainsKey("SaveData")) {
            //            Debug.Log("No Save Data");
            //            PlayerPrefs.SetString(SAVE_NAME, JsonUtil.DataToString<PlayerData>(playerData));
            //            LoadFromPlayerPrefs();
            //            UpdateUserData();
            //        }
            //        else {
            //            // Check Conflict
            //            Debug.Log("SaveData: " + result.Data["SaveData"].Value);

            //            // cloud data = playerdata
            //            playerData = JsonUtil.StringToPlayerData(result.Data["SaveData"].Value);


            //            // local data is playerprefs
            //            PlayerData localdata = JsonUtil.StringToPlayerData(PlayerPrefs.GetString(SAVE_NAME));

            //            Debug.LogError("Player data: " + JsonUtil.DataToString<PlayerData>(playerData) + "  LocalData: " + JsonUtil.DataToString<PlayerData>(localdata));

            //            PlayerPrefs.SetString(SAVE_NAME, JsonUtil.DataToString<PlayerData>(CheckConflict(localdata, playerData)));
            //            LoadFromPlayerPrefs();
            //        }
            //    }, (error) => {
            //        Debug.Log("Got error retrieving user data:");
            //        Debug.Log(error.GenerateErrorReport());
            //    });
            //}
#endif
        }

        private PlayerData CheckConflict(PlayerData localData, PlayerData cloudData) {

            if (localData.GameData.Count == 0) {
                Debug.Log("Overide local Data");
                return playerData;
            }
            else if (localData.GameData[(int)GameDataType.PlayerXP] > cloudData.GameData[(int)GameDataType.PlayerXP]) {
                UpdateUserData();
                Debug.Log("Overide Remote Data");
                return localData;
            }
            else {
                int local = 0, cloud = 0;

                foreach (int j in localData.BattlefieldData) if (j == 1) local++;
                foreach (int j in localData.CharacterData) if (j == 1) local++;
                foreach (int j in localData.DogTagData) if (j == 1) local++;
                foreach (int j in localData.GunFrameData) if (j == 1) local++;
                if (localData.ShouldPlayAds == 0) local++;

                foreach (int j in cloudData.BattlefieldData) if (j == 1) cloud++;
                foreach (int j in cloudData.CharacterData) if (j == 1) cloud++;
                foreach (int j in cloudData.DogTagData) if (j == 1) cloud++;
                foreach (int j in cloudData.GunFrameData) if (j == 1) cloud++;
                if (cloudData.ShouldPlayAds == 0) cloud++;

                if (local > cloud) {
                    Debug.Log("Overide Remote Data");
                    return localData;
                }
                else {
                    Debug.Log("Overide Local Data");
                    return cloudData;
                }
            }
        }

        public void LoadFromPlayerPrefs() {
            if (PlayerPrefs.GetString(SAVE_NAME) != string.Empty) {
                LoadData();
                LoadPlayerData();
                LoadBattlefieldData();
                LoadDogTagData();
                LoadGunFramesData();
                //AdManager.Instance.ShouldPlayAds = JsonUtil.StringToPlayerData(PlayerPrefs.GetString(SAVE_NAME)).ShouldPlayAds;
                ChangeServerDataEvent();
            }
        }

        public int GetAdStatus() {
            return JsonUtil.StringToPlayerData(PlayerPrefs.GetString(SAVE_NAME)).ShouldPlayAds;
        }

        public bool CheckItemStatus(GameDataType gameDataType, int id) {
            bool purchase = false;
            if (gameDataType == GameDataType.GunFramesID) {
                if (gunFramesStatus[id] == 0) {
                    purchase = false;
                }
                else {
                    purchase = true;
                }
            }

            if (gameDataType == GameDataType.DogTagID) {
                if (dogTagStatus[id] == 0) {
                    purchase = false;
                }
                else {
                    purchase = true;
                }
            }

            if (gameDataType == GameDataType.BattleFieldID) {
                if (battleFieldStatus[id] == 0) {
                    purchase = false;
                }
                else {
                    purchase = true;
                }
            }

            if (gameDataType == GameDataType.PlayerID) {
                if (playerStatus[id] == 0) {
                    purchase = false;
                }
                else {
                    purchase = true;
                }
            }

            return purchase;
        }

        public void UnlockItem(GameDataType gameDataType, int id, int price) {
            switch (gameDataType) {
                case GameDataType.GunFramesID:
                    gunFramesStatus[id] = 1;
                    //Coins -= price;
                    break;
                case GameDataType.DogTagID:
                    dogTagStatus[id] = 1;
                    //Coins -= price;
                    break;
                case GameDataType.BattleFieldID:
                    battleFieldStatus[id] = 1;
                    //Coins -= price;
                    break;
                case GameDataType.PlayerID:
                    playerStatus[id] = 1;
                    //Coins -= price;
                    break;
                default:
                    break;
            }
            SaveData();
        }

        public void UpdateVariables(GameDataType gameDataType, int id) {
            switch (gameDataType) {
                case GameDataType.PlayerXP:
                    break;
                case GameDataType.MaxXP:
                    break;
                case GameDataType.PlayerLevel:
                    break;
                case GameDataType.TotalKills:
                    break;
                case GameDataType.HighScore:
                    break;
                case GameDataType.Coins:
                    Coins += id;
                    break;
                case GameDataType.GunFramesID:
                    gunFramesID = id;
                    break;
                case GameDataType.PlayerRankID:
                    playerRankID = id;
                    break;
                case GameDataType.DogTagID:
                    dogTagID = id;
                    break;
                default:
                    break;
            }
            SaveData();
        }

        public void RankUp() {
            maxXP += xpMultiplier;
            playerXP = 0;
            playerLevel += 1;
            //Coins += 100;
        }

        public void ClearData() {
            PlayerPrefs.DeleteAll();
        }

        public void LoadLocal() {
            LoadData();
        }

        public void SaveLocal() {
            saveData = new int[] { playerXP, maxXP, playerLevel, totalKills, highScore, coins, gunFramesID, playerRankID, dogTagID, battleFieldID, playerID };

            SavePlayerData();
            SaveBattlefieldData();
            SaveDogTagData();
            SaveGunFramesData();

            UpdatePlayerData();
            // Push to firebase..
            Debug.Log("Saved local!");
        }

        public void UpdatePlayerData() {
            playerData.GameData = PlayerData.IntToList(saveData);
            playerData.CharacterData = PlayerData.IntToList(savePlayerStatus);
            playerData.BattlefieldData = PlayerData.IntToList(saveBattlefieldStatus);
            playerData.DogTagData = PlayerData.IntToList(saveDogTagStatus);
            playerData.GunFrameData = PlayerData.IntToList(saveGunFramesStatus);

            PlayerPrefs.SetString(SAVE_NAME, JsonUtil.DataToString<PlayerData>(playerData));
        }

        public void SaveData() {
            SaveLocal();
            UpdateUserData();
        }

        public void LoadData() {
            loadData = JsonUtil.StringToPlayerData(PlayerPrefs.GetString(SAVE_NAME)).GameData.ToArray();

            if (loadData.Length != 0) {
                playerXP = loadData[((int)GameDataType.PlayerXP)];
                maxXP = loadData[((int)GameDataType.MaxXP)];
                playerLevel = loadData[((int)GameDataType.PlayerLevel)];
                totalKills = loadData[((int)GameDataType.TotalKills)];
                highScore = loadData[((int)GameDataType.HighScore)];
                //coins = loadData[((int)GameDataType.Coins)];
                gunFramesID = loadData[((int)GameDataType.GunFramesID)];
                playerRankID = loadData[((int)GameDataType.PlayerRankID)];
                dogTagID = loadData[((int)GameDataType.DogTagID)];
                battleFieldID = loadData[((int)GameDataType.BattleFieldID)];
                playerID = loadData[((int)GameDataType.PlayerID)];
            }
            else {
                SaveData();
                LoadData();
            }
        }

        [SerializeField]
        private int[] savePlayerStatus;
        private void SavePlayerData() {
            savePlayerStatus = new int[] { playerStatus[0], playerStatus[1], playerStatus[2], playerStatus[3] };
            UpdatePlayerData();
            //PlayerPrefsX.SetIntArray("TangoCharlie_PlayerData_0" + saveVersion.ToString(), playerStatus);
        }

        [SerializeField]
        private int[] loadplayerStatus;
        private void LoadPlayerData() {
            loadplayerStatus = JsonUtil.StringToPlayerData(PlayerPrefs.GetString(SAVE_NAME)).CharacterData.ToArray();

            //loadplayerStatus = PlayerPrefsX.GetIntArray("TangoCharlie_PlayerData_0" + saveVersion.ToString());
            if (loadplayerStatus.Length != 0) {
                playerStatus[0] = loadplayerStatus[0];
                playerStatus[1] = loadplayerStatus[1];
                playerStatus[2] = loadplayerStatus[2];
                playerStatus[3] = loadplayerStatus[3];
            }
            else {
                SavePlayerData();
                LoadPlayerData();
            }
        }

        [SerializeField]
        private int[] saveBattlefieldStatus;
        private void SaveBattlefieldData() {
            saveBattlefieldStatus = new int[] { battleFieldStatus[0], battleFieldStatus[1], battleFieldStatus[2], battleFieldStatus[3] };
            UpdatePlayerData();
            // PlayerPrefsX.SetIntArray("TangoCharlie_BattleField_0" + saveVersion.ToString(), battleFieldStatus);
        }

        [SerializeField]
        private int[] loadBattlefieldStatus;
        private void LoadBattlefieldData() {

            loadBattlefieldStatus = JsonUtil.StringToPlayerData(PlayerPrefs.GetString(SAVE_NAME)).BattlefieldData.ToArray();

            //loadBattlefieldStatus = PlayerPrefsX.GetIntArray("TangoCharlie_BattleField_0" + saveVersion.ToString());
            if (loadBattlefieldStatus.Length != 0) {
                battleFieldStatus[0] = loadBattlefieldStatus[0];
                battleFieldStatus[1] = loadBattlefieldStatus[1];
                battleFieldStatus[2] = loadBattlefieldStatus[2];
                battleFieldStatus[3] = loadBattlefieldStatus[3];
            }
            else {
                SaveBattlefieldData();
                LoadBattlefieldData();
            }
        }

        [SerializeField]
        private int[] saveDogTagStatus;
        private void SaveDogTagData() {
            saveDogTagStatus = new int[] { dogTagStatus[0], dogTagStatus[1], dogTagStatus[2], dogTagStatus[3] };
            UpdatePlayerData();
            //PlayerPrefsX.SetIntArray("TangoCharlie_DogTag_0" + saveVersion.ToString(), dogTagStatus);
        }

        [SerializeField]
        private int[] loadDogTagStatus;
        private void LoadDogTagData() {
            loadDogTagStatus = JsonUtil.StringToPlayerData(PlayerPrefs.GetString(SAVE_NAME)).DogTagData.ToArray();
            //loadDogTagStatus = PlayerPrefsX.GetIntArray("TangoCharlie_DogTag_0" + saveVersion.ToString());
            if (loadDogTagStatus.Length != 0) {
                dogTagStatus[0] = loadDogTagStatus[0];
                dogTagStatus[1] = loadDogTagStatus[1];
                dogTagStatus[2] = loadDogTagStatus[2];
                dogTagStatus[3] = loadDogTagStatus[3];
            }
            else {
                SaveDogTagData();
                LoadDogTagData();
            }
        }

        [SerializeField]
        private int[] saveGunFramesStatus;
        private void SaveGunFramesData() {
            saveGunFramesStatus = new int[] { gunFramesStatus[0], gunFramesStatus[1], gunFramesStatus[2], gunFramesStatus[3], gunFramesStatus[4] };
            UpdatePlayerData();
            // PlayerPrefsX.SetIntArray("TangoCharlie_GunFrames_0" + saveVersion.ToString(), gunFramesStatus);
        }

        [SerializeField]
        private int[] loadGunFramesStatus;
        private void LoadGunFramesData() {
            loadGunFramesStatus = JsonUtil.StringToPlayerData(PlayerPrefs.GetString(SAVE_NAME)).GunFrameData.ToArray();
            //loadGunFramesStatus = PlayerPrefsX.GetIntArray("TangoCharlie_GunFrames_0" + saveVersion.ToString());
            if (loadGunFramesStatus.Length != 0) {
                gunFramesStatus[0] = loadGunFramesStatus[0];
                gunFramesStatus[1] = loadGunFramesStatus[1];
                gunFramesStatus[2] = loadGunFramesStatus[2];
                gunFramesStatus[3] = loadGunFramesStatus[3];
                gunFramesStatus[4] = loadGunFramesStatus[4];
            }
            else {
                SaveGunFramesData();
                LoadGunFramesData();
            }
        }

        public void CheckHighScore(int score) {
            if (score > highScore) {
                highScore = score;
                //SocialController._instance.AddScoreToLeaderboard(GPGSIds.leaderboard_leaderboard, highScore);
            }
        }

        public static void ChangeServerDataEvent() {
            if (OnServerDataEvent != null)
                OnServerDataEvent();
        }

        public delegate void ServerDataEvent();
        public static event ServerDataEvent OnServerDataEvent;
    }

    public enum GameDataType
    {
        PlayerXP,
        MaxXP,
        PlayerLevel,
        TotalKills,
        HighScore,
        Coins,
        GunFramesID,
        PlayerRankID,
        DogTagID,
        BattleFieldID,
        PlayerID
    }

    #region Player Data
    // Player data class for cloud storage
    [System.Serializable]
    public class PlayerData
    {
        private List<int> gameData = new List<int>();
        private List<int> gunFrameData = new List<int>();
        private List<int> dogTagData = new List<int>();
        private List<int> battlefieldData = new List<int>();
        private List<int> characterData = new List<int>();
        private int shouldPlayAds = 1;

        public static List<int> IntToList(int[] array) {
            List<int> list = new List<int>();
            for (int i = 0; i < array.Length; i++)
                list.Add(array[i]);

            return list;
        }
        public List<int> GameData {
            get {
                return gameData;
            }

            set {
                gameData = value;
            }
        }

        public List<int> GunFrameData {
            get {
                return gunFrameData;
            }

            set {
                gunFrameData = value;
            }
        }

        public List<int> DogTagData {
            get {
                return dogTagData;
            }

            set {
                dogTagData = value;
            }
        }

        public List<int> BattlefieldData {
            get {
                return battlefieldData;
            }

            set {
                battlefieldData = value;
            }
        }

        public List<int> CharacterData {
            get {
                return characterData;
            }

            set {
                characterData = value;
            }
        }

        public int ShouldPlayAds {
            get {
                return shouldPlayAds;
            }

            set {
                shouldPlayAds = value;
                if (shouldPlayAds == 0) {
                    //AdManager.Instance.HideBannerAd();
                }
            }
        }
    }
    #endregion

    #region User
    [SerializeField]
    public class User
    {
        public string userName;
        public int userScore;

        public User() {
            userName = "Harish";// PlayGamesPlatform.Instance.GetUserDisplayName();
            userScore = DataManager._instance.HighScore;
        }
    }
    #endregion
}
