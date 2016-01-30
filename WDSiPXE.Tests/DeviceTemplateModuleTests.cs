using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using WDSiPXE;
using Nancy.Testing;
using Nancy;

namespace WDSiPXE.Tests
{
    [TestClass]
    public class DeviceTemplateModuleTests
    {
        [TestMethod]
        public void GetDevicebyId()
        {
            FakeDeviceRepository fdr = new FakeDeviceRepository();
            Device d = new Device();
            String netbootGuid = "00-00-00-00-00-01";
            d.Add("netbootGuid", netbootGuid);
            fdr.Devices.Add(netbootGuid, d);
            DeviceTemplateModule module = new DeviceTemplateModule(fdr);
            Browser browser = new Browser(with => with.Module(module));
            BrowserResponse response = browser.Get("/DeviceTemplateModuleTests/" + netbootGuid, c => {
                //c.Header("Accept", "text/plain");
                c.HostName("localhost");
                c.Query("Template", "netbootguid");
            });
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(response.Body.AsString(), netbootGuid);
        }
    }
}