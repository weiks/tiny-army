using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hammerplay.Utils {
	public interface IPoolable {

		void PoolInstantiate(Vector3 position, Quaternion rotation);
		void PoolDestroy();
		bool IsAlive();
		GameObject GetGameObject();
	}
}
