using System;
using System.Collections.Generic;

public class Zapytanie{

	string tekstZapytania;

	public bool czasownik_podB, czasownik_opuB, czas_przesunB;
	public bool obrocB, kierunekB, hakB;
	public bool trzymaB;                   // czy trzyma coś
	public List <int> obiekty_rodzaj= new List<int>();
	public List <int> obiekty_kolor= new List<int>();
	public List <int> zaimki= new List<int>();
	public List <int> sektory= new List<int>();
	public bool jednostkiB;

	public int liczba_czasownikow;
	public int czasownik_pod, czasownik_opu, czas_przesun;
	public int obroc, kierunek, hak;
	public float liczba=0;
	public int sektor_z, sektor_do, sektor;
	public int trzyma;
	public int jednostki;

	public Zapytanie (string text){
		tekstZapytania = text;
	}

	public string getTekstZapytania(){
		return tekstZapytania;
	}


}

