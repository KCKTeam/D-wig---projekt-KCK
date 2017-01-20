using System;
using UnityEngine;
using System.Text.RegularExpressions;

public class AnalizaZapytania : MonoBehaviour{
	Zapytanie zapytanie;
	Slownik slownik;

	string [] substrings;
	char [] tokeny;
	char delimiter = ' ';

	public AnalizaZapytania (){
	}

	public void dodajZapytanie(Zapytanie doDodania){
		zapytanie = doDodania;
		substrings = zapytanie.getTekstZapytania ().ToLower().Split (delimiter);
		tokeny = new char[10];
	}

	public void dodajSlownik(Slownik doDodania){
		slownik = doDodania;
	}
		
	public void znajdzTokeny(){
		int index = 0;
		foreach (string substring in substrings) {
			
			//znajdowanie czasownika podnoszenia
			for (int i = 0; i < slownik.czas_pod.Length; i++) {
				if (substring.Contains (slownik.czas_pod [i])) {
					zapytanie.czasownik_podB = true;
					zapytanie.czasownik_pod = index;
					tokeny [index] = 'c';
					zapytanie.liczba_czasownikow++;
				}
			}

			//znajdowanie czasownika opuszczania
			for (int i = 0; i < slownik.czas_opu.Length; i++) {
				if (substring.Contains (slownik.czas_opu[i])) {
					zapytanie.czasownik_opuB = true;
					zapytanie.czasownik_opu = index;
					tokeny [index] = 'c';
					zapytanie.liczba_czasownikow++;
				}
			}

			//znajdowanie czasownika przesunięcia
			for (int i = 0; i < slownik.czas_przesun.Length; i++) {
				if (substring.Contains (slownik.czas_przesun[i])) {
					zapytanie.czas_przesunB = true;
					zapytanie.czas_przesun = index;
					tokeny [index] = 'c';
					zapytanie.liczba_czasownikow++;
				}
			}


			//znajdowanie rodzajów
			for (int i = 0; i < slownik.rodzaje.Length; i++) {
				if (substring.Contains (slownik.rodzaje [i])) {
					zapytanie.obiekty_rodzaj.Add (index);
					tokeny [index] = 'r';
				}
			}

			//znajdowanie kolorów
			for (int i = 0; i < slownik.kolory.Length; i++) {
				if (substring.Contains (slownik.kolory [i])) {
					zapytanie.obiekty_kolor.Add (index);
					tokeny [index] = 'k';
				}
			}

			//znajdowanie liczby
			if (Regex.IsMatch (substring, @"^\d+$")) {
				float.TryParse (substring, out zapytanie.liczba);
				tokeny [index] = 'l';
			}

			//znajdowanie słów obrotu
			for (int i = 0; i < slownik.czas_obrotu.Length; i++) {
				if (substring.Contains (slownik.czas_obrotu[i])) {
					zapytanie.obrocB = true;
					zapytanie.obroc = index;
					tokeny [index] = 'c';
					zapytanie.liczba_czasownikow++;
				}
			}

			//znajdowanie słów kierunku
			for (int i = 0; i < slownik.kierunki.Length; i++) {
				if (substring.Contains (slownik.kierunki[i])) {
					zapytanie.kierunekB = true;
					zapytanie.kierunek = index;
					tokeny [index] = 'j';
				}
			}

			//znajdowanie zaimków
			for (int i = 0; i < slownik.zaimki.Length; i++) {
				if (substring.Contains (slownik.zaimki[i])) {
					zapytanie.zaimki.Add (index);
					tokeny [index] = 'z';
				}
			}

			//znajdowanie sektorów
			for (int i = 0; i < slownik.sektor.Length; i++) {
				if (substring.Contains (slownik.sektor[i])) {
					zapytanie.sektory.Add (index);
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

	public void CKYstart(){
		CKY parserCYK = new CKY ();
		parserCYK.Parsowanie (tokeny);
	}

	public void znajdzPolecenie(){
		zapytanie.trzymaB = CraneManager.Instance.checkJoint ();
		zapytanie.trzymaB = true;

		//Debug.Log(zapytanie.zaimki [0]);

		if (zapytanie.liczba_czasownikow == 1 && zapytanie.obiekty_rodzaj.Count<=1 && zapytanie.sektory.Count<1) {
			
			if (zapytanie.obrocB && zapytanie.jednostkiB && zapytanie.liczba != 0) {
				if (substrings [zapytanie.kierunek].Contains ("lew")) {
					zapytanie.liczba = zapytanie.liczba * -1f;
				}
				float kat = zapytanie.liczba;
				StartCoroutine (CraneManager.Instance.rotation (kat));
			}
			if (zapytanie.czasownik_opuB && zapytanie.liczba != 0 && zapytanie.jednostkiB && zapytanie.obiekty_rodzaj.Count == 0) {
				Debug.Log ("Opuszczam hak o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki]);
			}

			if (zapytanie.czasownik_podB && zapytanie.liczba != 0 && zapytanie.jednostkiB && zapytanie.obiekty_rodzaj.Count == 0) {
				Debug.Log ("Podnoszę hak o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki]);
			}
			if (zapytanie.czasownik_podB && zapytanie.liczba != 0 && zapytanie.jednostkiB && zapytanie.obiekty_rodzaj.Count == 1) {
				Debug.Log ("Podnoszę przedmiot o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki]);
			}

			if (zapytanie.czasownik_opuB && zapytanie.liczba != 0 && zapytanie.jednostkiB && zapytanie.obiekty_rodzaj.Count == 1) {
				Debug.Log ("Opuszczam przedmiot o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki]);
			}
			if (zapytanie.czasownik_podB && zapytanie.obiekty_rodzaj.Count == 1) {
				Debug.Log ("Podnoszę...");
			}
			if (zapytanie.czasownik_opuB && zapytanie.obiekty_rodzaj.Count == 1) {
				Debug.Log ("Opuszczam...");
			}

			if (zapytanie.czas_przesunB && zapytanie.liczba != 0 && zapytanie.jednostkiB) {
				float distance = zapytanie.liczba;
				if (zapytanie.kierunekB) {
					if (substrings [zapytanie.kierunek].Contains ("tył")) {
						distance *= -1;
						Debug.Log ("Przesuwam hak o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki] + " w tył");
					} else
						Debug.Log ("Przesuwam hak o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki] + " w przód");
				}
			}

		}
		
		else if (zapytanie.liczba_czasownikow == 1 && zapytanie.obiekty_rodzaj.Count==2) {
			Debug.Log ("Przenoszę A obok B");
		}

		else if(zapytanie.liczba_czasownikow == 2 && zapytanie.obiekty_rodzaj.Count==2){
			Debug.Log ("Podnoszę A i kładę obok B");
		}

		else if(zapytanie.czasownik_podB&&zapytanie.obiekty_rodzaj.Count==1&&zapytanie.sektory.Count==1){
			Debug.Log ("Podnoszę i umieszczam w sektorze...");
		}

		else if(zapytanie.czasownik_opuB&&zapytanie.trzymaB&&zapytanie.sektory.Count==1){
			Debug.Log ("Umieszczam trzymany obiekt w sektorze...");
		}

	}
}

