using UnityEngine;
using System.Collections;

public class BarfHitter : MonoBehaviour {

	BarfEffect effect;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter(Collision obj) {
//		Debug.Log(obj.name);
//		if (obj.gameObject.GetComponent<BarfEffect>()) {
//			effect = obj.gameObject.GetComponent<BarfEffect>();
//			Destroy(this.gameObject);
//			effect.slowness -= effect.slowAmount;
//		}
	}

}
