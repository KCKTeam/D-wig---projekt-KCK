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
		StartCoroutine (putUpDistance (-10f));
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

	public IEnumerator Lift(GameObject obiekt){
		yield return StartCoroutine (rotation(Angle(obiekt)));
		yield return StartCoroutine (Move (Distance (obiekt)));
		yield return StartCoroutine (lineDown(obiekt));
		StartCoroutine (lineUp ());
	}

	public IEnumerator putDown(){
		yield return StartCoroutine (putObjectDown());
		StartCoroutine (lineUp ());

	}
	public IEnumerator putObjectDown(){
		GameObject obiekt = hak.GetChild (0).GetComponent<FixedJoint> ().connectedBody.gameObject;
		Debug.Log (obiekt.GetComponent<objectProperties> ().rodzaj + " " + obiekt.GetComponent<objectProperties> ().kolor);
		if (obiekt != null) {
			float przesun = obiekt.transform.GetChild (1).transform.position.y;
			float time = 0.01f;
			if (przesun < 0) {
				float moveDistance = -0.1f;
				while (przesun < 0f) {
					lina.localScale = new Vector3 (lina.localScale.x, lina.localScale.y, lina.localScale.z + moveDistance);
					przesun += moveDistance;
					yield return new WaitForSeconds (time);
				}
			} else {
				float moveDistance = 0.1f;
				while (przesun > 0f) {
					lina.localScale = new Vector3 (lina.localScale.x, lina.localScale.y, lina.localScale.z + moveDistance);
					przesun -= moveDistance;
					yield return new WaitForSeconds (time);
				}
			}
			hak.GetChild (0).GetComponent<FixedJoint> ().connectedBody = null;
		}
	}

	public IEnumerator putUpDistance(float distance){
		float przesun = distance;
		float time = 0.01f;
		if (przesun < 0) {
			float moveDistance = -0.1f;
			while (przesun < 0f && lina.localScale.z>0) {
				lina.localScale = new Vector3 (lina.localScale.x, lina.localScale.y, lina.localScale.z + moveDistance);
				przesun += moveDistance;
				yield return new WaitForSeconds (time);
			}
		} else {
			float moveDistance = 0.1f;
			while (przesun > 0f && lina.localScale.z>0) {
				lina.localScale = new Vector3 (lina.localScale.x, lina.localScale.y, lina.localScale.z + moveDistance);
				przesun -= moveDistance;
				yield return new WaitForSeconds (time);
			}
		}
	}

	IEnumerator lineDown(GameObject obiekt){
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

	IEnumerator lineUp(){
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
		Vector2 vec3 = new Vector2 (hak.position.x, hak.position.z);
		float x = vec2.x - vec1.x;
		float y = vec2.y - vec1.y;
		float tan = Mathf.Atan(y/x);
		float angle = tan * (180 / Mathf.PI);

		float x1 = vec3.x - vec1.x;
		float y1 = vec3.y - vec1.y;
		float tan1 = Mathf.Atan(y1/x1);
		float angle1 = tan1 * (180 / Mathf.PI);

		if (vec2.x < 0 && vec2.y > 0) {
			angle = 180 + angle;
		}
		if (vec2.x < 0 && vec2.y < 0) {
			angle = -180 + angle;
		}
		if (vec3.x < 0 && vec3.y > 0) {
			angle1 = 180 + angle1;
		}
		if (vec3.x < 0 && vec3.y < 0) {
			angle1 = -180 + angle1;
		}

		float angle2=angle - angle1;
		return angle2*(-1);
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

		if (vec2.y >= 0) {
			if(vec1.y<=vec2.y)
				distance=distance*(-1);
		}
		if (vec2.y < 0) {
			if(vec1.y>=vec2.y)
				distance=distance*(-1);
		}

		return distance;
	}

	public bool checkJoint(){
		bool joint = !(hak.GetChild (0).GetComponent<FixedJoint> ().connectedBody == null);
		return joint;
	}
}