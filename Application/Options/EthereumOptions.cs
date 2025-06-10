using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Options
{
    public class EthereumOptions
    {
        //public string RpcUrl { get; set; }

        public List<string> RpcUrls { get; set; } = new List<string>();
    }
}
