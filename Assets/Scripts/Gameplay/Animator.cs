using UnityEngine;
using System.Collections;

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

	//Input
	public float rightTrigger;
	public float leftTrigger;
	private float triggerDebounce = 0.02f;
	public float maxRotateAngle = 25;


	// Use this for initialization
	void Start () {
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
		if(Input.GetAxisRaw("RightTrigger") > triggerDebounce)
			rightTrigger = Input.GetAxisRaw("RightTrigger");
		else
			rightTrigger = 0;

		if(Input.GetAxisRaw("LeftTrigger") > triggerDebounce)
			leftTrigger = Input.GetAxisRaw("LeftTrigger");
		else
			leftTrigger = 0;
		
//		Debug.Log(rightTrigger);

		rightMouthTopHinge.transform.rotation = Quaternion.AngleAxis(-maxRotateAngle * Mathf.Abs(rightTrigger) * 0.5f , transform.forward);
		rightMouthBottomHinge.transform.rotation = Quaternion.AngleAxis(maxRotateAngle * Mathf.Abs(rightTrigger) , transform.forward);
		rightHeadHinge.transform.rotation = Quaternion.AngleAxis(-maxRotateAngle * Mathf.Abs(rightTrigger) * 0.2f , transform.forward);
		rightNeckHinge.transform.rotation = Quaternion.AngleAxis(-maxRotateAngle * Mathf.Abs(rightTrigger) * 0.1f, transform.forward);

		leftMouthTopHinge.transform.rotation = Quaternion.AngleAxis(maxRotateAngle * Mathf.Abs(leftTrigger) * 0.5f , transform.forward);
		leftMouthBottomHinge.transform.rotation = Quaternion.AngleAxis(-maxRotateAngle * Mathf.Abs(leftTrigger) , transform.forward);
		leftHeadHinge.transform.rotation = Quaternion.AngleAxis(maxRotateAngle * Mathf.Abs(leftTrigger) * 0.2f , transform.forward);
		leftNeckHinge.transform.rotation = Quaternion.AngleAxis(maxRotateAngle * Mathf.Abs(leftTrigger) * 0.1f, transform.forward);
		//Bob
		unicorn.transform.position = new Vector3(unicorn.transform.position.x, unicorn.transform.position.y + Mathf.Sin(Time.time)*0.0015f, unicorn.transform.position.z);
		whale.transform.position = new Vector3(whale.transform.position.x, whale.transform.position.y + Mathf.Sin(Time.time)*0.0015f, whale.transform.position.z);

	}
}
