using Uniconta.DataModel;

namespace ToftKassePlugin1
{
    public class Kasseopsaetning : TableDataWithKey
    {
        public override int UserTableId { get { return 1418; } }
        public string Butiksnummer
        {
            get { return this.GetUserFieldString(0); }
            set { this.SetUserFieldString(0, value); }
        }

        public string Butiksnavn
        {
            get { return this.GetUserFieldString(1); }
            set { this.SetUserFieldString(1, value); }
        }

        public string Kasse
        {
            get { return this.GetUserFieldString(2); }
            set { this.SetUserFieldString(2, value); }
        }

        public string Kontant
        {
            get { return this.GetUserFieldString(3); }
            set { this.SetUserFieldString(3, value); }
        }

        public string Omsaetning
        {
            get { return this.GetUserFieldString(4); }
            set { this.SetUserFieldString(4, value); }
        }

        public string Kassedifference
        {
            get { return this.GetUserFieldString(5); }
            set { this.SetUserFieldString(5, value); }
        }

        public string Oeredifference
        {
            get { return this.GetUserFieldString(6); }
            set { this.SetUserFieldString(6, value); }
        }

        public string Bank
        {
            get { return this.GetUserFieldString(7); }
            set { this.SetUserFieldString(7, value); }
        }

    }
}
