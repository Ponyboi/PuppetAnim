using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Modes{
	Talk, Barf, Munch
}
public enum CreatureType{
	Unicorn, Whale
}
public enum PlayMode {
	Play, Animate
}
public class Animator : MonoBehaviour {
	//Moving Parts
	public GameObject creature;
	private GameObject Head;
	private GameObject HeadHinge;
	private GameObject Neck;
	private GameObject NeckHinge;
	private GameObject MouthTop;
	private GameObject MouthTopHinge;
	private GameObject MouthBottom;
	private GameObject MouthBottomHinge;

	//Sound
	public GameObject audioNode;
	public AudioSource mouth;
	public AudioClip munch;
	public AudioClip barf;
	public float soundVolume = 1.0f;
	public bool barfSoundBool = true;

	//Movement
	private Vector3 creaturePos;
	public float moveSpeed = 0.002f;
	public float lerpSpeed = 5f;

	//Input
	public float rightTrigger;
	public float leftTrigger;
	public float rightAnalogV;
	public float rightAnalogH;
	public float leftAnalogV;
	public float leftAnalogH;
	public bool DpadH = false;
	private float triggerDebounce = 0.02f;
	public float maxTalkRotateAngle = 20;
	public float maxBarfRotateAngle = 30;
	public float maxMunchRotateAngle = 30;
	private float barfWaitTime = 0.34f;
	private float munchWaitTime = 0.3f;
	public float buttonDownTime = 0;
	public float buttonUpTime = 0;
	public float animLerpVal = 0;
	public float animLerpTime = 20f;
	public float animLerpCurrentTime = 0;
	public float animLerpFirstTime = 0;
	public KeyCode creatureKeyCode;

	//Recording
	//public float[] rightTriggerBuffer;
	public List<List<float>> animationGlobe = new List<List<float>>();
	public List<float> currentBuffer = new List<float>(1000);
	public List<float> rightTriggerBuffer = new List<float>(1000);
	public List<float> rightAnalogVBuffer = new List<float>(1000);
	public List<float> talkBuffer = new List<float>(1000);
	public List<float> barfBuffer = new List<float>(1000);
	public List<float> munchBuffer = new List<float>(1000);
	public int index = 0;
	public float currentVal = 0;
	public int loopPos = 999999999;
	public bool play = false;
	public bool record = false;
	public bool looping = false;
	public float interval = 0.1f;
	public float currentTime = 0f;
	public int animIndex = 0;

	// Delegates
	private AnimationDelegate[] currentAnim;

	public Modes mode = Modes.Talk;
	public PlayMode playMode = PlayMode.Play;
	public CreatureType creatureType = CreatureType.Unicorn;
	public int animSign;

	// Use this for initialization
	void Start () {
		creature = this.gameObject;
		currentAnim = new AnimationDelegate[]{Talk, Barf, Munch};
//		animationGlobe.Add(talkBuffer);
//		animationGlobe.Add(barfBuffer);
		if (creatureType == CreatureType.Unicorn) {
			animSign = 1;
			creatureKeyCode = KeyCode.Return;
		} else {
			animSign = -1;
			creatureKeyCode = KeyCode.Space;
		}

		creaturePos = new Vector3(creature.transform.position.x, creature.transform.position.y, creature.transform.position.z);
		//buffer = new float[];
		currentBuffer.Insert(index, 0);
		talkBuffer.Insert(index, 0);
		barfBuffer.Insert(index, 0);
		munchBuffer.Insert(index, 0);
		rightTriggerBuffer.Insert(index, 0);
		rightAnalogVBuffer.Insert(index, 0);
		Debug.Log(rightTriggerBuffer[index]);

		Head = Traversals.TraverseHierarchy(creature.transform, "Head").gameObject;
		HeadHinge = Traversals.TraverseHierarchy(creature.transform, "HeadHinge").gameObject;
		Neck = Traversals.TraverseHierarchy(creature.transform, "Neck").gameObject;
		NeckHinge = Traversals.TraverseHierarchy(creature.transform, "NeckHinge").gameObject;
		MouthTop = Traversals.TraverseHierarchy(creature.transform, "MouthTop").gameObject;
		MouthTopHinge = Traversals.TraverseHierarchy(creature.transform, "MouthTopHinge").gameObject;
		MouthBottom = Traversals.TraverseHierarchy(creature.transform, "MouthBottom").gameObject;
		MouthBottomHinge = Traversals.TraverseHierarchy(creature.transform, "MouthBottomHinge").gameObject;

		audioNode = new GameObject();
	 	mouth = audioNode.AddComponent<AudioSource>();
		mouth.clip = munch;
		audioNode.transform.parent = transform;
	}
	
