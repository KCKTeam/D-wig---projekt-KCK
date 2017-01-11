using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;


public class GetText : MonoBehaviour {
	public Text craneResponse;
	public Text playerText;
	public Transform TextContainer;
	//tablica przechowująca wszystkie obiekty ze sceny
	public GameObject [] obiekty;
	string[] substrings;
	//słowa kluczowe
	string [] kolory = {"ziel","niebi","czerw", "fiolet", "żółt", "biał", "czarn"};
	string [] rodzaje = {"becz", "kontene", "skrzyn","samoch", "karto", "drzwi", "okn", "pił" };
	string [] czas_pod = {"podnieś", "unieś"};
	string [] czas_kladz = {"opuść", "postaw","odłóż", "połóż"};
	bool znajdzKolor=false;
//	string [] zaimki = {"obok", "lew","praw", "na", "przed", "za"};

	InputField wejscie;

	void Start(){
		//wczytanie wszystkich obiektów do tablicy
		obiekty = GameObject.FindGameObjectsWithTag ("obiekt");
		wejscie = GetComponent<InputField> ();
	}

	public void text(){
		string text = wejscie.text;//zmienna text przechowuje polecenie wpisane w Unity
		myText(text); //wyswietla w oknie gry tekst wpisany przez uzytkownika
		wejscie.text="";

		//sprawdzenie czy nalezy wyszukac tylko kolor czy przeszukac cale wyrazenie
		if (znajdzKolor) {
			wczytajKolor (text);
		} else {
			searchOrder (text);
		}
	}

	string kolorG = "";
	string rodzajG = "";
	void wczytajKolor(string text){
		char delimiter = ' ';
		substrings = text.ToLower ().Split (delimiter);
		for (int i = 0; i < kolory.Length; i++) {
			for (int j = 0; j < substrings.Length; j++) {
				if (substrings [j].Contains (kolory [i])) {
					kolorG = kolory [i];
				}
			}
		}
			findGameObject (rodzajG, kolorG);
			znajdzKolor = false;
	}

	bool czy_trzymam=false;

	void searchOrder(string pobrany_tekst){
		bool obrot = false, czy_liczba = false;
		bool czy_podnoszenie = false, czy_kladzenie = false;
		int index_podnoszenia = -1, index_kladzenia = -1;
		string kierunek = "";
		float kat = 0;
		//zmienne do przetworzenia tekstu
		char delimiter = ' ';
		substrings = pobrany_tekst.ToLower ().Split (delimiter);

		//----------------------obracanie dźwigu---------------------//
		foreach (string substring in substrings) {
			if (substring == "obróć") {
				obrot = true;
			}
			if (Regex.IsMatch (substring, @"^\d+$")) {
				czy_liczba = true;
				float.TryParse (substring, out kat);
			}
			if (substring == "lewo" || substring == "prawo") {
				kierunek = substring;
			}
		}

		if (obrot && czy_liczba) {
			if (kierunek == "lewo") {
				kat *= -1;
			}
			craneText ("Obracam dźwig o " + kat.ToString () + " stopni");
			StartCoroutine(CraneManager.Instance.rotation(kat));
		}
		//--------------------------------------------------------------//

		//----wyszukiwanie czasowników podnoszenia i opuszczania----//
		//foreach (string substring in substrings)
		for(int j=0;j<substrings.Length;j++){
			for (int i = 0; i < czas_pod.Length; i++) {
				if (substrings[j].Contains (czas_pod [i])) {
					czy_podnoszenie = true;
					index_podnoszenia = j;
				}
			}

			for (int i = 0; i < czas_kladz.Length; i++) {
				if (substrings[j].Contains (czas_kladz [i])) {
					czy_kladzenie = true;
					index_kladzenia = j;
				}
			}
		}
		//---------------------------------------------------------//

		//----sprawdzenie, którą funkcję należy wywołać----//
		if (czy_podnoszenie && czy_kladzenie && (!czy_trzymam)) {
			Debug.Log ("Podnosze i klade");
			if (index_kladzenia > 1) { //jeśli znaleziono czasownik kładzenia
				znajdzObiekt (index_podnoszenia, index_kladzenia); //znajdź obiekt od indeksu czasownika podnoszenia do indeksu czasownika kładzenia
				znajdzObiekt (index_kladzenia, substrings.Length); //znajdź obiekt od indeksu czasownika kładzenia do końca wyrażenia
			}
		}else if (czy_podnoszenie) {
			GameObject obiekt=znajdzObiekt ();
			if (obiekt != null) {
				string text = "Podnoszę "+obiekt.GetComponent<objectProperties>().kolor+" "+obiekt.GetComponent<objectProperties>().rodzaj;
				craneText (text);
				StartCoroutine (CraneManager.Instance.Lift (obiekt));
			}
		}else if (czy_kladzenie) {
			czy_trzymam=CraneManager.Instance.checkJoint ();
			if (!czy_trzymam) {
				string text = "Nie trzymam zadnego obiektu";
				craneText (text);
			} else {
				string text = "Opuszczam...";
				craneText (text);
				StartCoroutine (CraneManager.Instance.putDown ());
			}
			//GameObject obiekt=znajdzObiekt ();
			//Debug.Log ("Znalazłem " + obiekt.GetComponent<objectProperties> ().rodzaj + " " + obiekt.GetComponent<objectProperties> ().kolor);
		}
	}

