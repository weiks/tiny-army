using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;

public class AboutUs : MonoBehaviour {

    [SerializeField]
    private Button backButton;

    private void OnEnable () {
        backButton.onClick.AddListener(BackButton);
    }

    private void OnDisable() {
        backButton.onClick.RemoveAllListeners();
    }

    private void Update() {
        if (Input.GetKeyDown("escape")) {
            BackButton();
        }
    }

    private void BackButton() {
        gameObject.SetActive(false);
        MasterAudio.PlaySound("click");
    }
}
