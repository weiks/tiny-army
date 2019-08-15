using Hammerplay.Utils;
using UnityEngine;

namespace Hammerplay.TinyArmy {
	public class Blast : MonoBehaviour, IPoolable {


		public GameObject GetGameObject() {
			return gameObject;
		}

		public bool IsAlive() {
			return gameObject.activeSelf;
		}

		public void PoolDestroy() {
			gameObject.SetActive(false);
		}

		public void PoolInstantiate(Vector3 position, Quaternion rotation) {
			gameObject.SetActive(true);
			transform.position = position;
			transform.rotation = rotation;
		}

	}
}
