using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnim : MonoBehaviour {

	public Vector3 target;
	public float speed = 1.5f;

	// Use this for initialization
	void Start () {
		target = transform.position;
	}

	// Update is called once per frame
	void Update () {
		if (target != transform.position) {
			transform.position = Vector3.MoveTowards (transform.position, target, speed * Time.deltaTime);
		}

	}
}
