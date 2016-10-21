using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GetText : MonoBehaviour {
	InputField wejscie;
	void Start(){
		wejscie = GetComponent<InputField> ();
	}

	public void text(){
		Debug.Log (wejscie.text);
		string ab = wejscie.text;
		Debug.Log (ab + "123");
		//int stopnie = (int)wejscie.text;
	}
}
