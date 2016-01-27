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
        public Device GetDeviceById(string id)
        {
            Device d = new Device();
            d.Add("netbootGuid", id);
            return d;
        }
    }
}
