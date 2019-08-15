using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hammerplay.Utils;

namespace Hammerplay.TinyArmy
{

    public class LevelManager : Singleton<LevelManager>
    {
        public static int TRIES_TILL_AD = 2;

        public bool isContinue = false;
        public bool rewardOnce = true;
        public bool isBeatMyBestMode = false;

        public bool menu = false;
        public bool restart = false;
        public bool isScoreDirty = false;

        public int retriesTillAd = TRIES_TILL_AD;

        public int prizePool;

        public int playerID, battleFieldID;

        public int score;
        public int highScore;

        protected override void Awake() {
            _instance = this;
        }

        private void Start() {
            highScore = DataManager._instance.HighScore;
        }

    }
}
