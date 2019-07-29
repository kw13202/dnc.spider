using System;
using System.Collections.Generic;
using System.Text;

namespace dnc.model
{
    public class Proxy
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public bool? IsValid { get; set; }
    }
}
