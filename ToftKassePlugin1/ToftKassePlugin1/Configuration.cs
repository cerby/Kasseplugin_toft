using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uniconta.API.Service;
using Uniconta.API.System;

namespace ToftKassePlugin1
{
    public static class Configuration
    {
        public static CrudAPI CrudApi { get; set; }
        public static BaseAPI BaseApi { get; set; }
    }
}
