using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using System.DirectoryServices;
using System.Text;
using System.Collections.Specialized;

namespace WDSiPXE.Models
{
    public class ADDeviceRepository : IDeviceRepository
    {
        private ADDomainConfig[] _domains;
        private string[] _properties = new string[] { };
        public static readonly string DEFAULT_FILTER = "(&(objectCategory=computer)(netbootGuid={0})(operatingSystemVersion=*)(dNSHostName=*))";
        public static readonly string[] DEFAULT_PROPERTIES = new string[] { "name", "netbootGuid", "operatingSystemVersion", "dNSHostName" };

        public ADDeviceRepository(IConfiguration config) {
            _domains = config.GetSection("AD:Domains").Get<ADDomainConfig[]>() ?? new ADDomainConfig[] { new ADDomainConfig()};
            _properties = config.GetSection("AD:Properties").Get<string[]>() ?? DEFAULT_PROPERTIES;
            Console.WriteLine($"Searching properties {_properties.Length}");
        }

        public DirectorySearcher BuildDirectorySearcher(ADDomainConfig config, string NormalizedId) {
            DirectorySearcher searcher;
            if(String.IsNullOrEmpty(config.ConnectionString)) {
                searcher = new DirectorySearcher();
            } else {
                searcher = new DirectorySearcher(new DirectoryEntry(config.ConnectionString));
            }
            if(!String.IsNullOrEmpty(config.SearchBase)) {
                searcher.SearchRoot = new DirectoryEntry($"{searcher.SearchRoot.Path}/config.SearchBase");
            }
            if(String.IsNullOrEmpty(config.Filter)) {
                searcher.Filter = String.Format(ADDeviceRepository.DEFAULT_FILTER, NormalizedId);
            } else {
                searcher.Filter = String.Format(config.Filter, NormalizedId);
            }
            Console.WriteLine($"Search filter is {searcher.Filter}");
            if(_properties.Length > 0) {
                searcher.PropertiesToLoad.AddRange(_properties);
            }
            return searcher;
        }

        public Device GetDeviceById(string id)
        {
            Device device = null;
            String normalizedId = MacToNetbootGuid(id);
            foreach(ADDomainConfig domain in _domains) {
                using(DirectorySearcher searcher = BuildDirectorySearcher(domain, normalizedId)) {
                    Console.WriteLine($"Searching AD Domain {searcher.SearchRoot.Path}");
                    SearchResult adComputer = searcher.FindOne();
                    if(adComputer != null) {
                        device = new Device();
                        foreach(String key in adComputer.Properties.PropertyNames) {
                            Console.WriteLine($"Adding property {key}");
                            device[key] = adComputer.Properties[key];
                        }
                        break;
                    }
                }
            }
            if(device != null) {
                return device;
            } else {
                throw new DeviceNotFoundException(String.Format($"Unable to locate a device with id '{id}'"));
            }
        }

        public String MacToNetbootGuid(string mac) {
            String netbootGuid = mac.Trim().Replace(":", "").Replace("-", "").PadLeft(32, '0');
            StringBuilder netbootGuidSB = new StringBuilder();
            for(int i=0; i < netbootGuid.Length; i += 2) {
                netbootGuidSB.Append("\\");
                netbootGuidSB.Append(netbootGuid.Substring(i,2));
            }
            return netbootGuidSB.ToString();
        }
    }
}