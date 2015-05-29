using UnityEngine;
using System.Collections;

public class AutomationMenu : MonoBehaviour {
	public int id;
	private Animator animator;
	GUIText indexGUI;
	GUIText loopPositionGUI;
	GUIText playGUI;
	GUIText recordGUI;
	GUIText loopingGUI;

	public float spacing = 0.1f;
	public int fontSize = 16;

	public bool showAutomation = true;
	// Use this for initialization
	void Start () {
		animator = GameObject.Find("Player" + id).GetComponent<Animator>();
		indexGUI = CreateGUIText(0, 5*spacing);
		loopPositionGUI = CreateGUIText(0, 4*spacing);
		playGUI = CreateGUIText(0, 3*spacing);
		recordGUI = CreateGUIText(0, 2*spacing);
		loopingGUI = CreateGUIText(0, 1*spacing);
	}
	
	// Update is called once per frame
	void Update () {
		if (ControllerInput.RightAnalog_ClickDown(id))
			ToggleVisibility();
		RenderAutomation();
	}

	private void RenderAutomation() {
		indexGUI.text = "Index: " + animator.index;
		loopPositionGUI.text = "Loop: " + animator.loopPos;
		playGUI.text = "Playing: " + animator.play;
		recordGUI.text = "Recording: " + animator.record;
		loopingGUI.text = "Looping: " + animator.looping;
	}

	private GUIText CreateGUIText(float offsetX, float offsetY) {
		GameObject textObj = new GameObject();
		textObj.transform.position = this.transform.position + new Vector3(offsetX, offsetY, 0);
		textObj.transform.parent = this.transform;
		GUIText radialText = textObj.AddComponent<GUIText>();
		radialText.font = (Font)Resources.Load("Funny2", typeof(Font));
		radialText.fontSize = fontSize;
		radialText.alignment = TextAlignment.Right;
		return radialText;
	}
	private void ToggleVisibility() {
		showAutomation = !showAutomation;
		indexGUI.gameObject.SetActive(showAutomation);
		loopPositionGUI.gameObject.SetActive(showAutomation);
		playGUI.gameObject.SetActive(showAutomation);
		recordGUI.gameObject.SetActive(showAutomation);
		loopingGUI.gameObject.SetActive(showAutomation);
	}
}
