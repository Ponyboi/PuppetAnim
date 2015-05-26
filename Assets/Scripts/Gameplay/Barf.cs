using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Barf : MonoBehaviour {
	public Animator creature;
	public CreatureType creatureType = CreatureType.Unicorn;
	public Eat eater;

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

	public float shrimpTimeout = 0;
	public float shrimpInterval = 0.1f;


	// Use this for initialization
	void Start () {
//		animator = GameObject.Find("_Animator").GetComponent<Animator>();
		if (creatureType == CreatureType.Unicorn) {
			creature = GameObject.Find("Unicorn").GetComponent<Animator>();
		} else {
			creature = GameObject.Find("Whale").GetComponent<Animator>();
		}
		target = Traversals.TraverseHierarchy(transform, "BarfTarget").gameObject;
		eater = Traversals.TraverseHierarchy(creature.transform, "Eat").GetComponent<Eat>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Jump")) {
			spawnNow = !spawnNow;
		}
		direction = (target.transform.position - transform.position).normalized;
		if (creature.currentVal > 0.3f && creature.mode == AnimMode.Barf && eater.shrimpCount > 0)
			spawnNow = true;
		else
			spawnNow = false;

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
					spreadCalculated = spread * creature.currentVal;
				else
					spreadCalculated = spread * creature.currentVal;
			}

			float x = Random.Range(direction.x - spreadCalculated, direction.x + spreadCalculated);
			float y = Random.Range(direction.y - spreadCalculated, direction.y - spreadCalculated);
			Vector3 eulerAngleVelocity = new Vector3(Random.Range(0,100), Random.Range(0,100), Random.Range(0,100));
			Vector3 genDir = new Vector3(x, y, 0);
			item = (Transform)Instantiate(pukeChunkList[typeIndex]);
			item.name = "Barf";
			item.gameObject.AddComponent<BarfHitter>();
			item.gameObject.AddComponent<PukeLifespan>();
			item.transform.position = transform.position;
			item.GetComponent<Rigidbody>().AddForce(genDir * chunkSpeed);
			Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
			item.GetComponent<Rigidbody>().MoveRotation(item.GetComponent<Rigidbody>().rotation * deltaRotation);
			speed = 1.04f - (creature.currentVal / (creature.currentVal + 0.001f));
			timeOut += speed + (speed * Random.Range(0, timeVariation));
		}
		if (shrimpTimeout < Time.time) {
			shrimpTimeout = Time.time + shrimpInterval;
			eater.shrimpCount--;
		}
	}
}
