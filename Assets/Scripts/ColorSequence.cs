using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSequence : MonoBehaviour {

	private List<Color> colors;

	public int steps = 8;
	public int step = 0;

	Color CreateRandom() {
		return Random.ColorHSV();
	}

	// Use this for initialization
	void Start () {
		colors = new List<Color> ();
		colors.Add (CreateRandom ());
		colors.Add (CreateRandom ());
	}

	public Color GetNextColor() {
		if (step == steps) {
			step = 0;
			colors.Add (CreateRandom ());
		}
		Color c = Color.Lerp (colors [colors.Count - 2], colors [colors.Count - 1], (float) step / steps);
		step++;
		return c;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
