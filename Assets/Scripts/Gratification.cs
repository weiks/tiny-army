using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hammerplay.Utils
{
    public class Gratification : MonoBehaviour
    {

        public static Gratification _instance;

        [SerializeField]
        private Color WeaponTextColor, BossTextColor;

        [SerializeField]
        private Text notificationText;

        public Text NotificationText {
            get { return notificationText; }
            set { notificationText = value; }
        }


        [SerializeField]
        private Image image;

        void Awake() {
            _instance = this;

            transform.localScale = Vector3.zero;
        }
        private void ShowMessage(string message) {
            notificationText.text = message;
            gameObject.transform.DOScale(Vector3.one, 0.35f).OnComplete(HideMessage);
        }

        public void ShowWeaponNotification(string message) {
            notificationText.color = WeaponTextColor;
            ShowMessage(message);
        }

        public void ShowBossNotification(string message) {
            notificationText.color = BossTextColor;
            ShowMessage(message);
        }

        void HideMessage() {
            gameObject.transform.DOScale(Vector3.zero, 0.1f).SetDelay(1f).OnComplete(() => {
            });
        }
    }

}