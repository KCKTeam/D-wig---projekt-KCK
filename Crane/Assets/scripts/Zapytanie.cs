using System;
using System.Collections.Generic;

public class Zapytanie{

	string tekstZapytania;

	public bool czasownik_podB, czasownik_opuB;
	public bool obrocB, kierunekB, hakB;
	public bool trzymaB;                   // czy trzyma coś
	public List <int> obiekty_rodzaj;
	public List <int> obiekty_kolor;
	public List <int> zaimki;
	public List <int> sektory;

	public int czasownik_pod, czasownik_opu;
	public int obroc, kierunek, hak;
	public float liczba=0;
	public int sektor_z, sektor_do, sektor;
	public int trzyma;

	public Zapytanie (string text){
		tekstZapytania = text;
	}

	public string getTekstZapytania(){
		return tekstZapytania;
	}


}

