using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum AnimMode{
	Talk, Barf, Munch, Up, Down
}
public enum CreatureType{
	Unicorn, Whale
}
public enum PlayMode {
	Play, Animate
}
public class Animator : MonoBehaviour {
	private string savePath;

	public int id;

	//Moving Parts
	public GameObject creature;
	private GameObject head;
	private GameObject headHinge;
	private GameObject neck;
	private GameObject neckHinge;
	private GameObject mouthTop;
	private GameObject mouthTopHinge;
	private GameObject mouthBottom;
	private GameObject mouthBottomHinge;

	//Radial Menu
	private Radial radialMenu;
	
	//Rotations
	private Quaternion neckHingeRot;
	private Quaternion headHingeRot;
	private Quaternion mouthBottomHingeRot;
	private Quaternion mouthTopHingeRot;
	
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
	public float rightAnalogX;
	public float rightAnalogY;
	public float leftAnalogX;
	public float leftAnalogY;
	public Vector3 rightAnalog;
	public Vector3 leftAnalog;
	public bool DpadH = false;
	public bool DpadV = false;
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
	public Dictionary<string, List<float>> animationContainer = new Dictionary<string, List<float>>();
	public List<AnimationContainer> animContainers = new List<AnimationContainer>();
	public List<float> currentBuffer = new List<float>(1000);
	public List<float> talkBuffer = new List<float>(1000);
	public List<float> barfBuffer = new List<float>(1000);
	public List<float> munchBuffer = new List<float>(1000);
	public List<float> upBuffer = new List<float>(1000);
	public List<float> downBuffer = new List<float>(1000);
	public int index = 0;
	public float currentVal = 0;
	public int loopPos = 999999999;
	public bool play = false;
	public bool record = false;
	public bool looping = false;
	public float interval = 0.01f;
	public float currentTime = 0f;
	public int animIndex = 0;
	
	// Delegates
	private AnimationDelegate[] currentAnim;
	private List<List<float>> savedAnimBuffers;
	
	public AnimMode mode = AnimMode.Talk;
	public PlayMode playMode = PlayMode.Play;
	public CreatureType creatureType = CreatureType.Unicorn;
	public int animSign;
	
	// Use this for initialization
	void Start () {
		creature = this.gameObject;
		savePath = Application.dataPath;
		Array animFunctions = Enum.GetValues(typeof(AnimMode));
		for (int i=0; i<animFunctions.Length; i++) {
			animationContainer.Add(((AnimMode)i).ToString("f"), new List<float>());
			animationContainer[((AnimMode)i).ToString("f")].Insert(index, 0);
		}

		radialMenu = GameObject.Find("Radial" + id).GetComponent<Radial>();
		
		talkBuffer.Insert(index, 0);
		barfBuffer.Insert(index, 0);
		munchBuffer.Insert(index, 0);
		upBuffer.Insert(index, 0);
		downBuffer.Insert(index, 0);
		currentBuffer.Insert(index, 0);
		
		currentAnim = new AnimationDelegate[]{Talk, Barf, Munch, Up, Down};
		savedAnimBuffers = new List<List<float>>{talkBuffer, barfBuffer, munchBuffer, upBuffer, downBuffer};
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

		
		head = Traversals.TraverseHierarchy(creature.transform, "Head").gameObject;
		headHinge = Traversals.TraverseHierarchy(creature.transform, "HeadHinge").gameObject;
		neck = Traversals.TraverseHierarchy(creature.transform, "Neck").gameObject;
		neckHinge = Traversals.TraverseHierarchy(creature.transform, "NeckHinge").gameObject;
		mouthTop = Traversals.TraverseHierarchy(creature.transform, "MouthTop").gameObject;
		mouthTopHinge = Traversals.TraverseHierarchy(creature.transform, "MouthTopHinge").gameObject;
		mouthBottom = Traversals.TraverseHierarchy(creature.transform, "MouthBottom").gameObject;
		mouthBottomHinge = Traversals.TraverseHierarchy(creature.transform, "MouthBottomHinge").gameObject;

		neckHingeRot = Quaternion.AngleAxis(0, transform.forward);
		headHingeRot = Quaternion.AngleAxis(0, transform.forward);
		mouthBottomHingeRot = Quaternion.AngleAxis(0, transform.forward);
		mouthTopHingeRot = Quaternion.AngleAxis(0, transform.forward);

		audioNode = new GameObject();
		mouth = audioNode.AddComponent<AudioSource>();
		mouth.clip = munch;
		audioNode.transform.parent = transform;
	}
	
