using UnityEngine;
using System.Collections;

public class Eat : MonoBehaviour {

	public Animator creature;
	public CreatureType creatureType = CreatureType.Unicorn;
	public int shrimpCount = 0;
	public float testTime = 0;

	// Use this for initialization
	void Start () {
		if (creatureType == CreatureType.Unicorn) {
			creature = GameObject.Find("Unicorn").GetComponent<Animator>();
		} else {
			creature = GameObject.Find("Whale").GetComponent<Animator>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > testTime) {
			//Debug.Log(creature.creatureType + " shrimpCount: " + shrimpCount);
			testTime += 1;
		}
	}

	void OnTriggerStay(Collider obj) {
		//Debug.Log(obj.name);
		if (obj.name == "Shrimp") {
			if (creature.animLerpCurrentTime > Time.time && creature.mode == AnimMode.Munch) {
				Destroy(obj.gameObject);
				shrimpCount++;
			}
		}
	}
}
