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
        private string TESTID = "00-00-00-00-00-01";

        private IDeviceRepository GetTestDeviceRepository() {
            FakeDeviceRepository fdr = new FakeDeviceRepository();
            Device d = new Device();
            String netbootGuid = this.TESTID;
            d.Add("netbootGuid", netbootGuid);
            d.Add("templateName", "dynamictemplate");
            fdr.Devices.Add(netbootGuid, d);
            return fdr;
        }

        [TestMethod]
        public void GetDevicebyId()
        {
            IDeviceRepository fdr = this.GetTestDeviceRepository();
            DeviceTemplateModule module = new DeviceTemplateModule(fdr);
            Browser browser = new Browser(with => with.Module(module));
            BrowserResponse response = browser.Get("/DeviceTemplateModuleTests/" + this.TESTID, c => {
                //c.Header("Accept", "text/plain");
                c.HostName("localhost");
                c.Query("Template", "netbootguid");
            });
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(this.TESTID, response.Body.AsString());
        }

        [TestMethod]
        public void BaseUrl()
        {
            IDeviceRepository fdr = this.GetTestDeviceRepository();
            DeviceTemplateModule module = new DeviceTemplateModule(fdr);
            Browser browser = new Browser(with => with.Module(module));
            BrowserResponse response = browser.Get("/DeviceTemplateModuleTests/" + this.TESTID, c =>
            {
                c.HostName("localhost");
                c.Query("Template", "baseurl");
            });
            Assert.AreEqual("http://localhost", response.Body.AsString());
        }

        [TestMethod]
        public void TemplateProperty()
        {
            IDeviceRepository fdr = this.GetTestDeviceRepository();
            DeviceTemplateModule module = new DeviceTemplateModule(fdr);
            Browser browser = new Browser(with => with.Module(module));
            BrowserResponse response = browser.Get("/DeviceTemplateModuleTests/" + this.TESTID, c =>
            {
                c.HostName("localhost");
                c.Query("TemplateProperty", "templateName");
            });
            Assert.AreEqual("DynamicTemplateContent", response.Body.AsString());
        }
    }
}