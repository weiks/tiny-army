using DG.Tweening;
using UnityEngine;
using Hammerplay.Utils;

namespace Hammerplay.TinyArmy {
	public class GameCamera : MonoBehaviour {

		[SerializeField]
		private SpriteRenderer backgroundSprite;

		[ContextMenu("MoveCameraToBottom")]
		private void MoveCameraToBottom() {
			float vertExtent = Camera.main.orthographicSize;
			float hortExtent = vertExtent * Screen.width / Screen.height;

			var leftBound = hortExtent - backgroundSprite.sprite.bounds.size.x / 2;
			var rightBound = (backgroundSprite.sprite.bounds.size.x / 2) - hortExtent;
			var bottomBound = vertExtent - backgroundSprite.sprite.bounds.size.y / 2;
			var topBound = (backgroundSprite.sprite.bounds.size.y / 2) - vertExtent;

			var pos = new Vector3(transform.position.x, backgroundSprite.bounds.min.y, transform.position.z);
			pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
			pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);

			transform.position = pos;
		}

		[ContextMenu("AspectRatioCamera")]
		private void AspectRatioCamera() {
			Camera.main.orthographicSize = Misc.Interpolate(Camera.main.aspect, 0.4387729f, 0.7503526f, 10.79f, 8.853186f);
			MoveCameraToBottom();
		}

		private void Update() {
			AspectRatioCamera();
		}

		private Vector3 currentPosition;
		private bool isShaking;

		private void _CameraShake(float duration, float amount) {
			if (isShaking)
				return;

			Vibration.Vibrate((long)(duration * 500));
			isShaking = true;

			MoveCameraToBottom();
			currentPosition = transform.position;

			transform.DOShakePosition(duration, amount).OnComplete(() => {
				isShaking = false;
				transform.position = currentPosition;
			}); ;
		}

		public static void CameraShake(float duration, float amount) {
			_instance._CameraShake(duration, amount);
		}

		private static GameCamera _instance;

		private void Awake() {
			_instance = this;
		}
	}
}
