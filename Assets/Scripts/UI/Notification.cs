using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;
using Hammerplay.TinyArmy;

public class Notification : MonoBehaviour {

    [SerializeField]
    private Button confirmButton;

    [SerializeField]
    private Button quitButton;

    [SerializeField]
    private Text _message;

    private void OnEnable() {
        confirmButton.onClick.AddListener(BackButton);
        quitButton.onClick.AddListener(BackButton);

    }

    private void OnDisable() {
        confirmButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();

    }

    public void SetNotificationMessage(string message) {
        _message.text = message;

    }

    public void SetNotificationMessage(string message, OnNotificationClosed closedEvent) {
        _message.text = message;
        NotificationClosedEvent += closedEvent;
    }

    private void Update() {
        if (Input.GetKeyDown("escape")) {
            BackButton();
        }
    }

    private void BackButton() {
        gameObject.SetActive(false);
        MasterAudio.PlaySound("click");

        if (NotificationClosedEvent != null) {
            NotificationClosedEvent();
            foreach (var del in NotificationClosedEvent.GetInvocationList())
                NotificationClosedEvent -= del as OnNotificationClosed;
        }

    }
    public delegate void OnNotificationClosed();
    public static event OnNotificationClosed NotificationClosedEvent;
}
