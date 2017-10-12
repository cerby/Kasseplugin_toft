using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Uniconta.API.Service;
using Uniconta.API.System;
using Uniconta.Common;
using Uniconta.DataModel;
using Uniconta.ClientTools;
using System.Globalization;

namespace ToftKassePlugin1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private static UnicontaConnection Connection = new UnicontaConnection(APITarget.Live);
        public static Session currentSession = new Session(Connection);
        //info til at connecte til uniconta og starte sessíon
        //private static CrudAPI crudAPI;
        //Company currentCompany;
        //login key info
        GLDailyJournal invJournal;
        Kasseopsaetning[] kasse;
        Typekonfiguration[] type;
        List<Kasseopsaetning_Toft> summe;
        List<Kasseopsaetning_Toft> records;
        SQLCache GLAccountCache;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UnsummedGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
 
            

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
             Read();
            await Insert();
            MessageBox.Show("Indlæsning af fil er færdig");
        }

        public async Task Query()
        {
            List<PropValuePair> filter = new List<PropValuePair>();
            var wherefilter = PropValuePair.GenereteWhereElements("Journal", typeof(string), "Dag");
            filter.Add(wherefilter);
            invJournal = (await Configuration.CrudApi.Query<GLDailyJournal>(null, filter)).FirstOrDefault();
            kasse = await Configuration.CrudApi.Query<Kasseopsaetning>();
            type = await Configuration.CrudApi.Query<Typekonfiguration>();
            GLAccountCache = await Configuration.CrudApi.CompanyEntity.LoadCache(typeof(GLAccount), Configuration.CrudApi);
            //her gør vi en query for at få den til at forstå hvilke tabeller den skal arbejde med på uniconta siden            
        }
        private void Read()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                //lader funktionen forstå at den kun skal åbne en fil og kigge efter *.dat filer

                //hvis det lykkes at åbne filen, så gå videre med ne
                StreamReader sr;
                try
                {
                    sr = new StreamReader(new FileStream(ofd.FileName, FileMode.Open), Encoding.GetEncoding("iso-8859-1"));
                }
                catch (Exception)
                {
                    throw new CustomException("Filen er i brug andetsteds, luk filen og prøv igen");
                }              
                //læs filen og benyt dansk unicode sæt
                CsvConfiguration config = new CsvConfiguration();
                config.Delimiter = ";";
                config.RegisterClassMap(new MyClassmap());
                config.HasHeaderRecord = false;
                CsvReader csv;
                try
                {
                    csv = new CsvReader(sr, config);
                    records = csv.GetRecords<Kasseopsaetning_Toft>().OrderBy(s => s.Klient).ThenBy(s => s.Kasse)
                    .ThenBy(s => s.Type.Length).ThenBy(s => s.Type).ThenBy(s => s.Dato).ToList();
                }
                catch (Exception)
                {

                    throw new CustomException("Filen kunne ikke læses");
                }               

                //csvreader skal vide hvad vores delimiter er, den skal også vide hvilken struktur filen har og den skal vide, at den skal benytte input fra streamreader
                //til sidst bliver den bedt om at sortere listen efter type og så efter dato
                summe = Calculations(records);
                //kør calculation metoden
            }
            else
            {
                MessageBox.Show("no file");
            }

        }
        private List<Kasseopsaetning_Toft> Calculations(List<Kasseopsaetning_Toft> records)
        {
            int total = records.Count();
            //skaffer totale antal poster i vores fil
            double beloeb = Convert.ToDouble(records[0].Beloeb);
            //konverter første beløb i filen til double og læg ind i beloeb, så den kan benyttes i loopet
            Kasseopsaetning_Toft kasse;
            //opret en holder til vores info
            summe = new List<Kasseopsaetning_Toft>();
            //opret liste af klasse til at holde summene for de forskellige typer og datoer
            for (int i = 0; i < total - 1; i++)
            {
                if (records[i].Kasse == records[i + 1].Kasse && records[i].Klient == records[i + 1].Klient &&
                    records[i].Type == records[i + 1].Type && records[i].Dato == records[i + 1].Dato)
                {
                    beloeb = beloeb + Convert.ToDouble(records[i + 1].Beloeb);
                    //såfremt kasse, klient, type og dato matcher, så læg beløbene sammen
                }
                else
                {
                    kasse = new Kasseopsaetning_Toft();
                    //ny instans af klasse hver gang, da den ellers fucker op i beløbene
                    kasse.Klient = records[i].Klient;
                    kasse.Kasse = records[i].Kasse;
                    kasse.Type = records[i].Type;
                    kasse.Dato = records[i].Dato;
                    kasse.Beloeb = Convert.ToString(beloeb);
                    beloeb = Convert.ToDouble(records[i + 1].Beloeb);
                    summe.Add(kasse);
                    //gem sum information i kasse, nulstil beløb og smid kasse i listen
                }
            }

            //viser listen
            return summe.OrderBy(d => d.Dato).ToList(); ;
        }

        public async Task Insert()
        {
            await Query();
            //currentCompany = await currentSession.OpenCompany(companyId, true);
            //crudAPI = new CrudAPI(currentSession, currentCompany);
            summe = Calculations(records);
            int localtype;
            List<GLDailyJournalLine> newJournalLines = new List<GLDailyJournalLine>();
            Typekonfiguration kassetype = new Typekonfiguration();
            if (summe != null)
            {
                foreach (var i in summe)
                {
                    localtype = Convert.ToInt32(i.Type);
                    var kasseopsætning = kasse.FirstOrDefault(p => p.KeyStr == i.Klient + "-" + i.Kasse);
                    if (kasseopsætning != null && localtype > 99)
                    {
                        kassetype = type.FirstOrDefault(p => p.Kasse == kasseopsætning.KeyStr && p.Type.ToString() == i.Type);
                    }
                    GLDailyJournalLine JournalLine = new GLDailyJournalLine();
                    JournalLine._Date = DateTime.ParseExact(i.Dato, "ddMMyy", CultureInfo.InvariantCulture);
                    JournalLine._Voucher = Convert.ToInt32(i.Dato);
                    GLAccount gLAccount;
                    if (localtype == 10)
                    {
                        JournalLine._Text = "Varesalg inkl. moms";
                        try
                        {
                            JournalLine._Account = kasseopsætning.Omsaetning;
                        }
                        catch (Exception)
                        {
                            throw new CustomException("Ingen kasse fundet med id " + i.Klient + "-" + i.Kasse);
                        }
                        gLAccount = GLAccountCache.Get(kasseopsætning.Omsaetning) as GLAccount;
                        JournalLine._Vat = gLAccount._Vat;
                        JournalLine._Credit = Convert.ToDouble(i.Beloeb);
                        JournalLine._AccountType = (byte)GLJournalAccountType.Finans;
                    }
                    else if (localtype == 30)
                    {
                        JournalLine._Text = "Returbeløb ink. moms";
                        JournalLine._Text = "Varesalg inkl. moms";
                        try
                        {
                            JournalLine._Account = kasseopsætning.Omsaetning;
                        }
                        catch (Exception)
                        {
                            throw new CustomException("Ingen kasse fundet med id " + i.Klient + "-" + i.Kasse);
                        }
                        gLAccount = GLAccountCache.Get(kasseopsætning.Omsaetning) as GLAccount;
                        JournalLine._Vat = gLAccount._Vat;
                        JournalLine._Credit = Convert.ToDouble(i.Beloeb);
                        JournalLine._AccountType = (byte)GLJournalAccountType.Finans;
                    }
                    else if (localtype == 40)
                    {
                        JournalLine._Text = "Vareforbrug";
                        JournalLine._Text = "Varesalg inkl. moms";
                        try
                        {
                            JournalLine._Account = kasseopsætning.Omsaetning;
                        }
                        catch (Exception)
                        {
                            throw new CustomException("Ingen kasse fundet med id " + i.Klient + "-" + i.Kasse);
                        }
                        gLAccount = GLAccountCache.Get(kasseopsætning.Omsaetning) as GLAccount;
                        JournalLine._Vat = gLAccount._Vat;
                        JournalLine._Credit = Convert.ToDouble(i.Beloeb);
                        JournalLine._AccountType = (byte)GLJournalAccountType.Finans;
                    }
                    else if (localtype > 99 && localtype < 1000 && kassetype != null)
                    {
                        JournalLine._Text = kassetype.Navn;
                        JournalLine._Account = kassetype.Konto;
                        gLAccount = GLAccountCache.Get(kassetype.Konto) as GLAccount;
                        JournalLine._Vat = gLAccount._Vat;
                        JournalLine._Credit = Convert.ToDouble(i.Beloeb);

                        if (kassetype.Kontotype == "Finans")
                        {
                            JournalLine._AccountType = (byte)GLJournalAccountType.Finans;
                        }
                        else if (kassetype.Kontotype == "Kreditor")
                        {
                            JournalLine._AccountType = (byte)GLJournalAccountType.Creditor;
                        }
                        else if (kassetype.Kontotype == "Debtor")
                        {
                            JournalLine._AccountType = (byte)GLJournalAccountType.Debtor;
                        }
                        
                    }
                    if (invJournal != null)
                    {
                        JournalLine.SetMaster(invJournal);
                        newJournalLines.Add(JournalLine);
                    }
                }
                if (newJournalLines.Any())
                {
                    ErrorCodes insertResult = await Configuration.CrudApi.Insert(newJournalLines);
                }
            }
        }
    }
    public class CustomException : ApplicationException
    {
        public CustomException() : base() { }
        public CustomException(string sMessage) : base(sMessage) { }
    }
}
