using System;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class AnalizaZapytania : MonoBehaviour{
	Zapytanie zapytanie;
	Slownik slownik;
	GameObject [] obiekty;
	List <GameObject> findedObjects=new List<GameObject>();
	List <string> dopytania = new List<string>();

	string [] substrings;
	char delimiter = ' ';

	string [] koloryNaScenie;
	string [] rodzajeNaScenie;

	public AnalizaZapytania (){
		dopytania.Clear();
	}

	public void dodajZapytanie(Zapytanie doDodania){
		zapytanie = doDodania;
		substrings = zapytanie.getTekstZapytania ().ToLower().Split (delimiter);
	}

	public void dodajSlownik(Slownik doDodania){
		slownik = doDodania;
	}

	public void dodajObiekty(GameObject [] doDodania){
		obiekty = doDodania;
	}
	/*
	public void uzupelnijKoloryRodzaje(){
		koloryNaScenie=new string[obiekty.Length];
		rodzajeNaScenie=new string[obiekty.Length];
		for (int i = 0; i < obiekty.Length; i++) {
			koloryNaScenie [i] = obiekty [i].GetComponent<objectProperties> ().kolor;
			rodzajeNaScenie [i] = obiekty [i].GetComponent<objectProperties> ().rodzaj;
		}

		for (int i = 0; i < koloryNaScenie.Length; i++) {
			for (int j = 0; j < slownik.kolory.Length; j++) {
				if (koloryNaScenie [i].Contains (slownik.kolory[j])) {
					koloryNaScenie[i]=slownik.kolory[j];
				}
			}
			//Debug.Log (koloryNaScenie [i]);
		}

		for (int i = 0; i < rodzajeNaScenie.Length; i++) {
			for (int j = 0; j < slownik.kolory.Length; j++) {
				if (rodzajeNaScenie [i].Contains (slownik.rodzaje[j])) {
					rodzajeNaScenie[i]=slownik.rodzaje[j];
				}
			}
			//Debug.Log (rodzajeNaScenie [i]);
		}
	}*/
	public void podmienKoloryRodzaje(){
		for(int i=0;i< zapytanie.obiekty_rodzaj.Count;i++){
			for (int j = 0; j < slownik.kolory.Length; j++) {
					if (substrings[zapytanie.obiekty_rodzaj [i]].Contains(slownik.rodzaje[j])) {
					substrings[zapytanie.obiekty_rodzaj [i]] = slownik.rodzaje [j];
				}
			}
		}

		for(int i=0;i< zapytanie.obiekty_kolor.Count;i++){
			for (int j = 0; j < slownik.kolory.Length; j++) {
				if (substrings[zapytanie.obiekty_kolor [i]].Contains(slownik.kolory[j])) {
					substrings[zapytanie.obiekty_kolor [i]] = slownik.kolory [j];
				}
			}
		}
	}
		

	public void znajdzObiekty(){
		podmienKoloryRodzaje ();

		//zapytanie.trzymaB=CraneManager.Instance.checkJoint ();
		zapytanie.trzymaB=false;
	
		int iloscRodzajow=zapytanie.obiekty_rodzaj.Count;
		int iloscKolorow=zapytanie.obiekty_kolor.Count;
		List <int> pozostaleIndexy=new List<int>(zapytanie.obiekty_rodzaj);

		/*Dla każdego rodzaju z listy szuka dopasowania koloru. 
		Jeśli takowe znajduje, to do listy findedObjects dodaje
		obiekt znaleziony po rodzaju i kolorze. Usuwa również 
		z listy pozostałych elementów index rodzaju, 
		który został wykorzystany do znalezienia obiektu.
		*/

			for (int i = 0; i < zapytanie.obiekty_rodzaj.Count; i++) {
				for (int j = 0; j < zapytanie.obiekty_kolor.Count; j++) {
					if ((zapytanie.obiekty_rodzaj [i] - zapytanie.obiekty_kolor [j]) == 1 || (zapytanie.obiekty_rodzaj [i] - zapytanie.obiekty_kolor [j]) == -1) {
						string rodzaj = substrings [zapytanie.obiekty_rodzaj [i]];
						string kolor = substrings [zapytanie.obiekty_kolor [j]];
						findedObjects.Add (znajdzNaScenie (rodzaj, kolor));
						pozostaleIndexy.Remove (zapytanie.obiekty_rodzaj [i]);
					}
				}
			}

			/*
		 Wywołanie funkcji znajdzNaScenie dla każdego indeksu, który pozostał.
		 */
			for (int i = 0; i < pozostaleIndexy.Count; i++) {
				znajdzNaScenie (substrings [pozostaleIndexy [i]]);
			}
		
	}

	GameObject znajdzNaScenie(string r, string k){
		GameObject znaleziony = null;
		for (int i = 0; i < obiekty.Length; i++) {
			if(obiekty[i].GetComponent<objectProperties>().kolor.Contains(k)&&obiekty[i].GetComponent<objectProperties>().rodzaj.Contains(r)){
				znaleziony = obiekty [i];
				Debug.Log ("Znalazłem " + obiekty[i].GetComponent<objectProperties>().kolor + obiekty[i].GetComponent<objectProperties>().rodzaj);
			}
		}
		return znaleziony;
	}

	GameObject znajdzNaScenie (string r){
		GameObject znaleziony = null;
		int takieSame = 0;
		for (int i = 0; i < obiekty.Length; i++) {
			if (obiekty [i].GetComponent<objectProperties> ().rodzaj.Contains (r)) {
				takieSame++;
				znaleziony = obiekty [i];
			}
		}
		if (takieSame > 1) {
			Debug.Log ("Więcej niż jeden tego rodzaju");
			dopytania.Add (r);
			return null;
		} else {
			Debug.Log ("Znalazłem " + znaleziony.GetComponent<objectProperties> ().kolor + znaleziony.GetComponent<objectProperties> ().rodzaj);
			return znaleziony;
		}

	}

	public void dopytaj(List <string> doSprawdzenia, string d){
		string [] dopytanie = d.ToLower ().Split (' ');
		dopytania = doSprawdzenia;
		tokenyDopytania (dopytanie);
		if (zapytanie.twierdzenieB) {
			Debug.Log ("Wybieram obiekt losowo");
			dopytania.RemoveAt (0);
			zapytanie.twierdzenieB = false;
			if (dopytania.Count > 0)
				dopytaj(dopytania, "");
		} else if (zapytanie.przeczenieB) {
			Debug.Log ("Podaj kolor.");
			zapytanie.przeczenieB = false;
		} else if (zapytanie.kolorB) {
			Debug.Log ("Wybrany kolor: " + dopytanie[zapytanie.kolor]);
			int index = dopytania.Count - 1;
			dopytania.RemoveAt (index);
			zapytanie.kolorB = false;
			if (dopytania.Count > 0)
				dopytaj(dopytania, "");
		}
		else {
			Debug.Log ("Znalazłem więcej niż jeden obiekt rodzaju: " + doSprawdzenia [0]);
			Debug.Log ("Wybrać obiekt losowo?");
		}

		//jeśli dopytał o wszystko to musi uruchomić znajdowanie polecenia z tego miejsca
		if (dopytania.Count == 0)
			znajdzPolecenie ();
		
	}

	public void tokenyDopytania(string [] dopytanie){
		foreach (string substring in dopytanie) {
			int index = 0;
			for (int i = 0; i < slownik.twierdzenia.Length; i++) {
				if (substring==slownik.twierdzenia [i]) {
					zapytanie.twierdzenieB = true;
				}
			}

			for (int i = 0; i < slownik.przeczenia.Length; i++) {
				if (substring == slownik.przeczenia [i]) {
					zapytanie.przeczenieB = true;
				}
			}

			for (int i = 0; i < slownik.kolory.Length; i++) {
				if (substring.Contains (slownik.kolory [i])) {
					zapytanie.kolorB = true;
					zapytanie.kolor = index;
				}
			}
			index++;
		}
	}

	public List <string> listaDopytan(){
		return dopytania;
	}

/*	GameObject checkData(string rodzaj, string kolor){
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
	}*/

	public void znajdzTokeny(){
		int index = 0;
		foreach (string substring in substrings) {

			//znajdowanie czasownika podnoszenia
			for (int i = 0; i < slownik.czas_pod.Length; i++) {
				if (substring.Contains (slownik.czas_pod [i])) {
					zapytanie.czasownik_podB = true;
					zapytanie.czasownik_pod = index;
					zapytanie.liczba_czasownikow++;
				}
			}

			//znajdowanie czasownika opuszczania
			for (int i = 0; i < slownik.czas_opu.Length; i++) {
				if (substring.Contains (slownik.czas_opu[i])) {
					zapytanie.czasownik_opuB = true;
					zapytanie.czasownik_opu = index;
					zapytanie.liczba_czasownikow++;
				}
			}

			//znajdowanie czasownika przesunięcia
			for (int i = 0; i < slownik.czas_przesun.Length; i++) {
				if (substring.Contains (slownik.czas_przesun[i])) {
					zapytanie.czas_przesunB = true;
					zapytanie.czas_przesun = index;
					zapytanie.liczba_czasownikow++;
				}
			}


			//znajdowanie rodzajów
			for (int i = 0; i < slownik.rodzaje.Length; i++) {
				if (substring.Contains (slownik.rodzaje [i])) {
					zapytanie.obiekty_rodzaj.Add (index);
				}
			}

			//znajdowanie kolorów
			for (int i = 0; i < slownik.kolory.Length; i++) {
				if (substring.Contains (slownik.kolory [i])) {
					zapytanie.obiekty_kolor.Add (index);
				}
			}

			//znajdowanie liczby
			if (Regex.IsMatch (substring, @"^\d+$")) {
				float.TryParse (substring, out zapytanie.liczba);
			}

			//znajdowanie sektora
			if (Regex.IsMatch (substring, @"^[a-h]{1}[0-9]{1}$")) {
				zapytanie.sektory.Add (index);
				Debug.Log ("sektor " + substrings [index]);
			}

			//znajdowanie słów obrotu
			for (int i = 0; i < slownik.czas_obrotu.Length; i++) {
				if (substring.Contains (slownik.czas_obrotu[i])) {
					zapytanie.obrocB = true;
					zapytanie.obroc = index;
					zapytanie.liczba_czasownikow++;
				}
			}

			//znajdowanie słów kierunku
			for (int i = 0; i < slownik.kierunki.Length; i++) {
				if (substring.Contains (slownik.kierunki[i])) {
					zapytanie.kierunekB = true;
					zapytanie.kierunek = index;
				}
			}

			//znajdowanie zaimków
			for (int i = 0; i < slownik.zaimki.Length; i++) {
				if (substring.Contains (slownik.zaimki[i])) {
					zapytanie.zaimki.Add (index);
				}
			}

			//znajdowanie sektorów
			for (int i = 0; i < slownik.sektor.Length; i++) {
				if (substring.Contains (slownik.sektor[i])) {
					zapytanie.sektory.Add (index);
					zapytanie.sektorB = true;
				}
			}

			//znajdowanie jednostek
			for (int i = 0; i < slownik.jednostki.Length; i++) {
				if (substring.Contains (slownik.jednostki[i])) {
					zapytanie.jednostkiB = true;
					zapytanie.jednostki = index;
				}
			}


			index++;
		}
	}

	public void znajdzPolecenie(){
		zapytanie.trzymaB = CraneManager.Instance.checkJoint ();
		zapytanie.trzymaB = false;

		Debug.Log (zapytanie.obiekty_rodzaj.Count);

		// Pozostałe opcje

		/*if (zapytanie.obrocB && zapytanie.jednostkiB && zapytanie.liczba != 0) {
			if (substrings [zapytanie.kierunek].Contains ("lew")) {
				zapytanie.liczba = zapytanie.liczba * -1f;
			}
			float kat = zapytanie.liczba;
			StartCoroutine (CraneManager.Instance.rotation (kat));
		}
		else if (zapytanie.czasownik_opuB && zapytanie.liczba != 0 && zapytanie.jednostkiB && zapytanie.obiekty_rodzaj.Count == 0) {
			Debug.Log ("Opuszczam hak na wysokość " + zapytanie.liczba + " " + substrings [zapytanie.jednostki]);
			// wysokość(liczba);
		}

		else if (zapytanie.czasownik_podB && zapytanie.liczba != 0 && zapytanie.jednostkiB && zapytanie.obiekty_rodzaj.Count == 0) {
			Debug.Log ("Podnoszę hak na wysokość " + zapytanie.liczba + " " + substrings [zapytanie.jednostki]);
			// wysokość(liczba);
		}*/

		if (zapytanie.liczba_czasownikow == 1 && zapytanie.obiekty_rodzaj.Count <= 1 && zapytanie.sektory.Count < 1) {

			if (zapytanie.obrocB && zapytanie.jednostkiB && zapytanie.liczba != 0) {
				if (substrings [zapytanie.kierunek].Contains ("lew")) {
					zapytanie.liczba = zapytanie.liczba * -1f;
				}
				float kat = zapytanie.liczba;
				StartCoroutine (CraneManager.Instance.rotation (kat));
			}
			if (zapytanie.czasownik_opuB && zapytanie.liczba != 0 && zapytanie.jednostkiB && zapytanie.obiekty_rodzaj.Count == 0) {
				Debug.Log ("Opuszczam hak o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki]);

				StartCoroutine (CraneManager.Instance.opuscHak (zapytanie.liczba)); //funkcja dźwigu
			}

			if (zapytanie.czasownik_podB && zapytanie.liczba != 0 && zapytanie.jednostkiB && zapytanie.obiekty_rodzaj.Count == 0) {
				Debug.Log ("Podnoszę hak o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki]);

				StartCoroutine (CraneManager.Instance.opuscHak (zapytanie.liczba * -1f)); //funkcja dzwigu
			}
			if (zapytanie.czasownik_podB && zapytanie.liczba != 0 && zapytanie.jednostkiB && zapytanie.obiekty_rodzaj.Count == 1) {
				Debug.Log ("Podnoszę przedmiot o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki]);
			}

			if (zapytanie.czasownik_opuB && zapytanie.liczba != 0 && zapytanie.jednostkiB && zapytanie.obiekty_rodzaj.Count == 1) {
				Debug.Log ("Opuszczam przedmiot o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki]);
			}
			if (zapytanie.czasownik_podB && zapytanie.obiekty_rodzaj.Count == 1 && !zapytanie.jednostkiB) {
				Debug.Log ("Podnoszę...");
			}
			if (zapytanie.czasownik_opuB && zapytanie.obiekty_rodzaj.Count == 1 && !zapytanie.jednostkiB) {
				Debug.Log ("Opuszczam...");
			}

			if (zapytanie.czas_przesunB && zapytanie.liczba != 0 && zapytanie.jednostkiB) {
				float distance = zapytanie.liczba;
				if (zapytanie.kierunekB) {
					if (substrings [zapytanie.kierunek].Contains ("tył")) {
						distance *= -1;
						Debug.Log ("Przesuwam hak o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki] + " w tył");

						StartCoroutine (CraneManager.Instance.przesunProwadnice (zapytanie.liczba * -1f)); //funkcja dźwigu
					} else if (substrings [zapytanie.kierunek].Contains ("przód") || substrings [zapytanie.kierunek].Contains ("przod")) {
						Debug.Log ("Przesuwam hak o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki] + " w przód");

						StartCoroutine (CraneManager.Instance.przesunProwadnice (zapytanie.liczba)); //funkcja dźwigu
					} else if (substrings [zapytanie.kierunek].Contains ("dół")) {
						distance *= -1;
						Debug.Log ("Przesuwam hak o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki] + " w dół");

						StartCoroutine (CraneManager.Instance.opuscHak (zapytanie.liczba)); //funkcja dźwigu
					} else if (substrings [zapytanie.kierunek].Contains ("gór")) {
						Debug.Log ("Przesuwam hak o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki] + " w górę");

						StartCoroutine (CraneManager.Instance.opuscHak (zapytanie.liczba * -1f)); //funkcja dźwigu
					}
				} else
					Debug.Log ("Przesuwam hak o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki] + " w przód");
			}

		}

		// dźwig nic nie trzyma
		else if (!zapytanie.trzymaB) {
			//2 czasowniki, bez sektora
			if (zapytanie.czasownik_podB && zapytanie.czasownik_opuB && !zapytanie.sektorB) {
				//0 przdmiotów np. Podnieś połóż
				if (zapytanie.obiekty_rodzaj.Count == 0) {
					Debug.Log ("Nie podałeś przedmiotu");
				}
				//1 przedmiot np. Podnieś samochód i go odłóż
				else if (zapytanie.obiekty_rodzaj.Count == 1) {
					Debug.Log ("Podnoszę i opuszczam przedmiot");
					// podnieś(obiekt); połóż(obiekt);
				}
				//2 przedmioty np. Podnieś samochód i połóż go obok skrzynki
				else if (zapytanie.obiekty_rodzaj.Count == 2) {
					Debug.Log ("Podnoszę obiekt A i kładę obok B");
					//podnieś(obiekt1); połóżObok(obiekt1,obiekt2);
				}
			}
			//2 czasowniki z sektorem docelowym
			else if (zapytanie.czasownik_podB && zapytanie.czasownik_opuB && zapytanie.sektorB) {
				//0 przdmiotów np. Podnieś połóż B5
				if (zapytanie.obiekty_rodzaj.Count == 0) {
					Debug.Log ("Nie podałeś przedmiotu");
				}
				//1 przedmiot np. Podnieś czerwony samochód i połóż go w sektorze A3
				else if (zapytanie.obiekty_rodzaj.Count == 1) {
					Debug.Log ("Podnosz i opuszczam przedmiot w sektorze");
					// podnieś(obiekt); połóżWSektorze(sektor)
				}
				//2 przedmioty np. Podnieś samochód i połóż go w sektorze a3 obok skrzynki
				else if (zapytanie.obiekty_rodzaj.Count >= 2) {
					Debug.Log ("Ta operacja jest dla mnie za trudna");
				}
			}
			//1 czasowniki podnoszenia, bez sektora
			else if (zapytanie.czasownik_podB && !zapytanie.sektorB) {
				//0 przdmiotów np. Podnieś
				if (zapytanie.obiekty_rodzaj.Count == 0) {
					Debug.Log ("Nie podałeś przedmiotu");
				}
				//1 przedmiot np. Podnieś zielony samochód
				else if (zapytanie.obiekty_rodzaj.Count == 1) {
					Debug.Log ("Podnosz obiekt");
					// podnieś(obiekt);
				}
				//2 przedmioty np. Podnieś zielony samochód leżący obok czarnej skrzynki
				else if (zapytanie.obiekty_rodzaj.Count <= 2) {
					Debug.Log ("Nie mam takiej funkcji");
				}
			}
			//1 czasowniki podnoszenia, z sektorem np. Podnieś cokolwiek z sektora A5
			else if (zapytanie.czasownik_podB && zapytanie.sektorB) {
				//0, 1 przdmiot
				if (zapytanie.obiekty_rodzaj.Count == 0 || zapytanie.obiekty_rodzaj.Count == 1) {
					Debug.Log ("Podnosze obiekt z podanego sektora");
					// podnieśZSektora(sektor);
				}
			}
		}




		// dźwig coś trzyma
		if (zapytanie.trzymaB) {
			//2 czasowniki, bez sektora
			if (zapytanie.czasownik_podB && zapytanie.czasownik_opuB && !zapytanie.sektorB) {
				//0 przdmiotów
				if (zapytanie.obiekty_rodzaj.Count == 0) {
					Debug.Log ("Nie mam takiej funkcji");
				}
				//1,2 przedmioty np. połóż trzmany obiekt i podnieś samochód
				else if (zapytanie.obiekty_rodzaj.Count == 1 || zapytanie.obiekty_rodzaj.Count == 2) {
					Debug.Log ("Odkładam trzymany przedmiot A i podnosze B");
					// połóż(); podnieś(obiekt)
				}
			}
			//2 czasowniki, z sektorem
			else if (zapytanie.czasownik_podB && zapytanie.czasownik_opuB && zapytanie.sektorB) {
				//0 przdmiotów np. Odłóż co trzymasz i podnieś przedmiot z sektora B3
				if (zapytanie.obiekty_rodzaj.Count == 0) {
					Debug.Log ("Odkładam trzyman przedmiot A i podnoszę przedmiot B z podanego sektora");
					//połóż(); podnieśZSektora();
				}
				//1,2 przedmioty np. Odłóż co trzymasz w sektorze B4 i podnieś samochód
				else if (zapytanie.obiekty_rodzaj.Count == 1 || zapytanie.obiekty_rodzaj.Count == 2) {
					Debug.Log ("Odkładam trzymany przedmiot A w sektorze i podnosze B");
					// połóżwSektorze(sektor); podnieś(obiekt);
				}
			}
			//1 czasowniki opuszczania, bez sektora np. Połóż 
			else if (zapytanie.czasownik_opuB && !zapytanie.sektorB) {
				Debug.Log ("Odkładam trzymany przedmiot");
				//połóż();
			}
			//1 czasowniki opuszczania, z sektorem np. Odłóż skrzynkę w sektorze C3
			else if (zapytanie.czasownik_opuB && zapytanie.sektorB) {
				Debug.Log ("Odkładam trzymany przedmiot w sektorze");
				//połóżWSektorze(sektor);
			}
		}

	}
}