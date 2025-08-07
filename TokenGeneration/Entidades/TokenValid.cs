using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenGeneration.Entidades
{
    public class TokenValid
    {
        public bool IsValid { get; set; }
        public string Rol { get; set; } = string.Empty;
        public int UserId { get; set; } 
    }
}
