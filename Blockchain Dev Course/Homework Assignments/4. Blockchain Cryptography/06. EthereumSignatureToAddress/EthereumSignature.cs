using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthereumSignatureToAddress
{
    internal class EthereumSignature
    {
        public string Hash { get; set; }

        public string Signature { get; set; }

        public string V { get; set; }

        public string R { get; set; }

        public string S { get; set; }
    }
}
