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
            DeviceTemplateModule module = new DeviceTemplateModule(new FakeDeviceRepository());
            Browser browser = new Browser(with => with.Module(module));
            BrowserResponse response = browser.Get("/ipxe/00-00-00-00-00-01", c => {
                //c.Header("Accept", "text/plain");
                c.HostName("localhost");
                c.Query("Template", "esx55");
            });
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}