	// Update is called once per frame
	void Update () {
		InputControls();
		PlaybackControls();
		MovementControls();

		currentAnim[(int) mode](currentVal, 0);
		//Talk(0, currentBuffer[index]);
		//Barf(0, currentBuffer[index]);

		//Bob
		creature.transform.position = new Vector3(creature.transform.position.x, creature.transform.position.y + Mathf.Sin(Time.time)*0.0015f, creature.transform.position.z);
	}

	public void MovementControls() {
		if (creatureType == CreatureType.Unicorn) {
			creaturePos += new Vector3(Input.GetAxisRaw("RightAnalog_H"), -Input.GetAxisRaw("RightAnalog_V")) * 0.2f;
		} else {
			creaturePos += new Vector3(Input.GetAxisRaw("LeftAnalog_H"), -Input.GetAxisRaw("LeftAnalog_V")) * 0.2f;
		}

		if (creatureType == CreatureType.Unicorn) {
			if (Input.GetKey(KeyCode.UpArrow))
				creaturePos += Vector3.up * moveSpeed;
			if (Input.GetKey(KeyCode.RightArrow))
				creaturePos += Vector3.right * moveSpeed;
			if (Input.GetKey(KeyCode.DownArrow))
				creaturePos += Vector3.down * moveSpeed;
			if (Input.GetKey(KeyCode.LeftArrow))
				creaturePos += Vector3.left * moveSpeed;
		} else {
			if (Input.GetKey(KeyCode.W))
				creaturePos += Vector3.up * moveSpeed;
			if (Input.GetKey(KeyCode.D))
				creaturePos += Vector3.right * moveSpeed;
			if (Input.GetKey(KeyCode.S))
				creaturePos += Vector3.down * moveSpeed;
			if (Input.GetKey(KeyCode.A))
				creaturePos += Vector3.left * moveSpeed;
		}

		if (creaturePos.y > 2.8f)
			creaturePos = new Vector3(creaturePos.x, 2.8f, creaturePos.z);
		if (creaturePos.y <  -5.1f)
			creaturePos = new Vector3(creaturePos.x, -5.1f, creaturePos.z);
		if (creaturePos.x > 6)
			creaturePos = new Vector3(6, creaturePos.y, creaturePos.z);
		if (creaturePos.x < -10)
			creaturePos = new Vector3(-10, creaturePos.y, creaturePos.z);

		creature.transform.position = Vector3.Lerp(creature.transform.position, creaturePos, lerpSpeed * Time.deltaTime);

		if (Input.GetKeyUp(creatureKeyCode)) {
			buttonUpTime = Time.time;
			barfSoundBool = true;
		}
		if (buttonUpTime > buttonDownTime) {
			if (buttonUpTime - buttonDownTime < munchWaitTime) {
				mode = Modes.Munch;
				animLerpCurrentTime = Time.time + animLerpTime;
				buttonUpTime = buttonDownTime;
				mouth.clip = munch;
				PlaySound();
			}
		}
		if (Input.GetKey(creatureKeyCode)) {
			if (Input.GetKeyDown(creatureKeyCode)) {
				buttonDownTime = Time.time;
			}
				//If button held down, Barf
			if (Time.time - buttonDownTime > barfWaitTime) {
				mode = Modes.Barf;
				animLerpCurrentTime = Time.time + animLerpTime;
				animLerpFirstTime = buttonDownTime + barfWaitTime;
				if (barfSoundBool) {
					mouth.clip = barf;
					PlaySound();
				}
				barfSoundBool = false;
			}
		}

		if (animLerpCurrentTime > Time.time) {
			float timeVar = (Time.time - (animLerpCurrentTime - (animLerpTime)));
			float timeVarFirst = (Time.time - (animLerpFirstTime));
			if (Time.time < animLerpCurrentTime - (animLerpTime/2)) {
				animLerpVal = Mathf.Lerp(0, 1, timeVarFirst/(animLerpTime/2));
			} else {
				animLerpVal = Mathf.Lerp(1, 0, (timeVar - (animLerpTime/2))/(animLerpTime/2));
			}
		}
		if (playMode == PlayMode.Play) {
			currentVal = animLerpVal;
		}

	}

