using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	Vector3 rotationPoint;
	Vector3 lookAtPoint;
	Vector3 direction;
	float delta=80f;
	float rotationSpeed=80f;
	// Use this for initialization
	void Start () {
		rotationPoint =  new Vector3 (0f, 0, 0f);
		lookAtPoint = new Vector3 (0f, 10f, 0f);
		direction = Vector3.zero;
	}
	
	// Update is called once per frame
	float newYposition;
	void Update () {
		userInput ();
		transform.RotateAround (rotationPoint, direction, rotationSpeed * Time.deltaTime);
		newYposition = transform.position.y + delta * Time.deltaTime;
		transform.position = new Vector3 (transform.position.x, newYposition, transform.position.z);
		transform.LookAt (lookAtPoint);
	
	}


	void userInput()
	{
		if (Input.GetKey (KeyCode.LeftArrow)) {
			direction = Vector3.up;
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			direction = Vector3.down;
		}else
			direction = Vector3.zero;

		if (Input.GetKey (KeyCode.UpArrow)) {
			if (transform.position.y > 2f) {
				delta = -80f;
			}else
				delta = 0f;
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			delta = 80f;
		} else
			delta = 0f;

	}
}
