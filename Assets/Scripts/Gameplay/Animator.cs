using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Modes{
	Talk, Barf, Munch
}
public class Animator : MonoBehaviour {
	//Moving Parts
	public GameObject unicorn;
	private GameObject rightHead;
	private GameObject rightHeadHinge;
	private GameObject rightNeck;
	private GameObject rightNeckHinge;
	private GameObject rightMouthTop;
	private GameObject rightMouthTopHinge;
	private GameObject rightMouthBottom;
	private GameObject rightMouthBottomHinge;

	public GameObject whale;
	private GameObject leftHead;
	private GameObject leftHeadHinge;
	private GameObject leftNeck;
	private GameObject leftNeckHinge;
	private GameObject leftMouthTop;
	private GameObject leftMouthTopHinge;
	private GameObject leftMouthBottom;
	private GameObject leftMouthBottomHinge;

	private Vector3 uniPos;
	private Vector3 whalePos;
	public float moveSpeed = 0.002f;
	public float lerpSpeed = 0.3f;

	//Input
	public float rightTrigger;
	public float leftTrigger;
	public float rightAnalogV;
	public float rightAnalogH;
	public float leftAnalogV;
	public float leftAnalogH;
	public bool DpadH = false;
	private float triggerDebounce = 0.02f;
	public float maxTalkRotateAngle = 25;
	public float maxBarfRotateAngle = 30;
	public float maxMunchRotateAngle = 30;

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

	// Use this for initialization
	void Start () {
		currentAnim = new AnimationDelegate[]{Talk, Barf, Munch};
//		animationGlobe.Add(talkBuffer);
//		animationGlobe.Add(barfBuffer);

		uniPos = new Vector3(unicorn.transform.position.x, unicorn.transform.position.y, unicorn.transform.position.z);
		whalePos = new Vector3(whale.transform.position.x, whale.transform.position.y, whale.transform.position.z);

		//buffer = new float[];
		currentBuffer.Insert(index, 0);
		talkBuffer.Insert(index, 0);
		barfBuffer.Insert(index, 0);
		munchBuffer.Insert(index, 0);
		rightTriggerBuffer.Insert(index, 0);
		rightAnalogVBuffer.Insert(index, 0);
		Debug.Log(rightTriggerBuffer[index]);

		rightHead = Traversals.TraverseHierarchy(unicorn.transform, "Head").gameObject;
		rightHeadHinge = Traversals.TraverseHierarchy(unicorn.transform, "HeadHinge").gameObject;
		rightNeck = Traversals.TraverseHierarchy(unicorn.transform, "Neck").gameObject;
		rightNeckHinge = Traversals.TraverseHierarchy(unicorn.transform, "NeckHinge").gameObject;
		rightMouthTop = Traversals.TraverseHierarchy(unicorn.transform, "MouthTop").gameObject;
		rightMouthTopHinge = Traversals.TraverseHierarchy(unicorn.transform, "MouthTopHinge").gameObject;
		rightMouthBottom = Traversals.TraverseHierarchy(unicorn.transform, "MouthBottom").gameObject;
		rightMouthBottomHinge = Traversals.TraverseHierarchy(unicorn.transform, "MouthBottomHinge").gameObject;

		leftHead = Traversals.TraverseHierarchy(whale.transform, "Head").gameObject;
		leftHeadHinge = Traversals.TraverseHierarchy(whale.transform, "HeadHinge").gameObject;
		leftNeck = Traversals.TraverseHierarchy(whale.transform, "Neck").gameObject;
		leftNeckHinge = Traversals.TraverseHierarchy(whale.transform, "NeckHinge").gameObject;
		leftMouthTop = Traversals.TraverseHierarchy(whale.transform, "MouthTop").gameObject;
		leftMouthTopHinge = Traversals.TraverseHierarchy(whale.transform, "MouthTopHinge").gameObject;
		leftMouthBottom = Traversals.TraverseHierarchy(whale.transform, "MouthBottom").gameObject;
		leftMouthBottomHinge = Traversals.TraverseHierarchy(whale.transform, "MouthBottomHinge").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		InputControls();
		PlaybackControls();
		MovementControls();

		currentAnim[(int) mode](currentVal, 0);
		//Talk(0, currentBuffer[index]);
		//Barf(0, currentBuffer[index]);

	}

