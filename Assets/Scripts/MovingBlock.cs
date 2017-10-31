using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour {

	public bool moving = true;
	public float dist = 1.0f;
	public float cycleTime = 0.5f;
	public Vector3 dir = Vector3.right;

	private Vector3 origin;
	private float traveled = 0;
	private float dirMod = 1;

	public Vector3 GetDirAbs() {
		return new Vector3(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z));
	}

	// Use this for initialization
	void Start () {
		origin = transform.position;
	}

	// Update is called once per frame
	void Update () {
		if (moving) {
			traveled += dist * Time.fixedDeltaTime / cycleTime * dirMod;
			if (traveled > dist) {
				traveled = dist - traveled % dist;
				dirMod *= -1;
			} else if (traveled < 0) {
				traveled = -traveled;
				dirMod *= -1;
			}

			transform.position = origin + dir * traveled;
		}
	}
}
