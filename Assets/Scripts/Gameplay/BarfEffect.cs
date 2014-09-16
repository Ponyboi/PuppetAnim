using UnityEngine;
using System.Collections;

public class BarfEffect : MonoBehaviour {
	
	public Animator creature;
	public CreatureType creatureType = CreatureType.Unicorn;
	
	public float slowness = 1;
	public float slowAmount = 0.05f;
	public float recoveryRate = 0.01f;
	public float recoreryTime = 0.1f;
	public float recoveryTimeHolder = 0;
	public float creatureSpeed = 0;
	
	
	// Use this for initialization
	void Start () {
		if (creatureType == CreatureType.Unicorn) {
			creature = GameObject.Find("Unicorn").GetComponent<Animator>();
		} else {
			creature = GameObject.Find("Whale").GetComponent<Animator>();
		}
		creatureSpeed = creature.moveSpeed;
	}
	
	// Update is called once per frame
	void Update () {
		if (recoveryTimeHolder < Time.time) {
			recoveryTimeHolder = Time.time + recoreryTime;
			if (slowness < 1) {
				slowness += recoveryRate;
			}
		}
		creature.moveSpeed = creatureSpeed * slowness;
	}
	
	void OnCollisionEnter(Collision obj) {
		Debug.Log(obj.gameObject.name);
		if (obj.gameObject.name == "Barf") {
			Destroy(obj.gameObject);
			slowness -= slowAmount;
			if (slowness < 0)
				slowness = 0;
		}
	}
}
