using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;


public class GetText : MonoBehaviour {
	InputField wejscie;
	void Start(){
		wejscie = GetComponent<InputField> ();
	}

	public void text(){
		string text = wejscie.text; //zmienna text przechowuje polecenie wpisane w Unity
		searchOrder(text);
		/*
		string[] podzielText = text.Split (' '); //dzieli tekst na slowa
		for (int i = 0; i < podzielText.Length; i++) {
			Debug.Log (podzielText [i]);
		}

		//-------kilka przydatnych funkcji------//
		//text.Trim(); //usuwa spacje ze stringa
		//text.Contains("slowo_do_odnalezienia");
		//text.CompareTo("slowo_do_porownania");*/
	}

	void searchOrder(string pobrany_tekst){
		Debug.Log (pobrany_tekst);
		bool obrot = false, czy_liczba=false;
		float kat=0;

		char delimiter = ' ';
		string[] substrings = pobrany_tekst.Split(delimiter);
		foreach (string substring in substrings)
		{
			if (substring == "Obróć")
			{
				obrot = true;
			}
			if (Regex.IsMatch(substring, @"^\d+$"))
			{
				czy_liczba = true;
				float.TryParse(substring, out kat);
			}
		}

		if (obrot && czy_liczba) {
			Debug.Log ("Obracam dźwig o "+ kat.ToString() +" stopni");
			StartCoroutine(CraneManager.Instance.rotation(kat));
		}
	}
}
	