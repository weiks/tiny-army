using System.Collections;
using System.Collections.Generic;
using Doozy.Engine;
using Hammerplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestGameOver : MonoBehaviour {

	[SerializeField]
	private Button gameOverTestButton;

	private void OnEnable () {
		gameOverTestButton.onClick.AddListener (TestGameOverMethod);
	}

	private void OnDisable () {
		gameOverTestButton.onClick.RemoveAllListeners ();
	}

	private IEnumerator Start () {
		yield return new WaitForSeconds (2);
		GameEventMessage.SendEvent ("OnGameLoaded");
	}

	[ContextMenu ("Test Game Over")]
	public void TestGameOverMethod () {
		AdManager.DisplayInterstitialAd ();
		POQBridge.StartGameOver ("2541", ReturnToMainMenu, ContinueWithGame, ContinueWithGame, ContinueFailed);
	}

	private void ReturnToMainMenu () {
		Debug.Log ("Return to main menu");
		GameEventMessage.SendEvent ("MainMenu");
		SceneManager.LoadScene ("Empty");
	}

	private void ContinueWithGame () {
		Debug.Log ("Continuing with game");
	}

	private void ContinueFailed () {
		Debug.Log ("Continue failed");
		POQBridge.StartGameOver ("2541", ReturnToMainMenu, ContinueWithGame, ContinueWithGame, ContinueFailed);
	}

	private void RestartGame () {
		//GameEventMessage.SendEvent ("Restart");

	}
}