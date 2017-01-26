using System;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;


public class AnalizaZapytania : MonoBehaviour{
	Zapytanie zapytanie;
	Slownik slownik;
	GameObject [] obiekty;
	GameObject [] findedObjects;
	List <Dopytanie> dopytania=new List<Dopytanie>();

	string [] substrings;
	char delimiter = ' ';

	string [] koloryNaScenie;
	string [] rodzajeNaScenie;

	public AnalizaZapytania (){
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

		List <int> pozostaleIndexy=new List<int>(zapytanie.obiekty_rodzaj);
		findedObjects=new GameObject[zapytanie.obiekty_rodzaj.Count];

		for (int i = 0; i < findedObjects.Length; i++) {
			findedObjects [i] = null;
		}

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
					findedObjects [i] = (znajdzNaScenie (rodzaj, kolor));
					pozostaleIndexy.Remove (zapytanie.obiekty_rodzaj [i]);
				}
			}
		}

			/*
		 Wywołanie funkcji znajdzNaScenie dla każdego indeksu, który pozostał.
		 */

		/*
		Wyszukuje obiekty dla indeksów dla których nie znaleziono obiektu w pętli wyżej czyli jeśli podano tylko rodzaj.
		*/
		if (pozostaleIndexy.Count > 1) {
			for (int i = 0; i < pozostaleIndexy.Count; i++) {
				dopytania.Add (new Dopytanie(substrings [pozostaleIndexy [i]], i));
			}
		} else if (pozostaleIndexy.Count == 1) {
			for (int i = 0; i < pozostaleIndexy.Count; i++) {
				int h = 0;
				/*while sprawdza na której pozycji w tablicy umieścić obiekt. 
			 * Konieczne jeśli PIERWSZY wyszukany obiekt był DRUGIM wpisanym przez użytkownika.*/
				while (h < findedObjects.Length && findedObjects [h] != null) {
					h++;
				}
				dopytania.Add (new Dopytanie(substrings [pozostaleIndexy [i]], h));
			}
		}
			
		int g=0;
		int k=dopytania.Count;
		while (k > 0) {
			findedObjects[dopytania[g].index]=znajdzNaScenie (dopytania[g].rodzaj, dopytania[g].index);

			if (findedObjects [dopytania [g].index] != null) {
				dopytania.RemoveAt (g);
			} else {
				g++;
			}
			k--;
		}
	}

	GameObject znajdzNaScenie(string r, string k){
		GameObject znaleziony = null;
		for (int i = 0; i < obiekty.Length; i++) {
			if(obiekty[i].GetComponent<objectProperties>().kolor.Contains(k)&&obiekty[i].GetComponent<objectProperties>().rodzaj.Contains(r)){
				znaleziony = obiekty [i];
			}
		}
		return znaleziony;
	}

	GameObject znajdzNaScenie (string r, int index){
		GameObject znaleziony = null;
		int takieSame = 0;
		//sprawdzenie ile obiektów jest tego samego rodzaju
		for (int i = 0; i < obiekty.Length; i++) {
			if (obiekty [i].GetComponent<objectProperties> ().rodzaj.Contains (r)) {
				takieSame++;
				znaleziony = obiekty [i];
			}
		}

		if (takieSame > 1) { //jeśli więcej niż jeden to dodaj rodzaj do dopytania
			return null;
		} else { //jeśli nie to znaleziono obiekt
			return znaleziony;
		}

	}

	public void dopytaj(string tekstDopytania){
		string [] dopytanie = tekstDopytania.ToLower ().Split (' '); //dopytanie wpisane przez użytkownika
		tokenyDopytania (dopytanie); 

		//dopytania = doSprawdzenia; //lista dopytań

		if (zapytanie.twierdzenieB) {
			CraneManager.Instance.craneText ("Wybieram obiekt losowo");
			findedObjects[dopytania[0].index]=wybierzObiektLosowo (dopytania[0].rodzaj);
			dopytania.RemoveAt (0);
			zapytanie.twierdzenieB = false;
			if (dopytania.Count > 0)
				dopytaj("");
		} else if (zapytanie.przeczenieB) {
			CraneManager.Instance.craneText ("Podaj kolor.");
			zapytanie.przeczenieB = false;
		} else if (zapytanie.kolorB) {
			findedObjects[dopytania[0].index]=znajdzNaScenie ( dopytania[0].rodzaj, dopytanie[zapytanie.kolor]);
			dopytania.RemoveAt (0);
			zapytanie.kolorB = false;
			if (dopytania.Count > 0)
				dopytaj("");
		}
		else {
			CraneManager.Instance.craneText ("Znalazłem więcej niż jeden obiekt rodzaju: " + dopytania [0].rodzaj);
			CraneManager.Instance.craneText ("Wybrać obiekt losowo?");
		}

		//jeśli dopytał o wszystko to musi uruchomić znajdowanie polecenia z tego miejsca
		if (dopytania.Count == 0)
			znajdzPolecenie ();
		
	}

	GameObject wybierzObiektLosowo(string rodzaj){
		List<GameObject> doWylosowania=new List<GameObject>();
		int ilosc=0;
		//pętla sprawdza czy użytkownik wpisał więcej niż jeden obiekt tego samego rodzaju. Zabezpieczenie przed wylosowaniem 2x tego samego obiektu
		for (int i = 0; i < findedObjects.Length; i++) {
			if (findedObjects [i] != null) {
				if (findedObjects [i].GetComponent<objectProperties> ().rodzaj.Contains (rodzaj)) {
					ilosc++;
				}
			}
		}

		if (ilosc > 1) {
			return null;
		} else {
			for (int i = 0; i < obiekty.Length; i++) {
				if (obiekty [i].GetComponent<objectProperties> ().rodzaj.Contains (rodzaj)) {
					doWylosowania.Add (obiekty [i]);
				}
			}
			int indexWylosowanego = UnityEngine.Random.Range (0, doWylosowania.Count);
			return doWylosowania[indexWylosowanego];
		}
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

	public int iloscDopytan(){
		return dopytania.Count;
	}

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
			if (Regex.IsMatch (substring, @"^[a-j]{1}[0-9]{1}$")) {
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

	bool sprawdzProwadnice(float przesuniecie){
		float aktualnaPozycja=CraneManager.Instance.prowadnicaPosition ();
		if ((aktualnaPozycja + przesuniecie) > 40 || (aktualnaPozycja + przesuniecie) < 4) {
			CraneManager.Instance.craneText ("Wprowadzono zbyt wielką wartość. Przesuwam do skrajnej pozycji");
			return true;
		}else 
			return false;
	}

	bool sprawdzLine(float przesuniecie){
		float dodatek;

		if (CraneManager.Instance.checkJoint ()) {
			dodatek = CraneManager.Instance.trzymanyObiekt ().GetComponent<BoxCollider> ().size.y;
		} else {
			dodatek = 0f;
		}
		float aktualnaSkala = CraneManager.Instance.linaPosition ();
		if ((aktualnaSkala + przesuniecie + dodatek) > 45 || (aktualnaSkala + przesuniecie) < 1) {
			CraneManager.Instance.craneText ("Wprowadzono zbyt wielką wartość. Przesuwam do skrajnej pozycji");
			return true;
		}else 
			return false;
	
	}
	public void znajdzPolecenie(){
		zapytanie.trzymaB = CraneManager.Instance.checkJoint ();

		// liczba i jednostki
		if (zapytanie.liczba != 0 && zapytanie.jednostkiB && zapytanie.liczba_czasownikow > 0) {
			float distance = zapytanie.liczba;
			// przesuwanie haka np. Przesuń hak o 4 metry w tył
			if (zapytanie.kierunekB) {
				// hak w tył
				if (substrings [zapytanie.kierunek].Contains ("tył")) {
					distance *= -1;
					if (sprawdzProwadnice(distance)) {
						distance=(CraneManager.Instance.prowadnicaPosition ()*-1f)+4f;
						StartCoroutine (CraneManager.Instance.przesunProwadnice (distance));
					} else {
						CraneManager.Instance.craneText ("Przesuwam hak o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki] + " w tył");
						StartCoroutine (CraneManager.Instance.przesunProwadnice (zapytanie.liczba * -1f));
					}
				}
				// hak w przód
				else if (substrings [zapytanie.kierunek].Contains ("przód") || substrings [zapytanie.kierunek].Contains ("przod")) {
					if (sprawdzProwadnice(distance)) {
						distance=40f-CraneManager.Instance.prowadnicaPosition();
						StartCoroutine (CraneManager.Instance.przesunProwadnice (distance));
					} else {
						CraneManager.Instance.craneText ("Przesuwam hak o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki] + " w przód");
						StartCoroutine (CraneManager.Instance.przesunProwadnice (zapytanie.liczba));
					}
				}
				// obracanie dźwigu np. Obróć dźwig o 50 stopni w lewo
				else if (zapytanie.obrocB) {
					if (substrings [zapytanie.kierunek].Contains ("lew")) {
						distance = distance * -1f;
						CraneManager.Instance.craneText ("Obracam o " + distance + " stopni w lewo");
					} else {
						CraneManager.Instance.craneText ("Obracam o "+distance+" stopni w prawo");
					}

					StartCoroutine (CraneManager.Instance.obroc (distance));
				}
			}

			// opuszczanie liny
			else if (zapytanie.czasownik_opuB || zapytanie.czasownik_podB) {
				if (zapytanie.czasownik_opuB) {
					if(sprawdzLine(zapytanie.liczba)){
						distance=40f-CraneManager.Instance.linaPosition();
						StartCoroutine (CraneManager.Instance.opuscHak (distance));
					}else{
					CraneManager.Instance.craneText ("Opuszczam hak o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki]);
					StartCoroutine (CraneManager.Instance.opuscHak (zapytanie.liczba));
					}
				} else if (zapytanie.czasownik_podB) {
					if(sprawdzLine(zapytanie.liczba*-1f)){
						distance=CraneManager.Instance.linaPosition()-1f;
						StartCoroutine (CraneManager.Instance.opuscHak (distance));
					}else{
						CraneManager.Instance.craneText ("Podnoszę hak o " + zapytanie.liczba + " " + substrings [zapytanie.jednostki]);
						StartCoroutine (CraneManager.Instance.opuscHak (zapytanie.liczba * -1f)); 
					}
					 
				}
			}
		}


		// dźwig nic nie trzyma
		else if (!zapytanie.trzymaB) {
			//2 czasowniki, bez sektora
			if (zapytanie.czasownik_podB && zapytanie.czasownik_opuB && !zapytanie.sektorB) {
				//0 przdmiotów np. Podnieś połóż
				if (zapytanie.obiekty_rodzaj.Count == 0) {
					CraneManager.Instance.craneText ("Nie podałeś przedmiotu");
				}
				//1 przedmiot np. Podnieś samochód i go odłóż
				else if (zapytanie.obiekty_rodzaj.Count == 1) {
					CraneManager.Instance.craneText ("Podnoszę i opuszczam "+findedObjects[0].GetComponent<objectProperties>().kolor+" "+findedObjects[0].GetComponent<objectProperties>().rodzaj);
					StartCoroutine (CraneManager.Instance.podniesIodloz(findedObjects[0]));
					// podnieś(obiekt); połóż(obiekt);
				}
				//2 przedmioty np. Podnieś samochód i połóż go obok skrzynki
				else if (zapytanie.obiekty_rodzaj.Count == 2) {
					CraneManager.Instance.craneText ("Podnoszę "+findedObjects[0].GetComponent<objectProperties>().kolor+" "+findedObjects[0].GetComponent<objectProperties>().rodzaj+" i kładę obok wskazanego obiektu");
					StartCoroutine (CraneManager.Instance.polozObok(findedObjects[0],findedObjects[1]));
					//podnieś(obiekt1); połóżObok(obiekt1,obiekt2);
				}
			}
			//2 czasowniki z sektorem docelowym
			else if (zapytanie.czasownik_podB && zapytanie.czasownik_opuB && zapytanie.sektorB) {
				//0 przdmiotów np. Podnieś połóż B5
				if (zapytanie.obiekty_rodzaj.Count == 0) {
					CraneManager.Instance.craneText ("Nie podałeś przedmiotu");
				}
				//1 przedmiot np. Podnieś czerwony samochód i połóż go w sektorze A3
				else if (zapytanie.obiekty_rodzaj.Count == 1) {
					int indexSektora = 0;
					if (zapytanie.sektory.Count == 1) {
						indexSektora = zapytanie.sektory [0];
					} else if (zapytanie.sektory.Count == 2) {
						indexSektora = zapytanie.sektory [1];
					}
					CraneManager.Instance.craneText ("Podnoszę " + findedObjects [0].GetComponent<objectProperties> ().kolor + " " + findedObjects [0].GetComponent<objectProperties> ().rodzaj);
					StartCoroutine (CraneManager.Instance.podniesIpolozWsektorze (findedObjects[0],substrings [indexSektora]));
					// podnieś(obiekt); połóżWSektorze(sektor)
				}
				//2 przedmioty np. Podnieś samochód i połóż go w sektorze a3 obok skrzynki
				else if (zapytanie.obiekty_rodzaj.Count >= 2) {
					CraneManager.Instance.craneText ("Ta operacja jest dla mnie za trudna");
				}
			}
			//1 czasowniki podnoszenia, bez sektora
			else if (zapytanie.czasownik_podB && !zapytanie.sektorB) {
				//0 przdmiotów np. Podnieś
				if (zapytanie.obiekty_rodzaj.Count == 0) {
					CraneManager.Instance.craneText ("Nie podałeś przedmiotu");
				}
				//1 przedmiot np. Podnieś zielony samochód
				else if (zapytanie.obiekty_rodzaj.Count == 1) {
					CraneManager.Instance.craneText ("Podnoszę "+findedObjects[0].GetComponent<objectProperties>().kolor+" "+findedObjects[0].GetComponent<objectProperties>().rodzaj);
					StartCoroutine (CraneManager.Instance.podniesObiekt(findedObjects[0]));
					// podnieś(obiekt);
				}
				//2 przedmioty np. Podnieś zielony samochód leżący obok czarnej skrzynki
				else if (zapytanie.obiekty_rodzaj.Count <= 2) {
					CraneManager.Instance.craneText ("Nie mam takiej funkcji");
				}
			}
			//1 czasowniki podnoszenia, z sektorem np. Podnieś cokolwiek z sektora A5
			else if (zapytanie.czasownik_podB && zapytanie.sektorB) {
				//0, 1 przdmiot
				if (zapytanie.obiekty_rodzaj.Count == 0 || zapytanie.obiekty_rodzaj.Count == 1) {
					StartCoroutine (CraneManager.Instance.podniesZsektora(substrings[zapytanie.sektory[0]]));
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
					CraneManager.Instance.craneText ("Nie mam takiej funkcji");
				}
				//1,2 przedmioty np. połóż trzmany obiekt i podnieś samochód
				else if (zapytanie.obiekty_rodzaj.Count == 1 || zapytanie.obiekty_rodzaj.Count == 2) {
					if (findedObjects.Length == 1) {
						CraneManager.Instance.craneText ("Odkładam "+CraneManager.Instance.trzymanyObiekt().GetComponent<objectProperties>().kolor+" "+CraneManager.Instance.trzymanyObiekt().GetComponent<objectProperties>().rodzaj+" i podnoszę "+findedObjects[0].GetComponent<objectProperties>().kolor+" "+findedObjects[0].GetComponent<objectProperties>().rodzaj);
						StartCoroutine (CraneManager.Instance.odlozIpodnies(findedObjects[0]));
					} else if (findedObjects.Length == 2) {
						CraneManager.Instance.craneText ("Odkładam "+findedObjects[0].GetComponent<objectProperties>().kolor+" "+findedObjects[0].GetComponent<objectProperties>().rodzaj+" i podnoszę "+findedObjects[1].GetComponent<objectProperties>().kolor+" "+findedObjects[1].GetComponent<objectProperties>().rodzaj);
						StartCoroutine (CraneManager.Instance.odlozIpodnies(findedObjects[1]));
					}
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
					if(zapytanie.obiekty_rodzaj.Count == 1){
						StartCoroutine (CraneManager.Instance.polozWsektorzeIpodnies(substrings[zapytanie.sektory[0]],findedObjects[0]));
					}else if(zapytanie.obiekty_rodzaj.Count == 2){
						StartCoroutine (CraneManager.Instance.polozWsektorzeIpodnies(substrings[zapytanie.sektory[0]],findedObjects[1]));
					}
					// połóżwSektorze(sektor); podnieś(obiekt);
				}
			}
			else if (zapytanie.czasownik_opuB && !zapytanie.sektorB && zapytanie.obiekty_rodzaj.Count == 1) {
				StartCoroutine (CraneManager.Instance.polozTrzymanyObok (findedObjects[0]));
				CraneManager.Instance.craneText ("Odkładam trzymany przedmiot obok podanego");
				//połóżObok();
			}
			//1 czasowniki opuszczania, bez sektora np. Połóż 
			else if (zapytanie.czasownik_opuB && !zapytanie.sektorB) {
				CraneManager.Instance.craneText ("Odkładam "+CraneManager.Instance.trzymanyObiekt().GetComponent<objectProperties>().kolor+" "+CraneManager.Instance.trzymanyObiekt().GetComponent<objectProperties>().rodzaj);
				StartCoroutine (CraneManager.Instance.putDown());
			}
			//1 czasowniki opuszczania, z sektorem np. Odłóż skrzynkę w sektorze C3
			else if (zapytanie.czasownik_opuB && zapytanie.sektorB) {
				Debug.Log ("Odkładam trzymany przedmiot w sektorze");
				int indexSektora = 0;
				if (zapytanie.sektory.Count == 1) {
					indexSektora = zapytanie.sektory [0];
				} else if (zapytanie.sektory.Count == 2) {
					indexSektora = zapytanie.sektory [1];
				}
				StartCoroutine (CraneManager.Instance.polozWsektorze (substrings [indexSektora]));
				//połóżWSektorze(sektor);
			}
		}

	}


}