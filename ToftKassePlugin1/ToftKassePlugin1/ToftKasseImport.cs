using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Uniconta.API.Plugin;
using Uniconta.API.Service;
using Uniconta.API.System;
using Uniconta.Common;

namespace ToftKassePlugin1
{
    public class ToftKasseImport : IPluginBase
    {
        public string Name => "toft plugin";

        public event EventHandler OnExecute;

        public ErrorCodes Execute(UnicontaBaseEntity master, UnicontaBaseEntity currentRow, IEnumerable<UnicontaBaseEntity> source, string command, string args)
        {
            if (!IsWindowOpen<Window>("ToftImport"))
            {
                MainWindow form = new MainWindow();
                form.Name = "ToftImport";
                form.ShowDialog();
                return ErrorCodes.Succes;
            }
            else
            {
                Window wnd = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.Name.Equals("ToftImport"));
                wnd?.Activate();
                return ErrorCodes.Succes;
            }
        }
        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
               ? Application.Current.Windows.OfType<T>().Any()
               : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }

        public string[] GetDependentAssembliesName()
        {
            return new string[] { };
        }

        public string GetErrorDescription()
        {
            return "";
        }

        public void Intialize()
        {
            
        }

        public void SetAPI(BaseAPI api)
        {
            Configuration.BaseApi = api;
            Configuration.CrudApi = new CrudAPI(api.session, api.CompanyEntity);
        }

        public void SetMaster(List<UnicontaBaseEntity> masters)
        {
            
        }
    }
}
