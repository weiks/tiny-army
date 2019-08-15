using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;

public class ShopGratification : MonoBehaviour {
    [SerializeField]
    private Image productIcon;

    [SerializeField]
    private Text productLabel;

    [SerializeField]
    private Button closeButton;

    private void OnEnable() {
        closeButton.onClick.AddListener(()=> {
            gameObject.SetActive(false);
            MasterAudio.PlaySound("click");
        });
    }

    private void OnDisable() {
        closeButton.onClick.RemoveAllListeners();
    }

    public void ShowGratification(Sprite icon, string label) {
        productIcon.sprite = icon;
        productLabel.text = label;
    }
}
