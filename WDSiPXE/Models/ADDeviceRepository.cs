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

        public ADDeviceRepository(IConfiguration config) {
            _domains = config.GetSection("AD:Domains").Get<ADDomainConfig[]>();
            _properties = config.GetSection("AD:Properties").Get<string[]>();
            Console.WriteLine($"Searching properties {_properties.Length}");
        }

        public Device GetDeviceById(string id)
        {
            Device device = null;
            String normalizedId = MacToNetbootGuid(id);
            foreach(ADDomainConfig domain in _domains) {
                using(DirectorySearcher searcher = new DirectorySearcher()) {
                    Console.WriteLine($"Connecting to {domain.ConnectionString}/{domain.SearchBase}");
                    searcher.SearchRoot = new DirectoryEntry($"{domain.ConnectionString}/{domain.SearchBase}");
                    searcher.Filter = String.Format(domain.Filter, normalizedId);
                    if(_properties.Length > 0) {
                        searcher.PropertiesToLoad.AddRange(_properties);
                    }

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