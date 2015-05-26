using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Radial : MonoBehaviour {
	public enum RadialMode{
		RadialMenu, Blending
	}

	public int id;
	public RadialMode radialMode;
	public AnimMode animMode;
	public Vector3 rightAnalog;
	private List<string> radialNames;
	private List<string> animNames;
	private float segmentDegree;
	private GUIText radialText;
	public GUIText raidalModeText;

	private float mag;
	private float textVisibleTime = 1.5f;
	private float radialModeFade;
	private float radialAnimFade;

	// Use this for initialization
	void Start () {
		radialNames = new List<string>(Enum.GetNames(typeof(RadialMode)));
		animNames = new List<string>(Enum.GetNames(typeof(AnimMode)));
		segmentDegree = 360/animNames.Count;

		GameObject textObj = new GameObject();
		textObj.transform.position = this.transform.position;
		textObj.transform.parent = this.transform;
		radialText = textObj.AddComponent<GUIText>();
		radialText.font = (Font)Resources.Load("Funny2", typeof(Font));
		radialText.fontSize = 40;

		GameObject textModeObj = new GameObject();
		textModeObj.transform.position = new Vector3 (0.4f, 1, 0);
		raidalModeText = textModeObj.AddComponent<GUIText>();
		raidalModeText.font = (Font)Resources.Load("Funny2", typeof(Font));
		raidalModeText.fontSize = 35;

		;
	}
	
	// Update is called once per frame
	void Update () {
		float rightAnalogX = ControllerInput.RightAnalog_X_Axis(id);
		float rightAnalogY = ControllerInput.RightAnalog_Y_Axis(id);
		rightAnalog = new Vector3 (rightAnalogX, rightAnalogY, 0);
		mag = rightAnalog.magnitude;
		rightAnalog = rightAnalog.normalized;
		//Debug.Log(rightAnalog);

		ChangeMenuMode();
		if (radialMode == RadialMode.RadialMenu) {
			ChangeAnimMode();
		} else if(radialMode == RadialMode.Blending) {

		}
	}
	private void ChangeAnimMode() {
		if (mag> 0.4f) {
			float degree = XYToDegrees(rightAnalog, Vector3.zero);
			//Debug.Log(degree);
			//Debug.Log((degree/segmentDegree));
			animMode = (AnimMode)(degree/segmentDegree);
			radialText.text = animMode.ToString();
			radialAnimFade = Time.time;
		}
		if (radialAnimFade < (Time.time - textVisibleTime)) 
			radialText.gameObject.SetActive(false);
		else
			radialText.gameObject.SetActive(true);
				
	}
	private void ChangeMenuMode() {
		bool leftBumper = ControllerInput.Left_Bumper_ButtonDown(id);
		bool rightBumper = ControllerInput.Right_Bumper_ButtonDown(id);

		int modeInt = (int)radialMode;
		if (leftBumper) {
			if (modeInt == 0) 
				radialMode = (RadialMode)(radialNames.Count-1);
			else
				radialMode--;
			raidalModeText.text = radialMode.ToString();
			radialModeFade = Time.time;
			
		}
		if (rightBumper) {
			if (modeInt >= radialNames.Count) 
				radialMode = (RadialMode)(0);
			else
				radialMode++;
			raidalModeText.text = radialMode.ToString();
			radialModeFade = Time.time;
		}
		if (radialModeFade < (Time.time - textVisibleTime)) 
			raidalModeText.gameObject.SetActive(false);
		else
			raidalModeText.gameObject.SetActive(true);
	}
	private float XYToDegrees(Vector3 xy, Vector3 origin)
	{
		float deltaX = origin.x - xy.x;
		float deltaY = origin.y - xy.y;
		//double radAngle = Mathf.Atan2(deltaY, deltaX);
		double radAngle = Mathf.Atan2(-deltaY, deltaX);
		double degreeAngle = radAngle * 180.0 / Mathf.PI;
		
		return (float)(180.0 - degreeAngle);
	}
}
