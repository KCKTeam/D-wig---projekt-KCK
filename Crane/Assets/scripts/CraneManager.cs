using UnityEngine;
using System.Collections;

public class CraneManager : MonoBehaviour {

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
	}
	/*
	public IEnumerator rotation(float rotation){
		float rotationTime = rotation / 20f; //How much time will it take to rotate the crane
		float currentTime = 0.0f;
		bool done = false;
		float angle = Crane.rotation.eulerAngles.y + rotation;
		while () {
			float percent = currentTime / rotationTime;
			if (percent >= 1.0f)
			{
				percent = 1;
				done = true;
			}
			Crane.rotation = Quaternion.Euler (Crane.rotation.eulerAngles.x, angle, Crane.rotation.eulerAngles.z),percent );
			currentTime += 0.01f;
			yield return new WaitForSeconds(0.01f);
		}
	}*/

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

	public void Lift(GameObject obiekt){
		Transform podnoszony = obiekt.GetComponent<Transform> ();

	}
}
