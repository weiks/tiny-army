using UnityEngine;
using UnityEngine.UI;

public class HowToPlayPanel : MonoBehaviour {

    [SerializeField]
    private int tutorialIndex;

    [SerializeField]
    private Image tutorialScreen;

    [SerializeField]
    private Button previousButton, nextButton;

    [SerializeField]
    private Sprite[] tutorialScreens;

    private void OnEnable () {
        tutorialIndex = 0;
        previousButton.interactable = false;
        nextButton.interactable = true;
        tutorialScreen.sprite = tutorialScreens[tutorialIndex];
        previousButton.onClick.AddListener (PreviousScreen);
        nextButton.onClick.AddListener (NextScreen);
        Debug.Log (tutorialScreens.Length);
    }

    private void OnDisable () {
        previousButton.onClick.RemoveAllListeners ();
        nextButton.onClick.RemoveAllListeners ();
    }

    private void PreviousScreen () {
        if (tutorialIndex == 0) {
            previousButton.interactable = false;
        } else {
            tutorialIndex -= 1;
            if (tutorialIndex == 0) {
                previousButton.interactable = false;
            }

            if (tutorialIndex < tutorialScreens.Length - 1) {
                nextButton.interactable = true;
            }
            tutorialScreen.sprite = tutorialScreens[tutorialIndex];
        }
    }

    private void NextScreen () {
        if (tutorialIndex == tutorialScreens.Length - 1) {
            nextButton.interactable = false;
        } else {
            tutorialIndex += 1;
            if (tutorialIndex == tutorialScreens.Length - 1) {
                nextButton.interactable = false;
            }

            if (tutorialIndex > 0) {
                previousButton.interactable = true;
            }
            tutorialScreen.sprite = tutorialScreens[tutorialIndex];
        }
    }

}