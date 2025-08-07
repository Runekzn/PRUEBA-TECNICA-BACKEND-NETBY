using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Response.API_Autentication
{
    public class Auth
    {
        public int userId { get; set; }
        public string Rol { get; set; } = string.Empty;
    }
}
