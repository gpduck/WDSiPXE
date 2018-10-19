using System;
using System.Collections;
using Microsoft.Extensions.Configuration;

namespace WDSiPXE.Models
{
    public class DeviceTemplate : Hashtable
    {
        public String BaseUrl { get; set; }
        public Device Device { get; set; }
        public String ID { get; set; }
        public IConfiguration Global { get; set; }
    }
}