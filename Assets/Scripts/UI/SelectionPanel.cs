using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;
using NiobiumStudios;
using DG.Tweening;
using Doozy.Engine.UI;

namespace Hammerplay.TinyArmy {
    public class SelectionPanel : MonoBehaviour {

        [SerializeField]
        private Button battleButton;
        

        private void OnEnable() {
            battleButton.onClick.AddListener(StartGame);
        }

        private void OnDisable() {
            battleButton.onClick.RemoveAllListeners();
        }

        private void Start() {
            // dailyRewardObject.gameObject.SetActive(true);
        }


        private void StartGame() {
            if (DataManager._instance.BattleFieldStatus[GameManager.MapIndex - 1] == 0 || DataManager._instance.PlayerStatus[GameManager.SoliderIndex - 1] == 0) {
                UIPopup notification = UIPopup.GetPopup("YesAndNo");
                //
                notification.Data.SetLabelsTexts("Alert", "You need to purchase this item from the shop to use it in game");
                notification.Data.SetButtonsLabels("Ok", "");
                notification.Data.SetButtonsCallbacks(() => { notification.Hide(); }, null);
                notification.Data.Buttons[1].gameObject.SetActive(false);

                notification.Show();
            }

            if (DataManager._instance.BattleFieldStatus[GameManager.MapIndex - 1] == 1 && DataManager._instance.PlayerStatus[GameManager.SoliderIndex - 1] == 1) {
                PlayerPrefs.SetInt("SoldierIndex", GameManager.SoliderIndex);
                PlayerPrefs.SetInt("MapIndex", GameManager.MapIndex);
                GameManager.RestartGame();
                gameObject.SetActive(false);
            }
        }

        private void OpenPanel(PanelType panelType) {
            MasterAudio.PlaySound("click");
        }
    }

    public enum PanelType {
        PlayerProfile,
        AboutUs,
        CashStore
    }
}
