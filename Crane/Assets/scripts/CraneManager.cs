using UnityEngine;
using System.Collections;

public class CraneManager : MonoBehaviour {

	public GameObject toLiftObject;
	public Transform prowadnica;
	public Transform hak;
	public Transform lina;
	public float speed = 5f;
	private static CraneManager instance;
	public static CraneManager Instance{ get { return instance; } }
	void Awake(){
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		DontDestroyOnLoad (gameObject);
	}

	public Transform Crane;

	void Start(){
		//StartCoroutine (Lift (toLiftObject));
	}

	public IEnumerator rotation(float rotation){
		if (rotation > 0) {
			float rotationSpeed=0.5f;
			while (rotation > 0) {
				Crane.rotation = Quaternion.Euler (Crane.rotation.eulerAngles.x, Crane.rotation.eulerAngles.y + rotationSpeed, Crane.rotation.eulerAngles.z);
				rotation -= rotationSpeed;
				yield return new WaitForSeconds (0.01f);
			}
		} else {
			float rotationSpeed=-0.5f;
			while (rotation < 0) {
				Crane.rotation = Quaternion.Euler (Crane.rotation.eulerAngles.x, Crane.rotation.eulerAngles.y + rotationSpeed, Crane.rotation.eulerAngles.z);
				rotation -= rotationSpeed;
				yield return new WaitForSeconds (0.01f);
			}
		}
	}

	//IEnumerator Move(){

	//}

	public IEnumerator Lift(GameObject obiekt){
		yield return StartCoroutine (rotation(Angle(obiekt)));
		yield return StartCoroutine (Move (Distance (obiekt)));
		yield return StartCoroutine (putDown(obiekt));
		StartCoroutine (putUp (obiekt));
	}

	IEnumerator putDown(GameObject obiekt){
		float yObiekt = obiekt.transform.GetChild(0).transform.position.y;
		float yHak = hak.position.y;
		float przesun = yObiekt - yHak+1.4f;
		float time = 0.01f/speed;


		if (przesun < 0) {
			float moveDistance = 0.1f;
			while (przesun < 0f) {
				lina.localScale = new Vector3 (lina.localScale.x, lina.localScale.y, lina.localScale.z + moveDistance);
				przesun += moveDistance;
				yield return new WaitForSeconds (time);
			}
			hak.GetChild (0).GetComponent<FixedJoint> ().connectedBody = obiekt.GetComponent<Rigidbody> ();
		}else {
			float moveDistance = -0.1f;
			while (przesun > 0f) {
				lina.localScale = new Vector3 (lina.localScale.x, lina.localScale.y, lina.localScale.z+moveDistance);
				przesun -= moveDistance;
				yield return new WaitForSeconds (time);
			}
			hak.GetChild (0).GetComponent<FixedJoint> ().connectedBody = obiekt.GetComponent<Rigidbody> ();
		}
	}

	IEnumerator putUp(GameObject obiekt){
		float przesun = 20f;
		float time = 0.01f/speed;
		float moveDistance = 0.1f;
		while (przesun > 0f) {
			lina.localScale = new Vector3 (lina.localScale.x, lina.localScale.y, lina.localScale.z - moveDistance);
			przesun -= moveDistance;
			yield return new WaitForSeconds (time);
		}
	}
		
	float Angle(GameObject obiekt){
		Vector2 vec1 = new Vector2 (transform.position.x, transform.position.z);
		Vector2 vec2 = new Vector2 (obiekt.transform.position.x, obiekt.transform.position.z);
		float x = vec2.x - vec1.x;
		float y = vec2.y - vec1.y;
		Debug.Log (x + " " + y);
		float tan = Mathf.Atan(y/x);
		float angle = tan * (180 / Mathf.PI);
		Debug.Log ("Kąt: " + angle);
		return angle*(-1);
	}

	IEnumerator Move(float distance){
		float time = 0.01f;
		if (distance > 0) {
			float moveDistance = 0.1f;
			while (distance > 0f) {
				prowadnica.transform.localPosition = new Vector3 (prowadnica.transform.localPosition.x, prowadnica.transform.localPosition.y, prowadnica.transform.localPosition.z + moveDistance);
				distance -= moveDistance;
				yield return new WaitForSeconds (time);
			}
		} else {
			float moveDistance = -0.1f;
			while (distance < 0f) {
				prowadnica.transform.localPosition = new Vector3 (prowadnica.transform.localPosition.x, prowadnica.transform.localPosition.y, prowadnica.transform.localPosition.z + moveDistance);
				distance -= moveDistance;
				yield return new WaitForSeconds (time);
			}
		}
	}

	float Distance(GameObject obiekt){
		Vector2 vec1 = new Vector2 (obiekt.transform.position.x, obiekt.transform.position.z);
		Vector2 vec2 = new Vector2 (prowadnica.position.x, prowadnica.position.z);
		float distance = Vector2.Distance (vec1, vec2);
		return distance;
	}
}
