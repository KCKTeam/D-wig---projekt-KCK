using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace parsowanie_zdania
{

    public class Zdanie
    {
        public static Zdanie[] tablica = new Zdanie[40];
        public int nr;
        public String wyraz;
        public int ojciec;
        public int PokazNr() { return this.nr; }
        public void ZapiszZPliku()
        {
            int counter = 0;
            string line;
            
            // wczytanie pliku conll 
            System.IO.StreamReader file =
                new System.IO.StreamReader(@"j:\Documents\Visual Studio 2015\out.conll");
            while ((line = file.ReadLine()) != null)

            {
            // wczytuje linie dłuższe niż 2 znaki  (zabezpieczenie przed ostatnią linią)
               if(line.Length>2) {
                    string[] wyrazy = line.Split('\t');
                    tablica[counter] = new Zdanie();
                    //System.Console.WriteLine(wyrazy[0]);
                    tablica[counter].nr = Int32.Parse(wyrazy[0]);
                    tablica[counter].wyraz = wyrazy[1];
                    tablica[counter].ojciec = Int32.Parse(wyrazy[6]);
                }
                counter++; //licznik linii
            }

            file.Close();
        }

        static void Main(string[] args)
        {
            Zdanie z = new Zdanie();    //nowy obiekt typu zdanie
            z.ZapiszZPliku();           //wykonanie operacji zapisania z pliku do tablicy
            Console.WriteLine(tablica[0].wyraz);
            Console.ReadKey(); 
        }
    }
    class Program
    {
      
        
    }
}
