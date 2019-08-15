using System.Collections.Generic;
using UnityEngine;

namespace Hammerplay.TinyArmy {
	public class Lane : MonoBehaviour {

		[SerializeField]
		private int laneId = -1;

		public int LaneId {
			get { return laneId; }
		}

		private new Collider2D collider2D;

		public Collider2D Collider2D {
			get { return collider2D; }
		}

		private void Awake() {
			//InputManager.AddLane(this);
			
			collider2D = GetComponent<Collider2D>();
		}

		private void OnEnable() {
			GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
		}

		private void OnDisable() {
			GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
		}

		private void GameManager_OnGameStateChanged(GameManager.GameState gameState) {
			isPaused = !(gameState == GameManager.GameState.InGame);
		}


		private bool isPaused;

		private void Update() {
			if (isPaused || !GameManager.IsInputEnabled )
            {
				return;
            }

			Touch[] touches = Input.touches;

			for (int i = 0; i < touches.Length; i++) {
				Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touches[i].position);
				Vector2 touchPos = new Vector2(worldPosition.x, worldPosition.y);

				bool isColliding = (Collider2D == Physics2D.OverlapPoint(touchPos));

				if (isColliding) {
					/*if (touches[i].phase == TouchPhase.Began) {
						isBeganHere = true;
					}*/

					if ((touches[i].phase == TouchPhase.Stationary || touches[i].phase == TouchPhase.Moved)) {
						IsTouching = true;
					} else if (touches[i].phase == TouchPhase.Ended) {
						Tap();
						IsTouching = false;
						//isBeganHere = false;
					}
					return;

				} else {

					IsTouching = false;
				}
			}
			
		}

		[SerializeField]
		private bool isTouching;

		public bool IsTouching {
			get { return isTouching; }
			private set {
				isTouching = value;
				if (OnLaneTouching != null && Input.touches.Length < 3)
					OnLaneTouching(laneId, isTouching);
			}
		}

		private bool isBeganHere;

		public void Tap () {
			if (OnLapTapped != null)
				OnLapTapped(laneId);
		}

		public delegate void LaneTapHandler(int laneId);
		public static event LaneTapHandler OnLapTapped;

		public delegate void LaneTouchHanlder(int laneId, bool touching);
		public static event LaneTouchHanlder OnLaneTouching;
	}
}