	// Update is called once per frame
	void Update () {
		
		neckHingeRot = Quaternion.AngleAxis(0, transform.forward);
		headHingeRot = Quaternion.AngleAxis(0, transform.forward);
		mouthBottomHingeRot = Quaternion.AngleAxis(0, transform.forward);
		mouthTopHingeRot = Quaternion.AngleAxis(0, transform.forward);

		if (radialMenu.radialMode == Radial.RadialMode.RadialMenu) {
			mode = radialMenu.animMode;
		} else if (radialMenu.radialMode == Radial.RadialMode.RadialMenu) {

		}

		InputControls();
		PlaybackControls();
		MovementControls();
		
		//currentAnim[(int) mode](currentVal, 0);
		
		AnimationSumation();
		
		//Talk(0, currentBuffer[index]);
		//Barf(0, currentBuffer[index]);
		
		//Bob
		creature.transform.position = new Vector3(creature.transform.position.x, creature.transform.position.y + Mathf.Sin(Time.time)*0.0015f, creature.transform.position.z);
	}
	
	public void MovementControls() {
//		if (creatureType == CreatureType.Unicorn) {
//			creaturePos += new Vector3(Input.GetAxisRaw("RightAnalog_H"), -Input.GetAxisRaw("RightAnalog_V")) * 0.2f;
//		} else {
//			creaturePos += new Vector3(Input.GetAxisRaw("LeftAnalog_H"), -Input.GetAxisRaw("LeftAnalog_V")) * 0.2f;
//		}
		
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
//		Debug.Log (leftAnalog);
		creaturePos += leftAnalog * moveSpeed;
		
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
				mode = AnimMode.Munch;
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
				mode = AnimMode.Barf;
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
		rightTrigger = ControllerInput.RightTriggerAxis(id);
		Debug.Log (id);
		Debug.Log (rightTrigger);
		leftTrigger =  ControllerInput.LeftTriggerAxis(id);
		rightTrigger = ExtensionMethods.Remap(rightTrigger, 0.5f, 0, 0, 1);
		leftTrigger = ExtensionMethods.Remap(leftTrigger, 0.5f, 0, 1, 1);

		rightAnalogY =  ControllerInput.RightAnalog_X_Axis(id, 0.2f);
		rightAnalogX =  ControllerInput.RightAnalog_Y_Axis(id, 0.2f);
		leftAnalogX =  ControllerInput.LeftAnalog_X_Axis(id, 0.2f);
		leftAnalogY =  ControllerInput.LeftAnalog_Y_Axis(id, 0.2f);
	
		rightAnalogX = ExtensionMethods.Remap(rightAnalogX, 0, 1, 0, 1);
		rightAnalogY = ExtensionMethods.Remap(rightAnalogY, 0, 1, 0, 1);
		leftAnalogX = ExtensionMethods.Remap(leftAnalogX, 0, 1, 0, 1);
		leftAnalogY = ExtensionMethods.Remap(leftAnalogY, 0, 1, 0, 1);

		rightAnalog = new Vector3(rightAnalogX, rightAnalogY, 0);
		leftAnalog = new Vector3(leftAnalogX, leftAnalogY, 0);

		//		rightAnalogH = ExtensionMethods.Remap(Input.GetAxisRaw("RightAnalog_H"), 0, 1, 0, 1);
		//		leftAnalogV = ExtensionMethods.Remap(Input.GetAxisRaw("LefttAnalog_V"), 0, 1, 0, 1);
		//		leftAnalogH =ExtensionMethods.Remap(Input.GetAxisRaw("LeftAnalog_H"), 0, 1, 0, 1);
		//		else
		//			rightAnalogV = 0;
	}
	
