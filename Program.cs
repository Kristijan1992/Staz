using System;
using System.Globalization;

// Apstraktna klasa Osoba koja sadrži osnovne podatke o osobi
abstract class Osoba
{
    public string Ime { get; set; }
    public string Prezime { get; set; }
    public string OIB { get; set; }

    // Konstruktor za inicijalizaciju Osoba objekta
    protected Osoba(string ime, string prezime, string oib) => (Ime, Prezime, OIB) = (ime, prezime, oib);

    // Apstraktna metoda za ispis podataka
    public abstract void IspisPodataka();
}

// Klasa Zaposlenik nasleđuje klasu Osoba i dodaje specifične atribute za zaposlenika
class Zaposlenik : Osoba
{
    public string Uloga { get; set; }
    public DateTime DatumZaposlenja { get; set; }

    // Konstruktor za inicijalizaciju Zaposlenik objekta
    public Zaposlenik(string ime, string prezime, string oib, string uloga, DateTime datumZaposlenja)
        : base(ime, prezime, oib) => (Uloga, DatumZaposlenja) = (uloga, datumZaposlenja);

    // Metoda za izračun radnog staža
    public (int, int, int) IzracunajStaz()
    {
        DateTime danas = DateTime.Now;
        int godine = danas.Year - DatumZaposlenja.Year;
        int mjeseci = danas.Month - DatumZaposlenja.Month;
        int dani = danas.Day - DatumZaposlenja.Day;

        // Prilagođavanje mjeseci i dana ako su negativni
        if (dani < 0)
        {
            mjeseci--;
            dani += DateTime.DaysInMonth(danas.Year, danas.Month == 1 ? 12 : danas.Month - 1);
        }

        if (mjeseci < 0)
        {
            godine--;
            mjeseci += 12;
        }

        return (godine, mjeseci, dani);
    }

    // Override metoda za ispis podataka o zaposleniku
    public override void IspisPodataka()
    {
        var (godine, mjeseci, dani) = IzracunajStaz();
        Console.WriteLine($"Ime: {Ime}\nPrezime: {Prezime}\nOIB: {OIB}\nUloga: {Uloga}\nDatum zaposlenja: {DatumZaposlenja:dd.MM.yyyy}\nRadni staž: {godine} {GetJedMnogStr(godine, "godina", "godine")}, {mjeseci} {GetJedMnogStr(mjeseci, "mjesec", "mjeseca")} i {dani} {GetJedMnogStr(dani, "dan", "dana")}");
    }

    // Pomoćna metoda za dobijanje pravilnog oblika jednine ili množine
    private string GetJedMnogStr(int broj, string jednina, string mnozina) => broj == 1 ? jednina : mnozina;
}

// Glavna klasa Programa
class Program
{
    static void Main()
    {
        // Kreiranje novog zaposlenika i unos podataka
        var zaposlenik = new Zaposlenik(UnosNepraznog("Unesite ime: "), UnosNepraznog("Unesite prezime: "), UnosOIB("Unesite OIB (11 znamenki): "), UnosNepraznog("Unesite ulogu: "), UnosDatum("Unesite datum zaposlenja (dd.mm.yyyy): "));
        zaposlenik.IspisPodataka(); // Ispis podataka o zaposleniku
        PritisniEnterZaNastavak(); // Čekanje na pritisak Entera za nastavak
    }

    // Metoda za unos nepraznog stringa
    static string UnosNepraznog(string poruka)
    {
        string unos;
        do
        {
            Console.Write(poruka);
            unos = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(unos))
                Console.WriteLine("Ovo polje ne može biti prazno. Molimo pokušajte ponovo.");
        }
        while (string.IsNullOrWhiteSpace(unos));
        return unos;
    }

    // Metoda za unos OIB-a koji mora biti 11 znamenki
    static string UnosOIB(string poruka)
    {
        string oib;
        while (true)
        {
            Console.Write(poruka);
            oib = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(oib) && oib.Length == 11 && long.TryParse(oib, out _))
                break;
            Console.WriteLine("OIB mora biti točno 11 znamenki i ne smije biti prazan. Molimo pokušajte ponovo.");
        }
        return oib;
    }

    // Metoda za unos datuma u specifičnom formatu
    static DateTime UnosDatum(string poruka)
    {
        DateTime datum;
        while (true)
        {
            Console.Write(poruka);
            if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out datum) && datum <= DateTime.Now)
                break;
            Console.WriteLine("Pogrešan format datuma ili datum veći od trenutnog. Molimo unesite datum u formatu dd.mm.yyyy.");
        }
        return datum;
    }

    // Metoda za čekanje na pritisak Entera
    static void PritisniEnterZaNastavak()
    {
        Console.WriteLine("Pritisnite Enter za nastavak...");
        while (Console.ReadKey().Key != ConsoleKey.Enter) { }
    }
}