	public void MovementControls() {
		uniPos += new Vector3(Input.GetAxisRaw("RightAnalog_H"), -Input.GetAxisRaw("RightAnalog_V")) * 0.2f;
		whalePos += new Vector3(Input.GetAxisRaw("LeftAnalog_H"), -Input.GetAxisRaw("LeftAnalog_V")) * 0.2f;

		if (Input.GetKey(KeyCode.UpArrow))
			uniPos += Vector3.up * moveSpeed;
		if (Input.GetKey(KeyCode.RightArrow))
			uniPos += Vector3.right * moveSpeed;
		if (Input.GetKey(KeyCode.DownArrow))
			uniPos += Vector3.down * moveSpeed;
		if (Input.GetKey(KeyCode.LeftArrow))
			uniPos += Vector3.left * moveSpeed;

		if (uniPos.y > 2.8f)
			uniPos = new Vector3(uniPos.x, 2.8f, uniPos.z);
		if (uniPos.y <  -5.1f)
			uniPos = new Vector3(uniPos.x, -5.1f, uniPos.z);
		if (uniPos.x > 6)
			uniPos = new Vector3(6, uniPos.y, uniPos.z);
		if (uniPos.x < -10)
			uniPos = new Vector3(-10, uniPos.y, uniPos.z);

		if (Input.GetKeyDown(KeyCode.W))
			whalePos += Vector3.up * moveSpeed;
		if (Input.GetKeyDown(KeyCode.D))
			whalePos += Vector3.right * moveSpeed;
		if (Input.GetKeyDown(KeyCode.S))
			whalePos += Vector3.down * moveSpeed;
		if (Input.GetKeyDown(KeyCode.A))
			whalePos += Vector3.left * moveSpeed;

		if (whalePos.y > 2.8f)
			whalePos = new Vector3(whalePos.x, 2.8f, whalePos.z);
		if (whalePos.y <  -5.1f)
			whalePos = new Vector3(whalePos.x, -5.1f, whalePos.z);
		if (whalePos.x > 6)
			whalePos = new Vector3(6, whalePos.y, whalePos.z);
		if (whalePos.x < -10)
			whalePos = new Vector3(-10, whalePos.y, whalePos.z);

		unicorn.transform.position = Vector3.Lerp(unicorn.transform.position, uniPos, lerpSpeed * Time.deltaTime);
		whale.transform.position = Vector3.Lerp(whale.transform.position, whalePos, lerpSpeed * Time.deltaTime);
		
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

		Quaternion rightNeckHingeRot = Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(inputVal) * 0.1f, transform.forward);
		Quaternion rightHeadHingeRot = rightNeckHingeRot * Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(inputVal) * 0.2f , transform.forward);
		Quaternion rightMouthBottomHingeRot = rightHeadHingeRot * Quaternion.AngleAxis(maxTalkRotateAngle * Mathf.Abs(inputVal) * 1f, transform.forward);
		Quaternion rightMouthTopHingeRot = rightHeadHingeRot * Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(inputVal) * 0.5f , transform.forward);
		
		rightNeckHinge.transform.rotation =  rightNeckHingeRot;
		rightHeadHinge.transform.rotation =  rightHeadHingeRot;
		rightMouthBottomHinge.transform.rotation = rightMouthBottomHingeRot;
		rightMouthTopHinge.transform.rotation = rightMouthTopHingeRot;


		leftMouthTopHinge.transform.rotation = Quaternion.AngleAxis(maxTalkRotateAngle * Mathf.Abs(leftTrigger) * 0.5f , transform.forward);
		leftMouthBottomHinge.transform.rotation = Quaternion.AngleAxis(-maxTalkRotateAngle * Mathf.Abs(leftTrigger) * 0.75f , transform.forward);
		leftHeadHinge.transform.rotation = Quaternion.AngleAxis(maxTalkRotateAngle * Mathf.Abs(leftTrigger) * 0.2f , transform.forward);
		leftNeckHinge.transform.rotation = Quaternion.AngleAxis(maxTalkRotateAngle * Mathf.Abs(leftTrigger) * 0.1f, transform.forward);
		//Bob
		unicorn.transform.position = new Vector3(unicorn.transform.position.x, unicorn.transform.position.y + Mathf.Sin(Time.time)*0.0015f, unicorn.transform.position.z);
		whale.transform.position = new Vector3(whale.transform.position.x, whale.transform.position.y + Mathf.Sin(Time.time)*0.0015f, whale.transform.position.z);
	}

	public void Barf(float inputVal, float existingVal) {
		inputVal = Mathf.Clamp(inputVal + existingVal, 0, 1);

		Quaternion rightNeckHingeRot = Quaternion.AngleAxis(maxBarfRotateAngle * Mathf.Abs(inputVal) * 1.3f, transform.forward);
		Quaternion rightHeadHingeRot = rightNeckHingeRot * Quaternion.AngleAxis(-maxBarfRotateAngle * Mathf.Abs(inputVal) * 1.2f , transform.forward);
		Quaternion rightMouthBottomHingeRot = rightHeadHingeRot * Quaternion.AngleAxis(maxBarfRotateAngle * Mathf.Abs(inputVal) * 0.7f, transform.forward);
		Quaternion rightMouthTopHingeRot = rightHeadHingeRot * Quaternion.AngleAxis(-maxBarfRotateAngle * Mathf.Abs(inputVal) * 0.3f , transform.forward);

		rightNeckHinge.transform.rotation =  rightNeckHingeRot;
		rightHeadHinge.transform.rotation =  rightHeadHingeRot;
		rightMouthBottomHinge.transform.rotation = rightMouthBottomHingeRot;
		rightMouthTopHinge.transform.rotation = rightMouthTopHingeRot;
		
		leftMouthTopHinge.transform.rotation = Quaternion.AngleAxis(maxBarfRotateAngle * Mathf.Abs(leftTrigger) * 0.5f , transform.forward);
		leftMouthBottomHinge.transform.rotation = Quaternion.AngleAxis(-maxBarfRotateAngle * Mathf.Abs(leftTrigger) * 0.75f , transform.forward);
		leftHeadHinge.transform.rotation = Quaternion.AngleAxis(maxBarfRotateAngle * Mathf.Abs(leftTrigger) * 0.2f , transform.forward);
		leftNeckHinge.transform.rotation = Quaternion.AngleAxis(maxBarfRotateAngle * Mathf.Abs(leftTrigger) * 0.1f, transform.forward);
		//Bob
		unicorn.transform.position = new Vector3(unicorn.transform.position.x, unicorn.transform.position.y + Mathf.Sin(Time.time)*0.0015f, unicorn.transform.position.z);
		whale.transform.position = new Vector3(whale.transform.position.x, whale.transform.position.y + Mathf.Sin(Time.time)*0.0015f, whale.transform.position.z);
	}
	public void Munch(float inputVal, float existingVal) {
		inputVal = Mathf.Clamp(inputVal + existingVal, 0, 1);
		
		Quaternion rightNeckHingeRot = Quaternion.AngleAxis(maxBarfRotateAngle * Mathf.Abs(inputVal) * 0.7f, transform.forward);
		Quaternion rightHeadHingeRot = rightNeckHingeRot * Quaternion.AngleAxis(-maxBarfRotateAngle * Mathf.Abs(inputVal) * 0.8f , transform.forward);
		Quaternion rightMouthBottomHingeRot = rightHeadHingeRot * Quaternion.AngleAxis(-maxBarfRotateAngle * Mathf.Abs(inputVal) * 0.55f, transform.forward);
		Quaternion rightMouthTopHingeRot = rightHeadHingeRot * Quaternion.AngleAxis(maxBarfRotateAngle * Mathf.Abs(inputVal) * 0.2f , transform.forward);
		
		rightNeckHinge.transform.rotation =  rightNeckHingeRot;
		rightHeadHinge.transform.rotation =  rightHeadHingeRot;
		rightMouthBottomHinge.transform.rotation = rightMouthBottomHingeRot;
		rightMouthTopHinge.transform.rotation = rightMouthTopHingeRot;
	}

	
	public void InputRouter() {
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
