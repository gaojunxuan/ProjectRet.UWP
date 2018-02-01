using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectRet.UWP.Models
{
    public class Command
    {
        public string Body { get; set; }
        public string Credential { get; set; }
        public string GUID { get; set; }
        public override string ToString()
        {
            return $"projectret:?comm={Body}&auth={Credential}&key={Uri.EscapeDataString(GUID)}";
        }
    }
}
