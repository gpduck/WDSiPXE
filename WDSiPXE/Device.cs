using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace WDSiPXE
{
    public class Device : Hashtable
    {
        public Device() : base(StringComparer.InvariantCultureIgnoreCase) { }
    }
}