	//wyszukiwanie jednego obiektu
	GameObject znajdzObiekt(){
		GameObject findedObject;
		string kolor="brak";
		string rodzaj="brak";

		//----sprawdzenie koloru, rodzaju i zaimków---------//
		foreach (string substring in substrings) {
			for (int i = 0; i < kolory.Length; i++) {
				if (substring.Contains (kolory [i])) {
					kolor = kolory [i];
					//Debug.Log ("Twój kolor: " + substring + ", " + kolor);
				}
			}

			for (int i = 0; i < rodzaje.Length; i++) {
				if (substring.Contains (rodzaje [i])) {
					rodzaj = rodzaje [i];
					//Debug.Log ("Twój rodzaj: " + substring);
				}
			}
		}

		//weryfikacja danych dotyczących obiektu
		findedObject=checkData(rodzaj, kolor);
		return findedObject;

	}

	GameObject checkData(string rodzaj, string kolor){
		GameObject obiekt;
		if (rodzaj == "brak") {
			string text = "Nie znaleziono obiektu.";
			craneText (text);
		}else if (kolor == "brak") {
			int ile = 0;
			for (int i = 0; i < obiekty.Length; i++) {
				if (obiekty [i].GetComponent<objectProperties> ().rodzaj.Contains (rodzaj)) {
					kolor = obiekty [i].GetComponent<objectProperties> ().kolor;
					ile++;
				}
			}
			for (int i = 0; i < kolory.Length; i++) {
				if (kolor.Contains (kolory [i])) {
					kolor = kolory [i];
				}
			}
			if (ile > 1) {
				string text = "Znalazłem więcej niż jeden obiekt danego typu. Wprowadź kolor.";
				craneText (text);
				znajdzKolor=true;
				kolorG = "brak";
				rodzajG = rodzaj;
			}
		}
		obiekt=findGameObject (rodzaj, kolor);
		return obiekt;
	}

	GameObject findGameObject(string rodzaj, string kolor){
		GameObject obiekt=null;
		Debug.Log (rodzaj + kolor);
		for (int i = 0; i < obiekty.Length; i++) {
			if (obiekty [i].GetComponent<objectProperties> ().rodzaj.Contains (rodzaj) && obiekty [i].GetComponent<objectProperties> ().kolor.Contains (kolor)) {
				Debug.Log ("Znalazłem " + obiekty [i].GetComponent<objectProperties> ().rodzaj + " " + obiekty [i].GetComponent<objectProperties> ().kolor);
				obiekt=obiekty [i];
				//StartCoroutine (CraneManager.Instance.Lift (obiekty [i]));
			}
		}
		return obiekt;

	}
		//------------------------------------------------
	

	//wyszukiwanie dwóch obiektów
	void znajdzObiekt(int start_index, int end_index){
		string kolor="brak";
		string rodzaj="brak";
		//----sprawdzenie koloru i rodzaju ---------//
		for (int j = start_index; j < end_index; j++) {
			for (int i = 0; i < kolory.Length; i++) {
				if (substrings [j].Contains (kolory [i])) {
					kolor = kolory [i];
				}

				if (substrings [j].Contains (rodzaje [i])) {
					rodzaj = rodzaje [i];
				}
			}
		}
		checkData(rodzaj, kolor);
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