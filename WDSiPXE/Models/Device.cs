using System;
using System.Collections;

namespace WDSiPXE.Models
{
    public class Device : Hashtable
    {
        public Device() : base(StringComparer.InvariantCultureIgnoreCase) { }
    }
}