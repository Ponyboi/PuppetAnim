using UnityEngine;
using System.Collections;
using System;

public class GUI : MonoBehaviour {

	public GUIText player1Shrimp;
	public GUIText player2Shrimp;
	public GUIText counterText;
	public GUIText titleText;
	public GUIText winnerText;
	public GUIText controlsText;
	public Animator unicorn;
	public Animator whale;
	public Eat uniEat;
	public Eat whaleEat;

	GameObject audioNode;
	public AudioSource audioSource;
	public AudioClip music;

	public float counter = 30;
	public float timeCounter = 1;
	public String winner = "";

	public enum State {
		Controls, Play, Win, Exit
	}
	public State gameState = State.Controls;

	// Use this for initialization
	void Start () {
		player1Shrimp = Traversals.TraverseHierarchy(transform, "Player1Shrimp").GetComponent<GUIText>();
		player2Shrimp = Traversals.TraverseHierarchy(transform, "Player2Shrimp").GetComponent<GUIText>();
		counterText = Traversals.TraverseHierarchy(transform, "CounterText").GetComponent<GUIText>();
		titleText = Traversals.TraverseHierarchy(transform, "Title").GetComponent<GUIText>();
		winnerText = Traversals.TraverseHierarchy(transform, "Winner").GetComponent<GUIText>();
		controlsText = Traversals.TraverseHierarchy(transform, "Controls").GetComponent<GUIText>();
		unicorn = GameObject.Find("Unicorn").GetComponent<Animator>();
		whale = GameObject.Find("Whale").GetComponent<Animator>();
		uniEat = Traversals.TraverseHierarchy(unicorn.transform, "Eat").GetComponent<Eat>();
		whaleEat = Traversals.TraverseHierarchy(whale.transform, "Eat").GetComponent<Eat>();

		GameObject audioNode = new GameObject();
		audioSource = audioNode.AddComponent<AudioSource>();
		//audioSource = new AudioSource();
		audioSource.clip = music;
		audioSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
		switch (gameState) {
		case State.Controls:
			player1Shrimp.text = "";
			player2Shrimp.text = "";
			counterText.text = "";
			titleText.text = "Skateboard Dogs";
			winnerText.text = "";
			controlsText.text = "WASD and arrow keys to move \n Tap 'SPACE' and 'ENTER' to munch shrimp \n Hold 'space and 'ENTER' to barf \n and slow opponent. \n Eat as many shrimp as you can";
			break;
		case State.Play:
			player1Shrimp.text = "Player1 Shrimp: " + uniEat.shrimpCount;
			player2Shrimp.text = "Player2 Shrimp: " + whaleEat.shrimpCount;
			counterText.text = ""+counter;
			titleText.text = "";
			winnerText.text = "";
			controlsText.text = "";
			break;
		case State.Win:
			if (uniEat.shrimpCount > whaleEat.shrimpCount) {
				winner = "Player 1";
			} else {
				winner = "Player 2";
			}

			player1Shrimp.text = "";
			player2Shrimp.text = "";
			counterText.text = "";
			titleText.text = "";
			winnerText.text = "Winner is: " + winner + "!";
			controlsText.text = "";
			break;
		case State.Exit:
			Application.Quit();
			break;
		}

		if (Time.time > timeCounter && (gameState == State.Play || gameState == State.Win)) {
			timeCounter = Time.time + 1;
			counter--;
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			gameState = State.Play;
		}
		if (counter == 0) {
			gameState = State.Win;
		}
		if (counter == -5) {
			gameState = State.Exit;
		}
	}
}
