using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Win32;
using IniParser;
using IniParser.Model;
using System.IO;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace WDSiPXE.Models
{
    public class WDSDeviceRepository : IDeviceRepository
    {
        private String _path;

        public WDSDeviceRepository(IConfiguration config) {
            String remoteInstallPath;
            if(String.IsNullOrEmpty(config["WDS:RemoteInstallPath"])) {
                remoteInstallPath = WDSDeviceRepository.GetRemoteInstallPath();
            } else {
                remoteInstallPath = config["WDS:RemoteInstallPath"];
            }
            this._path = Path.Combine(remoteInstallPath, "Stores", "wdssdc", "wdssdc.ini");
            if (!File.Exists(this._path))
            {
                throw new FileNotFoundException("Unable to locate WDS store at " + this._path);
            }
        }

        public static String GetRemoteInstallPath()
        {
            String keyPath = "HKEY_LOCAL_MACHINE\\System\\CurrentControlSet\\Services\\WDSServer\\Providers\\WDSTFTP";
            String defaultValue = Environment.ExpandEnvironmentVariables("%SYSTEMROOT%\\RemoteInstall");
            String registryValue = (String)Registry.GetValue(keyPath, "RootFolder", defaultValue) ?? defaultValue;
            return registryValue;
        }

        public Device GetDeviceById(string id)
        {
            id = NormalizeMacAddress(id);
            FileIniDataParser parser = new FileIniDataParser();
            IniData repository = parser.ReadFile(this._path);
            try
            {
                SectionData deviceSection = repository.Sections.First<SectionData>(s =>
                {
                    String[] nameParts = s.SectionName.Split(new Char[] { '.' }, 2);
                    if (nameParts.Length == 2 && nameParts[0] == "Device")
                    {
                        String deviceId = repository[s.SectionName]["WDS.Device.ID"].Trim(new Char[] { '[', ']' });
                        return deviceId.ToLower() == id.ToLower();
                    }
                    else
                    {
                        return false;
                    }
                });

                Device device = new Device();
                foreach (KeyData k in deviceSection.Keys)
                {
                    Object value = k.Value;
                    if (k.Value.StartsWith("'") && k.Value.EndsWith("'"))
                    {
                        value = k.Value.Trim('\'');
                    }
                    device[k.KeyName] = value;
                }
                if (!device.ContainsKey("ID"))
                {
                    device["ID"] = id;
                }
                return device;
            }
            catch (InvalidOperationException ioe)
            {
                throw new DeviceNotFoundException(String.Format("Unable to locate a device with id '{0}'", id), ioe);
            }
        }

        public string NormalizeMacAddress(string id) {
            return id.Trim().Replace(":", "-");
        }
    }
}