	public void PlaybackControls() {
		//Playback Controls
		InputRouter();
		if (ControllerInput.A_ButtonDown(id))
			play = !play;
		if (play) {
			if (Time.time > currentTime) {
				if (currentBuffer.Count > index) {
					//currentBuffer.Add(0); 
					for (int i=0; i < Enum.GetName(typeof(AnimMode), 0).Length -1; i++) {
						savedAnimBuffers[i].Add(0);
					}
				}
				index++;
			}
		}
		//Clear
		if (ControllerInput.RightAnalog_Click(id)) {
			currentVal = 0;
		}
		//Record
		if (ControllerInput.B_ButtonDown(id))
			record = !record;
		if (record) {
			if (Time.time > currentTime) {
				currentBuffer[index] = currentVal;
			}
		}
		//Reset
		if (ControllerInput.X_ButtonDown(id)) {
			index = 0;
		}
		//Set Loop
		if (ControllerInput.Y_ButtonDown(id)) {
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
//		if (Input.GetAxisRaw("xbox_DPad_H") == 0)
//			DpadH = true;
//		if (Input.GetAxisRaw("xbox_DPad_H") > 0.5f) {
//			//			Debug.Log(mode);
//			if (DpadH) {
		//				if ((animIndex + 3) > Enum.GetName(typeof(AnimMode), 0).Length)
//					animIndex = 0;
//				else
//					animIndex ++;
		//				mode = (AnimMode) animIndex;
//				Debug.Log(mode);
//			}
//			DpadH = false;
//		}
		//Save Animation Buffers
//		if (Input.GetAxisRaw("xbox_DPad_V") == 0)
//			DpadV = true;
//		if (Input.GetAxisRaw("xbox_DPad_V") > 0.5f) {
//			if (DpadV) {
//				AnimationContainer animCont = new AnimationContainer(savedAnimBuffers, savePath);
//				animCont.Save(savePath);
//				animContainers.Add(animCont);
//			}
//			DpadV = false;
//		}



		if (Time.time > currentTime) {
			currentTime += interval;
		}
		//Debug.Log(currentVal);
//		Debug.Log(mode);
		currentVal = (Mathf.Clamp(currentVal + currentBuffer[index], 0, 1));
		//rightTriggerBuffer[index] = currentVal;
		//rightTrigger = Mathf.Clamp(rightTrigger + rightTriggerBuffer[index], 0, 1);
	}
	
	public void Talk(float inputVal, float existingVal) {
		inputVal = Mathf.Clamp(inputVal + existingVal, 0, 1);
		
		//		Debug.Log(inputVal);
		//		rightmouthTopHinge.transform.rotation = Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(inputVal) * 0.5f , transform.forward);
		//		rightmouthBottomHinge.transform.rotation = Quaternion.AngleAxis(maxTalkRotateAngle * Mathf.Abs(inputVal) , transform.forward);
		//		rightheadHinge.transform.rotation = Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(inputVal) * 0.2f , transform.forward);
		//		rightneckHinge.transform.rotation = Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(inputVal) * 0.1f, transform.forward);
		
		Quaternion talkNeckHingeRot = Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(inputVal) * 0.1f * animSign, transform.forward);
		Quaternion talkHeadHingeRot = talkNeckHingeRot * Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(inputVal) * 0.2f * animSign, transform.forward);
		Quaternion talkMouthBottomHingeRot = talkHeadHingeRot * Quaternion.AngleAxis(maxTalkRotateAngle * Mathf.Abs(inputVal) * 1f * animSign, transform.forward);
		Quaternion talkMouthTopHingeRot = talkHeadHingeRot * Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(inputVal) * 0.5f * animSign, transform.forward);

		neckHingeRot *= talkNeckHingeRot;
		headHingeRot *= talkHeadHingeRot;
		mouthBottomHingeRot *= talkMouthBottomHingeRot;
		mouthTopHingeRot *= talkMouthTopHingeRot;

