using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colorable : MonoBehaviour {

	public void SetColor(Color color) {
		GetComponent<Renderer> ().material = new Material (GetComponent<Renderer> ().material);
		GetComponent<Renderer> ().material.color = color;
	}
}
