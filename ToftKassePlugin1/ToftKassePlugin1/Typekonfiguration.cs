using Uniconta.DataModel;

namespace ToftKassePlugin1
{
    public class Typekonfiguration : TableData
    {
        public override int UserTableId { get { return 1419; } }
        public string Kasse
        {
            get { return this.GetUserFieldString(0); }
            set { this.SetUserFieldString(0, value); }
        }

        public string Betegnelse
        {
            get { return this.GetUserFieldString(1); }
            set { this.SetUserFieldString(1, value); }
        }

        public string Konto
        {
            get { return this.GetUserFieldString(2); }
            set { this.SetUserFieldString(2, value); }
        }

        public string ModKonto
        {
            get { return this.GetUserFieldString(3); }
            set { this.SetUserFieldString(3, value); }
        }

        public string Likvtype
        {
            get { return this.GetUserFieldString(4); }
            set { this.SetUserFieldString(4, value); }
        }

        public string Navn
        {
            get { return this.GetUserFieldString(5); }
            set { this.SetUserFieldString(5, value); }
        }
        public long Type
        {
            get { return this.GetUserFieldInt64(6); }
            set { this.SetUserFieldInt64(6, value); }
        }
        public string Kontotype
        {
            get { return this.GetUserFieldString(7); }
            set { this.SetUserFieldString(7, value); }
        }


    }
}
