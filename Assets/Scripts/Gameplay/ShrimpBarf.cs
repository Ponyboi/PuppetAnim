using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShrimpBarf : MonoBehaviour {
	public Animator animator;
	public enum BarfMode {Unicorn, Whale};
	public BarfMode barfMode = BarfMode.Unicorn;
	
	public List<Transform> pukeChunkList;
	public bool spawnNow = false;
	public bool animBool = false;
	public float speed = 0.2f;
	public float timeVariation = 0.5f;
	public float timeOut = 0f;
	public float spread = 0.3f;
	public float chunkSpeed = 300;
	public GameObject target;
	public Vector3 direction;
	
	
	// Use this for initialization
	void Start () {
		target = Traversals.TraverseHierarchy(transform, "BarfTarget").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
//		if (Input.GetButtonDown("Jump")) {
//			spawnNow = !spawnNow;
//		}
		direction = (target.transform.position - transform.position).normalized;
//		if (animator.currentVal > 0.1f && animator.mode == Modes.Barf)
//			spawnNow = true;
//		else
//			spawnNow = false;
		
		if (spawnNow) {
			puke();
		}
	}
	
	void puke() {
		Transform item;
		int typeIndex = Random.Range(0, pukeChunkList.Count);
		
		if (timeOut < Time.time) {
			float spreadCalculated = spread;
			if (animBool) {
				if (barfMode == BarfMode.Unicorn)
					spreadCalculated = spread * animator.currentVal;
				else
					spreadCalculated = spread * animator.currentVal;
			}
			
			float x = Random.Range(direction.x - spreadCalculated, direction.x + spreadCalculated);
			float y = Random.Range(direction.y - spreadCalculated, direction.y - spreadCalculated);
			float xPos = Random.Range(-10, 8);
			Vector3 eulerAngleVelocity = new Vector3(Random.Range(0,100), Random.Range(0,100), Random.Range(0,100));
			Vector3 genDir = new Vector3(x, y, 0);
			item = (Transform)Instantiate(pukeChunkList[typeIndex]);
			item.name = "Shrimp";
			item.gameObject.AddComponent<PukeLifespan>();
			item.transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
			item.rigidbody.AddForce(genDir * chunkSpeed);
			Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
			item.rigidbody.MoveRotation(item.rigidbody.rotation * deltaRotation);
	//		speed = 1.04f - (animator.currentVal / (animator.currentVal + 0.001f));
			timeOut += speed + (speed * Random.Range(0, timeVariation));
		}
	}
}
