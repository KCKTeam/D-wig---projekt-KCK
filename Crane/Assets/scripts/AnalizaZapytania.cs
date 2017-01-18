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
		substrings = zapytanie.getTekstZapytania ().Split (delimiter);
		tokeny = new char[3];
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
				}
			}

			//znajdowanie czasownika opuszczania
			for (int i = 0; i < slownik.czas_opu.Length; i++) {
				if (substring.Contains (slownik.czas_opu[i])) {
					zapytanie.czasownik_opuB = true;
					zapytanie.czasownik_opu = index;
					tokeny [index] = 'c';
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
				}
			}

			//znajdowanie słów przesunięcia
			for (int i = 0; i < slownik.czas_przesun.Length; i++) {
				if (substring.Contains (slownik.czas_przesun[i])) {
					zapytanie.hakB = true;
					zapytanie.hak = index;
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


			index++;
		}
		Debug.Log (tokeny[0]);
	}

	public void CKYstart(){
		CKY parserCYK = new CKY ();
		parserCYK.Parsowanie (tokeny);
	}

	public void znajdzPolecenie(){
		if (zapytanie.obrocB && zapytanie.liczba != 0) {
			if (substrings [zapytanie.kierunek].Contains ("lew")) {
				zapytanie.liczba = zapytanie.liczba * -1f;
			}
			float kat = zapytanie.liczba;
			StartCoroutine(CraneManager.Instance.rotation(kat));
		}
	}
}