	public void InputControls(){
		//Input
		if(Input.GetAxisRaw("RightTrigger") > triggerDebounce)
			rightTrigger = Input.GetAxisRaw("RightTrigger");
		else
			rightTrigger = 0;
		
		if(Input.GetAxisRaw("LeftTrigger") > triggerDebounce)
			leftTrigger = Input.GetAxisRaw("LeftTrigger");
		else
			leftTrigger = 0;

		rightAnalogV = ExtensionMethods.Remap(Input.GetAxisRaw("RightAnalog_V"), 0, 1, 0, 1);
//		rightAnalogH = ExtensionMethods.Remap(Input.GetAxisRaw("RightAnalog_H"), 0, 1, 0, 1);
//		leftAnalogV = ExtensionMethods.Remap(Input.GetAxisRaw("LefttAnalog_V"), 0, 1, 0, 1);
//		leftAnalogH =ExtensionMethods.Remap(Input.GetAxisRaw("LeftAnalog_H"), 0, 1, 0, 1);
//		else
//			rightAnalogV = 0;
	}

	public void PlaybackControls() {
		//Playback Controls
		InputRouter();
		if (Input.GetButtonDown("xbox_a"))
			play = !play;
		if (play) {
			if (Time.time > currentTime) {
				if (currentBuffer.Count > index)
					currentBuffer.Add(0); 
				index++;
			}
		}
		//Clear
		if (Input.GetButton("xbox_rightAnalogButton")) {
			currentVal = 0;
		}
		//Record
		if (Input.GetButtonDown("xbox_b"))
			record = !record;
		if (record) {
			if (Time.time > currentTime) {
				currentBuffer[index] = currentVal;
			}
		}
		//Reset
		if (Input.GetButtonDown("xbox_x")) {
			index = 0;
		}
		//Set Loop
		if (Input.GetButtonDown("xbox_y")) {
			looping = !looping;
			loopPos = index;
			index = 0;
		}
		if (looping) {
			if (index >= loopPos) {
				index = 0;
			}
		}
		//Switch Animation Mode
//		Debug.Log(Input.GetAxisRaw("xbox_DPad_H"));
		if (Input.GetAxisRaw("xbox_DPad_H") == 0)
			DpadH = true;
		if (Input.GetAxisRaw("xbox_DPad_H") > 0.5f) {
//			Debug.Log(mode);
			if (DpadH) {
				if ((animIndex + 3) > Enum.GetName(typeof(Modes), 0).Length)
					animIndex = 0;
				else
					animIndex ++;
				mode = (Modes) animIndex;
				Debug.Log(mode);
			}
			DpadH = false;
		}
		if (Time.time > currentTime) {
			currentTime += interval;
		}
		//Debug.Log(currentVal);
		currentVal = (Mathf.Clamp(currentVal + currentBuffer[index], 0, 1));
		//rightTriggerBuffer[index] = currentVal;
		//rightTrigger = Mathf.Clamp(rightTrigger + rightTriggerBuffer[index], 0, 1);
	}
	
