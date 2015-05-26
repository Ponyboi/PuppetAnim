using UnityEngine;
using System.Collections;

[System.Serializable]
public class TriggerStates{
	
	public bool leftTriggerDown;
	public bool leftTriggerHeld;
	public bool rightTriggerDown;
	public bool rightTriggerHeld;
	public bool rightTriggerUp;
	public bool leftTriggerUp;
	
	public void Clear(){
		leftTriggerDown = false;
		rightTriggerDown = false;
		leftTriggerUp = false;
		rightTriggerUp = false;
		//leftTriggerHeld = false;
		//rightTriggerHeld = false;
	}
}
