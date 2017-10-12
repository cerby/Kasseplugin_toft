using CsvHelper.Configuration;


namespace ToftKassePlugin1
{
    public sealed class MyClassmap : CsvClassMap<Kasseopsaetning_Toft>
    {
        public MyClassmap()
        {
            Map(m => m.Linjetype).Index(0);
            Map(m => m.Type).Index(1);
            Map(m => m.Noinfo1).Index(2);
            Map(m => m.Kasse).Index(3);
            Map(m => m.Klient).Index(4);
            Map(m => m.Beloeb).Index(5);
            Map(m => m.Moms).Index(6);
            Map(m => m.Dato).Index(7);
            Map(m => m.Bon).Index(8);
            Map(m => m.Noinfo10).Index(9);
            Map(m => m.Noinfo11).Index(10);
            Map(m => m.Noinfo12).Index(11);
            Map(m => m.Noinfo13).Index(12);
            Map(m => m.Noinfo14).Index(13);
            Map(m => m.Noinfo15).Index(14);
            Map(m => m.Noinfo16).Index(15);
            Map(m => m.Noinfo17).Index(16);
            Map(m => m.Varenavn).Index(17);
            Map(m => m.Varenavn2).Index(18);
            Map(m => m.Saeson).Index(19);
            Map(m => m.Noinfo21).Index(20);
            Map(m => m.Noinfo22).Index(21);
            Map(m => m.Noinfo23).Index(22);
            Map(m => m.Noinfo24).Index(23);
            Map(m => m.Noinfo25).Index(24);
            Map(m => m.Noinfo26).Index(25);
            Map(m => m.Noinfo27).Index(26);
        }
        //for at få CSVreader til at fatte hvad der hører til hvor af information, er vi nødt til at forklare den det, denne klasse gør netop dette
    }
}
