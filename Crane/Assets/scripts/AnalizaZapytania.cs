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

	public void CKYstart(){
		CKY parserCYK = new CKY ();
		parserCYK.Parsowanie (tokeny);
	}

	public void znajdzPolecenie(){
		zapytanie.trzymaB = CraneManager.Instance.checkJoint ();
		zapytanie.trzymaB = false;

		// Pozostałe opcje

		if (zapytanie.obrocB && zapytanie.jednostkiB && zapytanie.liczba != 0) {
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
					Debug.Log ("Podnosz i opuszczam przedmiot");
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