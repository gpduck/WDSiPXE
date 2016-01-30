using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDSiPXE;

namespace WDSiPXE.Tests
{
    class FakeDeviceRepository : IDeviceRepository
    {
        public Dictionary<String, Device>Devices = new Dictionary<String, Device>();


        public Device GetDeviceById(string id)
        {
            if (Devices.ContainsKey(id))
            {
                return Devices[id];
            }
            else
            {
                throw new DeviceNotFoundException("Device with id " + id + " not found.");
            }
        }
    }
}
