using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Barf : MonoBehaviour {
	public Animator animator;
	public enum BarfMode {Unicorn, Whale};
	public BarfMode barfMode = BarfMode.Unicorn;

	public List<Transform> pukeChunkList;
	public bool spawnNow = true;
	public float speed = 0.2f;
	public float timeVariation = 0.5f;
	public float timeOut = 0f;
	public float spread = 0.3f;
	public float chunkSpeed = 200;
	public GameObject target;
	public Vector3 direction;


	// Use this for initialization
	void Start () {
		animator = GameObject.Find("_Animator").GetComponent<Animator>();
		target = Traversals.TraverseHierarchy(transform, "BarfTarget").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		direction = (target.transform.position - transform.position).normalized; 
		if (spawnNow) {
			puke();
		}
	}

	void puke() {
		Transform item;
		int typeIndex = Random.Range(0, pukeChunkList.Count);
		if (timeOut < Time.time) {
			float spreadCalculated = 0.1f;
			if (barfMode == BarfMode.Unicorn)
				spreadCalculated = spread * animator.rightTrigger;
			else
				spreadCalculated = spread * animator.rightTrigger;

			float x = Random.Range(direction.x - spreadCalculated, direction.x + spreadCalculated);
			float y = Random.Range(direction.y - spreadCalculated, direction.y - spreadCalculated);
			Vector3 genDir = new Vector3(x, y, 0);
			item = (Transform)Instantiate(pukeChunkList[typeIndex]);
			item.gameObject.AddComponent<PukeLifespan>();
			item.transform.position = transform.position;
			item.rigidbody.AddForce(genDir * chunkSpeed);
			timeOut += speed + Random.Range(0, timeVariation);
		}
	}
}
