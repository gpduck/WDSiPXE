using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using WDSiPXE;

namespace WDSiPXE.Tests
{
    [TestClass]
    public class WDSDeviceRepositoryTests
    {
        [TestMethod]
        public void GetDeviceById()
        {
            WDSDeviceRepository wds = new WDSDeviceRepository(Directory.GetCurrentDirectory());
            Device d = wds.GetDeviceById("00-00-00-00-00-01");
            Assert.AreEqual("test", d["WDS.Device.Name"]);
            Assert.AreEqual("[00-00-00-00-00-01]", d["WDS.Device.ID"]);
        }

        [TestMethod]
        [ExpectedException(typeof(DeviceNotFoundException))]
        public void DeviceIdDoesNotExist()
        {
            WDSDeviceRepository wds = new WDSDeviceRepository(Directory.GetCurrentDirectory());
            wds.GetDeviceById("234");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void WDSSDCDoesNotExist()
        {
            WDSDeviceRepository wds = new WDSDeviceRepository("c:\\pathdoesnotexist");
            wds.GetDeviceById("234");
        }
    }
}
