using UnityEngine;
using System.Collections;

public class Eat : MonoBehaviour {

	public Transform shrimp;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider obj) {
		//Debug.Log(obj.name);
		if (obj.name == "Shrimp") {
			Debug.Log("Shrimp!");
		}
	}
}
