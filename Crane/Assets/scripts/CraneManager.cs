using UnityEngine;
using System.Collections;

public class CraneManager : MonoBehaviour {
	public Transform Crane;
	float rotationSpeed=100f;
	void Start(){
		StartCoroutine(rotation (45f));
	}

	IEnumerator rotation(float rotation){
		float rotationTime = rotation / 20f;//How much time will it take to dim the light
		float currentTime = 0.0f;
		bool done = false;
		float angle = Crane.rotation.eulerAngles.y + rotation;
		while (!done) {
			float percent = currentTime / rotationTime;
			if (percent >= 1.0f)
			{
				percent = 1;
				done = true;
			}
			Crane.rotation = Quaternion.Lerp(Crane.rotation,Quaternion.Euler (Crane.rotation.eulerAngles.x, angle, Crane.rotation.eulerAngles.z),percent );
			currentTime += 0.01f;
			yield return new WaitForSeconds(0.01f);
		}
	}
}
