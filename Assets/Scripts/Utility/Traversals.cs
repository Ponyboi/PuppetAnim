using UnityEngine;
using System.Collections;
using System;

public class Traversals : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static Transform TraverseHierarchy(Transform root, String name) {
		Transform obj = root;
		foreach (Transform child in root) {
			// Do something with child, then recurse.
			//Debug.Log(child.name);
			if (obj.name == name) {
				break;
			}
			if (child.gameObject.name == name) {
				return child;
			} else {
				obj = TraverseHierarchy(child, name);
			}
		}
		return obj;
	}
}
