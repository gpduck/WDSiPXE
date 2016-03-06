using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WDSiPXE.Tests
{
    public abstract class BaseDeviceRepository
    {
        protected abstract IDeviceRepository GetRepository();
        protected abstract IList<KeyValuePair<string, object>> TestValues { get; }

        [TestMethod]
        public void GetDeviceById()
        {
            IDeviceRepository repository = this.GetRepository();
            Device d = repository.GetDeviceById("00-00-00-00-00-01");
            foreach (KeyValuePair<string, object> kv in TestValues)
            {
                Assert.AreEqual(kv.Value, d[kv.Key]);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(DeviceNotFoundException))]
        public void DeviceIdDoesNotExist()
        {
            IDeviceRepository repository = this.GetRepository();
            repository.GetDeviceById("234");
        }
    }
}
