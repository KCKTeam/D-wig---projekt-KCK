using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;


public class GetText : MonoBehaviour {
	public Text craneResponse;
	public Text playerText;
	public Transform TextContainer;
	//tablica przechowująca wszystkie obiekty ze sceny
	public GameObject [] obiekty;

	Slownik slownik=new Slownik();
	AnalizaZapytania analiza;
	Zapytanie nowe;
//	string [] zaimki = {"obok", "lew","praw", "na", "przed", "za"};

	InputField wejscie;
	List <string> dopytania=new List<string>();

	void Start(){
		//wczytanie wszystkich obiektów do tablicy
		obiekty = GameObject.FindGameObjectsWithTag ("obiekt");
		wejscie = GetComponent<InputField> ();
		analiza = gameObject.AddComponent (typeof(AnalizaZapytania)) as AnalizaZapytania;
		dopytania.Clear ();
	}

	public void text(){
		string text = wejscie.text;//zmienna text przechowuje polecenie wpisane w Unity
		myText(text); //wyswietla w oknie gry tekst wpisany przez uzytkownika
		wejscie.text="";

		dopytania = analiza.listaDopytan ();

		if (dopytania.Count > 0) {
			analiza.dopytaj (dopytania, text);
		} else {
			nowe = new Zapytanie (text);
			analiza.dodajObiekty (obiekty);
			analiza.dodajZapytanie (nowe);
			analiza.dodajSlownik (slownik);

			analiza.znajdzTokeny ();
			analiza.znajdzObiekty ();

			if (dopytania.Count > 0) {
				analiza.dopytaj (dopytania, text);
			} else {
				//analiza.uzupelnijKoloryRodzaje ();
				analiza.znajdzPolecenie ();
			}
		}
	}
		
	public void craneText(string text){
		Text newText=Instantiate (craneResponse, TextContainer, worldPositionStays:false) as Text;
		newText.text = text;
	}

	public void myText(string text){
		Text newText=Instantiate (playerText, TextContainer, worldPositionStays:false) as Text;
		newText.text = text;
	}

}