using System;
using System.Collections.Generic;
using Nancy.ViewEngines.Razor;

namespace WDSiPXE
{
    public class RazorConfig : IRazorConfiguration
    {

        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }

        public IEnumerable<string> GetAssemblyNames()
        {
            yield return "WDSiPXE";
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            yield return "WDSiPXE";
        }
    }
}