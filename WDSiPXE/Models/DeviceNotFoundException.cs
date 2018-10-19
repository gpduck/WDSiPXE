using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WDSiPXE.Models
{
    public class DeviceNotFoundException : Exception
    {
        public DeviceNotFoundException() : base() { }
        public DeviceNotFoundException(String message) : base(message) { }
        public DeviceNotFoundException(String message, Exception innerException) : base(message, innerException) { }
    }
}