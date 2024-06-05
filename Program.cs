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
        Console.WriteLine($"Ime: {Ime}\nPrezime: {Prezime}\nOIB: {OIB}\nUloga: {Uloga}\nDatum zaposlenja: {DatumZaposlenja:dd.MM.yyyy}\nRadni staž: {godine} {GetJedMnogStr(godine, "godina", "godine", "godina")}, {mjeseci} {GetJedMnogStr(mjeseci, "mjesec", "mjeseca", "mjeseci")} i {dani} {GetJedMnogStr(dani, "dan", "dana", "dana")}");
    }

    // Pomoćna metoda za dobijanje pravilnog oblika jednine ili množine
    private string GetJedMnogStr(int broj, string jednina, string mnozinaJed, string mnozinaDv = null)
    {
        if (broj == 1) return jednina;
        if (broj >= 2 && broj <= 4 || (broj % 10 >= 2 && broj % 10 <= 4 && (broj % 100 < 10 || broj % 100 >= 20))) return mnozinaJed;
        return mnozinaDv ?? mnozinaJed;
    }
}

class Program
{
    static void Main()
    {
        var zaposlenik = ZaposlenikService.CreateZaposlenik();
        zaposlenik.IspisPodataka();
        ZaposlenikService.WaitForEnter();
    }
}

class ZaposlenikService
{
    public static Zaposlenik CreateZaposlenik()
    {
        string ime = GetNonEmptyInput("Unesite ime: ");
        string prezime = GetNonEmptyInput("Unesite prezime: ");
        string oib = GetOIBInput("Unesite OIB (11 znamenki): ");
        string uloga = GetNonEmptyInput("Unesite ulogu: ");
        DateTime datumZaposlenja = GetDateInput("Unesite datum zaposlenja (dd.mm.yyyy): ");
        return new Zaposlenik(ime, prezime, oib, uloga, datumZaposlenja);
    }

    public static void WaitForEnter()
    {
        Console.WriteLine("Pritisnite Enter za nastavak...");
        while (Console.ReadKey().Key != ConsoleKey.Enter) { }
    }

    // Metoda za unos nepraznog stringa
    private static string GetNonEmptyInput(string message)
    {
        string input;
        do
        {
            Console.Write(message);
            input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                Console.WriteLine("Ovo polje ne može biti prazno. Molimo pokušajte ponovo.");
        }
        while (string.IsNullOrWhiteSpace(input));
        return input;
    }

    // Metoda za unos OIB-a koji mora biti 11 znamenki
    private static string GetOIBInput(string message)
    {
        string oib;
        while (true)
        {
            Console.Write(message);
            oib = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(oib) && oib.Length == 11 && long.TryParse(oib, out _))
                break;
            Console.WriteLine("OIB mora biti točno 11 znamenki i ne smije biti prazan. Molimo pokušajte ponovo.");
        }
        return oib;
    }

    // Metoda za unos datuma u specifičnom formatu
    private static DateTime GetDateInput(string message)
    {
        DateTime date;
        while (true)
        {
            Console.Write(message);
            if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date) && date <= DateTime.Now)
                break;
            Console.WriteLine("Pogrešan format datuma ili datum veći od trenutnog. Molimo unesite datum u formatu dd.mm.yyyy.");
        }
        return date;
    }
}
