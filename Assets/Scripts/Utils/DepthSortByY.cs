using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class DepthSortByY : MonoBehaviour {

	private const int IsometricRangePerYUnit = 100;

	private SpriteRenderer spriteRenderer;

	private void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update() {
		spriteRenderer.sortingOrder = -(int)(transform.position.y * IsometricRangePerYUnit);
	}
}
