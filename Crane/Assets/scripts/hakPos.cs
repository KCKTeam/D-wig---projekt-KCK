using UnityEngine;
using System.Collections;

public class hakPos : MonoBehaviour {
	public Transform lina;
	float pozycja;
	float startPos;
	void Awake(){
		startPos = transform.position.y;
		pozycja=startPos-lina.localScale.z+1;
	}
	void Update(){
		pozycja = startPos-lina.localScale.z+1;;
		transform.position = new Vector3(transform.position.x, pozycja, transform.position.z );
	}

}
