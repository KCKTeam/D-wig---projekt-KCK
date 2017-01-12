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
//	string [] zaimki = {"obok", "lew","praw", "na", "przed", "za"};

	InputField wejscie;

	void Start(){
		//wczytanie wszystkich obiektów do tablicy
		obiekty = GameObject.FindGameObjectsWithTag ("obiekt");
		wejscie = GetComponent<InputField> ();
		analiza = gameObject.AddComponent (typeof(AnalizaZapytania)) as AnalizaZapytania;
	}

	public void text(){
		string text = wejscie.text;//zmienna text przechowuje polecenie wpisane w Unity
		myText(text); //wyswietla w oknie gry tekst wpisany przez uzytkownika
		wejscie.text="";

		Zapytanie nowe=new Zapytanie(text);
		analiza.dodajZapytanie (nowe);
		analiza.dodajSlownik (slownik);
		analiza.znajdzTokeny ();
		analiza.znajdzPolecenie ();

	}
		
	void craneText(string text){
		Text newText=Instantiate (craneResponse, TextContainer, worldPositionStays:false) as Text;
		newText.text = text;
	}

	void myText(string text){
		Text newText=Instantiate (playerText, TextContainer, worldPositionStays:false) as Text;
		newText.text = text;
	}

}