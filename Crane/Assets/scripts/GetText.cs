using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GetText : MonoBehaviour {
	InputField wejscie;
	void Start(){
		wejscie = GetComponent<InputField> ();
	}

	public void text(){
		string text = wejscie.text; //zmienna text przechowuje polecenie wpisane w Unity
		string[] podzielText = text.Split (' '); //dzieli tekst na slowa
		for (int i = 0; i < podzielText.Length; i++) {
			Debug.Log (podzielText [i]);
		}
		//-------kilka przydatnych funkcji------//
		//text.Trim(); //usuwa spacje ze stringa
		//text.Contains("slowo_do_odnalezienia");
		//text.CompareTo("slowo_do_porownania");
	}
}
