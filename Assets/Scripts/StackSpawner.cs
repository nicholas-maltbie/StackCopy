using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StackSpawner : MonoBehaviour {

	public Transform topOfStack;
	public float blockHeight = 0.5f;
	public float snapDelta = 0.1f;
	public float cycleTime = 2.5f;
	public float cycleDecay = 0.95f;
	public SpawnPt[] spawnLocs;

	public int score = 0;
	public Text scoreText;

	public ColorSequence colors;

	public CameraAnim camera;
	private Vector3 cameraOrig;

	public GameObject movingBlockPrefab;

	private bool playing = true;

	private GameObject lastBlock;
	private int currLoc;
	private GameObject curr;

	public float moveDist = 2.2f;

	public GameObject bottomBlock, baseBlock;

	// Use this for initialization
	void Start () {
		cameraOrig = camera.transform.localPosition;
		bottomBlock.GetComponent<Colorable> ().SetColor (colors.GetNextColor ());
		baseBlock.GetComponent<Colorable> ().SetColor (colors.GetNextColor ());
	}

	void SpawnNext() {
		curr = Instantiate (movingBlockPrefab);
		curr.GetComponent<Colorable> ().SetColor (colors.GetNextColor ());
		Vector3 spos = spawnLocs [currLoc].gameObject.transform.position;
		curr.transform.position = 
			new Vector3(spos.x, spos.y + blockHeight / 2, spos.z) + 
			new Vector3(LastBlockPos().x, 0, LastBlockPos().z);
		curr.transform.localScale = LastBlockSize ();
		curr.GetComponent<MovingBlock> ().dir = spawnLocs [currLoc].direction;
		curr.GetComponent<MovingBlock> ().dist = moveDist;
		curr.GetComponent<MovingBlock> ().cycleTime = cycleTime;
		cycleTime *= cycleDecay;

		currLoc++;
		if (currLoc >= spawnLocs.Length) {
			currLoc = 0;
		}

		camera.target = topOfStack.position + cameraOrig * Mathf.Max(LastBlockSize().x, LastBlockSize().z)  + 
			new Vector3(LastBlockPos().x, 0, LastBlockPos().z);
	}

	Vector3 LastBlockPos() {
		if (lastBlock == null) {
			return Vector3.zero;
		}
		return lastBlock.transform.position;
	}

	Vector3 LastBlockSize() {
		if (lastBlock == null) {
			return new Vector3(1, blockHeight, 1);
		}
		return lastBlock.transform.localScale;
	}

	void ChopBlock(GameObject block) {
		Vector3 cblock = block.transform.position;
		Vector3 sblock = block.transform.localScale;
		Vector3 center = LastBlockPos ();
		Vector3 size = LastBlockSize ();

		if (cblock != center) {
			Vector3 cnew = new Vector3 ((center.x + cblock.x) / 2, 
				               cblock.y, 
				               (center.z + cblock.z) / 2);
			Vector3 snew = new Vector3 (size.x - Mathf.Abs (center.x - cblock.x), 
				               blockHeight, 
				               size.z - Mathf.Abs (center.z - cblock.z));

			block.transform.position = cnew;
			block.transform.localScale = snew;

			//spawn falling block
			if (snew != size) {
				Vector3 fsize = new Vector3 (Mathf.Abs (center.x - cblock.x), 
					               blockHeight, 
					               Mathf.Abs (center.z - cblock.z));
				Vector3 moveDir = block.GetComponent<MovingBlock> ().GetDirAbs ();
				if (cblock.x < center.x || cblock.z < center.z)
					moveDir *= -1;
				Vector3 fcenter = new Vector3 (center.x + moveDir.x * (size.x + fsize.x) / 2, 
					                 cblock.y, 
					                 center.z + moveDir.z * (size.z + fsize.z) / 2);
				if (fsize.x == 0)
					fsize.x = size.x;
				if (fsize.z == 0)
					fsize.z = size.z;
				//Debug.Log (fsize);
				GameObject dup = Instantiate (block);
				dup.transform.position = fcenter;
				dup.transform.localScale = fsize;
				dup.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
			}
		}
	}

	Vector3 SnapDelta() {
		return snapDelta * LastBlockSize ();
	}

	bool IsInSnapDelta(Vector3 pos1, Vector3 pos2) {
		Vector3 delta = pos1 - pos2;
		return Mathf.Abs(delta.x) <= SnapDelta ().x && 
			Mathf.Abs(delta.y) <= SnapDelta().y && 
			Mathf.Abs(delta.z) <= SnapDelta ().z;
	}

	void DropBlock() {
		topOfStack.transform.position = topOfStack.transform.position + Vector3.up * blockHeight;
		curr.GetComponent<MovingBlock> ().moving = false;
		//curr.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
		if (IsInSnapDelta(new Vector3(curr.transform.position.x, 0, curr.transform.position.z), 
			new Vector3(LastBlockPos().x, 0, LastBlockPos().z))) {
			curr.transform.position = new Vector3(LastBlockPos ().x, curr.transform.position.y,  LastBlockPos ().z);
		}

		//check for loss
		Vector3 posDelta = new Vector3 (Mathf.Abs (LastBlockPos ().x - curr.transform.position.x), 
			                   0, 
			                   Mathf.Abs (LastBlockPos ().z - curr.transform.position.z));
		if (posDelta.x >= LastBlockSize ().x ||
		    posDelta.z >= LastBlockSize ().z) {
			//lose game
			playing = false;
			curr.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
		} else {
			score++;
			ChopBlock (curr);
			lastBlock = curr;
			curr = null;
		}
	}

	// Update is called once per frame
	void Update () {
		if (playing) {
			if (curr == null) {
				SpawnNext ();
			} else {
				if (Input.GetButtonDown ("Jump")) {
					DropBlock ();
				}
			}
		}
		scoreText.text = score.ToString ();
	}
}