	public void Talk(float inputVal, float existingVal) {
		inputVal = Mathf.Clamp(inputVal + existingVal, 0, 1);
		                           
		//		Debug.Log(inputVal);
//		rightMouthTopHinge.transform.rotation = Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(inputVal) * 0.5f , transform.forward);
//		rightMouthBottomHinge.transform.rotation = Quaternion.AngleAxis(maxTalkRotateAngle * Mathf.Abs(inputVal) , transform.forward);
//		rightHeadHinge.transform.rotation = Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(inputVal) * 0.2f , transform.forward);
//		rightNeckHinge.transform.rotation = Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(inputVal) * 0.1f, transform.forward);

		Quaternion NeckHingeRot = Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(inputVal) * 0.1f * animSign, transform.forward);
		Quaternion HeadHingeRot = NeckHingeRot * Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(inputVal) * 0.2f * animSign, transform.forward);
		Quaternion MouthBottomHingeRot = HeadHingeRot * Quaternion.AngleAxis(maxTalkRotateAngle * Mathf.Abs(inputVal) * 1f * animSign, transform.forward);
		Quaternion MouthTopHingeRot = HeadHingeRot * Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(inputVal) * 0.5f * animSign, transform.forward);
		
		NeckHinge.transform.rotation =  NeckHingeRot;
		HeadHinge.transform.rotation =  HeadHingeRot;
		MouthBottomHinge.transform.rotation = MouthBottomHingeRot;
		MouthTopHinge.transform.rotation = MouthTopHingeRot;

	}

	public void Barf(float inputVal, float existingVal) {
		inputVal = Mathf.Clamp(inputVal + existingVal, 0, 1);

		Quaternion NeckHingeRot = Quaternion.AngleAxis(maxBarfRotateAngle * Mathf.Abs(inputVal) * 1.3f * animSign, transform.forward);
		Quaternion HeadHingeRot = NeckHingeRot * Quaternion.AngleAxis(-maxBarfRotateAngle * Mathf.Abs(inputVal) * 1.2f * animSign, transform.forward);
		Quaternion MouthBottomHingeRot = HeadHingeRot * Quaternion.AngleAxis(maxBarfRotateAngle * Mathf.Abs(inputVal) * 0.7f * animSign, transform.forward);
		Quaternion MouthTopHingeRot = HeadHingeRot * Quaternion.AngleAxis(-maxBarfRotateAngle * Mathf.Abs(inputVal) * 0.3f * animSign, transform.forward);

		NeckHinge.transform.rotation =  NeckHingeRot;
		HeadHinge.transform.rotation =  HeadHingeRot;
		MouthBottomHinge.transform.rotation = MouthBottomHingeRot;
		MouthTopHinge.transform.rotation = MouthTopHingeRot;
	}
	public void Munch(float inputVal, float existingVal) {
		inputVal = Mathf.Clamp(inputVal + existingVal, 0, 1);
		
		Quaternion NeckHingeRot = Quaternion.AngleAxis(maxBarfRotateAngle * Mathf.Abs(inputVal) * 0.7f * animSign, transform.forward);
		Quaternion HeadHingeRot = NeckHingeRot * Quaternion.AngleAxis(-maxBarfRotateAngle * Mathf.Abs(inputVal) * 0.8f * animSign, transform.forward);
		Quaternion MouthBottomHingeRot = HeadHingeRot * Quaternion.AngleAxis(-maxBarfRotateAngle * Mathf.Abs(inputVal) * 0.55f * animSign, transform.forward);
		Quaternion MouthTopHingeRot = HeadHingeRot * Quaternion.AngleAxis(maxBarfRotateAngle * Mathf.Abs(inputVal) * 0.2f * animSign, transform.forward);
		
		NeckHinge.transform.rotation =  NeckHingeRot;
		HeadHinge.transform.rotation =  HeadHingeRot;
		MouthBottomHinge.transform.rotation = MouthBottomHingeRot;
		MouthTopHinge.transform.rotation = MouthTopHingeRot;
	}

	void PlaySound() {
		if (mouth.clip != null) //only play if we assigned a sound to the AudioClip slot!
		{
			if (mouth.isPlaying) //if already playing, stop it before playing again
				mouth.Stop ();
			mouth.volume = soundVolume; //set the volume each time, in case it has been changed
			mouth.Play();
			
		}
	}
	
	public void InputRouter() {
//		if (playMode == PlayMode.Play) {
//				currentVal = animLerpVal;
//		} else 
		if (playMode == PlayMode.Animate) {
			if (creatureType == CreatureType.Unicorn) {
				switch (mode) {
					case Modes.Talk: {
						currentVal = rightTrigger;
						currentBuffer = talkBuffer;
						break;
					}
					case Modes.Barf: {
						currentVal = rightTrigger;
						currentBuffer = barfBuffer;
						break;
					}
					case Modes.Munch: {
						currentVal = rightTrigger;
						currentBuffer = munchBuffer;
						break;
					}
				}
			} else {
				switch (mode) {
					case Modes.Talk: {
						currentVal = leftTrigger;
						currentBuffer = talkBuffer;
						break;
					}
					case Modes.Barf: {
						currentVal = leftTrigger;
						currentBuffer = barfBuffer;
						break;
					}
					case Modes.Munch: {
						currentVal = leftTrigger;
						currentBuffer = munchBuffer;
						break;
					}
				}
			}
		}
	}

//	public void InputRouter(float inputVal) {
//
//		switch (mode) {
//			case Modes.Talk: {
//				if (talkBuffer.Count < index)
//					talkBuffer.Add(0);
//				if (record) {
//					if (Time.time > currentTime) {
//						talkBuffer[index] = inputVal;
//					}
//				}
//			}
//			case Modes.Barf: {
//				if (barfBuffer.Count < index)
//					barfBuffer.Add(0);
//				if (record) {
//					if (Time.time > currentTime) {
//						barfBuffer[index] = inputVal;
//					}
//				}
//			}
//		}
//	}
}
