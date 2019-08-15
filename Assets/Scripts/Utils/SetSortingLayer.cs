using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hammerplay.Utils {
	[RequireComponent(typeof(SpriteRenderer))]
	[ExecuteInEditMode]
	public class SetSortingLayer : MonoBehaviour {

		[SerializeField]
		private SortingLayer sortingLayer;

		private void Awake() {
			GetComponent<SpriteRenderer>().sortingOrder = (int)sortingLayer;
		}

		private void Update() {
			if (Application.isEditor) {
				GetComponent<SpriteRenderer>().sortingOrder = (int)sortingLayer;
			}
		}

		/*[ContextMenu ("Copy Sorting Layer ID")]
		private void CopySortingLayerInfo () {
			sortingLayer = GetComponent<SpriteRenderer>().sortingLayerID;
		}*/

		private enum SortingLayer { Background = 1, Barricade = 1, Bullet = 2, Actor = 3, FX = 4}
	}
}
