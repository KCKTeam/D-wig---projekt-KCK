using UnityEngine;
using System.Collections;

public class detekcjaObiektu : MonoBehaviour {
	GameObject obiekt;
	void OnTriggerEnter(Collider other){
		obiekt = other.gameObject;
	}

	void OnTriggerExit(Collider other){
		obiekt = null;
	}

	public GameObject pobierzObiekt(){
		if (obiekt != null) {
			return obiekt;
		} else
			return null;
	}
}