//		neckHinge.transform.rotation =  neckHingeRot;
//		headHinge.transform.rotation =  headHingeRot;
//		mouthBottomHinge.transform.rotation = mouthBottomHingeRot;
//		mouthTopHinge.transform.rotation = mouthTopHingeRot;
	}
	
	public void Barf(float inputVal, float existingVal) {
		inputVal = Mathf.Clamp(inputVal + existingVal, 0, 1);
		
		Quaternion barfNeckHingeRot = Quaternion.AngleAxis(maxBarfRotateAngle * Mathf.Abs(inputVal) * 1.3f * animSign, transform.forward);
		Quaternion barfHeadHingeRot = barfNeckHingeRot * Quaternion.AngleAxis(-maxBarfRotateAngle * Mathf.Abs(inputVal) * 1.2f * animSign, transform.forward);
		Quaternion barfMouthBottomHingeRot = barfHeadHingeRot * Quaternion.AngleAxis(maxBarfRotateAngle * Mathf.Abs(inputVal) * 0.7f * animSign, transform.forward);
		Quaternion barfMouthTopHingeRot = barfHeadHingeRot * Quaternion.AngleAxis(-maxBarfRotateAngle * Mathf.Abs(inputVal) * 0.3f * animSign, transform.forward);
	
		neckHingeRot *= barfNeckHingeRot;
		headHingeRot *= barfHeadHingeRot;
		mouthBottomHingeRot *= barfMouthBottomHingeRot;
		mouthTopHingeRot *= barfMouthTopHingeRot;
	}
	public void Munch(float inputVal, float existingVal) {
		inputVal = Mathf.Clamp(inputVal + existingVal, 0, 1);
		
		Quaternion munchNeckHingeRot = Quaternion.AngleAxis(maxMunchRotateAngle * Mathf.Abs(inputVal) * 0.7f * animSign, transform.forward);
		Quaternion munchHeadHingeRot = munchNeckHingeRot * Quaternion.AngleAxis(-maxMunchRotateAngle * Mathf.Abs(inputVal) * 0.8f * animSign, transform.forward);
		Quaternion munchMouthBottomHingeRot = munchHeadHingeRot * Quaternion.AngleAxis(-maxMunchRotateAngle * Mathf.Abs(inputVal) * 0.55f * animSign, transform.forward);
		Quaternion munchMouthTopHingeRot = munchHeadHingeRot * Quaternion.AngleAxis(maxMunchRotateAngle * Mathf.Abs(inputVal) * 0.2f * animSign, transform.forward);
		
		neckHingeRot *= munchNeckHingeRot;
		headHingeRot *= munchHeadHingeRot;
		mouthBottomHingeRot *= munchMouthBottomHingeRot;
		mouthTopHingeRot *= munchMouthTopHingeRot;
	}
	public void Up(float inputVal, float existingVal) {
	}
	public void Down(float inputVal, float existingVal) {
	}
	
	public void AnimationSumation() {

		if (radialMenu.radialMode == Radial.RadialMode.Blending) {
			Blending();
		} else if(radialMenu.radialMode == Radial.RadialMode.RadialMenu) {
			for (int i=0; i < Enum.GetName(typeof(AnimMode), 0).Length -1; i++) {
				float val = 0;
				if (i == (int)mode)
					val = currentVal;
				//Debug.Log("index: " + i + " currentVal: " + val);
				currentAnim[i](val, savedAnimBuffers[i][index]);
			}
		}

		//currentAnim[(int) mode](currentVal, 0);

		neckHinge.transform.rotation =  neckHingeRot;
		headHinge.transform.rotation =  headHingeRot;
		mouthBottomHinge.transform.rotation = mouthBottomHingeRot;
		mouthTopHinge.transform.rotation = mouthTopHingeRot;
		
		//		neckHingeRot = new Quaternion();
		//		headHingeRot = new Quaternion();
		//		mouthBottomHingeRot = new Quaternion();
		//		mouthTopHingeRot = new Quaternion();
	}

	public void Blending() {
		float rightAnalogY =  ControllerInput.RightAnalog_Y_Axis(id, 0.2f);
		float rightAnalogX =  ControllerInput.RightAnalog_X_Axis(id, 0.2f);

		if (rightAnalogX > 0) {
			rightAnalogX = ExtensionMethods.Remap(rightAnalogX, 0, 1, 0, 1);
			currentAnim[(int)AnimMode.Munch](rightAnalogX, savedAnimBuffers[(int)AnimMode.Munch][index]);
		} else {
			rightAnalogX = ExtensionMethods.Remap(rightAnalogX, 0, -1, 0, 1);
			currentAnim[(int)AnimMode.Talk](rightAnalogX, savedAnimBuffers[(int)AnimMode.Talk][index]);
		}
		if (rightAnalogY > 0) {
			rightAnalogY = ExtensionMethods.Remap(rightAnalogY, 0, 1, 0, 1);
			currentAnim[(int)AnimMode.Up](rightAnalogY, savedAnimBuffers[(int)AnimMode.Up][index]);
		} else {
			rightAnalogY = ExtensionMethods.Remap(rightAnalogY, 0, -1, 0, 1);
			currentAnim[(int)AnimMode.Down](rightAnalogY, savedAnimBuffers[(int)AnimMode.Down][index]);
		}

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
		if (playMode == PlayMode.Play) {
			currentVal = animLerpVal;
		} else 
		if (playMode == PlayMode.Animate) {
			if (creatureType == CreatureType.Unicorn) {
				switch (mode) {
				case AnimMode.Talk: {
					currentVal = rightTrigger;
					currentBuffer = talkBuffer;
					break;
				}
				case AnimMode.Barf: {
					currentVal = rightTrigger;
					currentBuffer = barfBuffer;
					break;
				}
				case AnimMode.Munch: {
					currentVal = rightTrigger;
					currentBuffer = munchBuffer;
					break;
				}
				}
			} else {
				switch (mode) {
				case AnimMode.Talk: {
					currentVal = rightTrigger;
					currentBuffer = talkBuffer;
					break;
				}
				case AnimMode.Barf: {
					currentVal = rightTrigger;
					currentBuffer = barfBuffer;
					break;
				}
				case AnimMode.Munch: {
					currentVal = rightTrigger;
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
