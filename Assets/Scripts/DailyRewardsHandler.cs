using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NiobiumStudios;
using Hammerplay.TinyArmy;

public class DailyRewardsHandler : MonoBehaviour {

    void OnEnable() {
        DailyRewards.instance.onClaimPrize += OnClaimPrizeDailyRewards;
    }

    void OnDisable() {
        DailyRewards.instance.onClaimPrize -= OnClaimPrizeDailyRewards;
    }

    public void OnClaimPrizeDailyRewards(int day) {
        //This returns a Reward object
        Reward myReward = DailyRewards.instance.GetReward(day);
        //DataManager._instance.Coins += myReward.reward;
    }
}
