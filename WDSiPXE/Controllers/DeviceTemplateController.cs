using System;
using System.DirectoryServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WDSiPXE.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.Extensions;

namespace WDSiPXE.Controllers
{
    public class DeviceTemplateController : Controller
    {
        private IDeviceRepository _deviceRepository;
        private IConfiguration _config;

        public DeviceTemplateController(IDeviceRepository deviceRepository, IConfiguration config) {
          _deviceRepository = deviceRepository;
          _config = config;
        }

        public IActionResult GetDevice(string TemplateFolder,
                                string DeviceId,
                                string TemplateProperty = null,
                                string Template = null)
        {
          try {
            Device device = _deviceRepository.GetDeviceById(DeviceId);
            DeviceTemplate model = new DeviceTemplate();
            model.Device = device;
            model.ID = DeviceId;
            model.Global = _config.GetSection("Global");

            Uri uri = new Uri(Request.GetEncodedUrl());
            String baseUrl = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped).ToString();
            model.BaseUrl = baseUrl;

            String view = $"Views/{TemplateFolder}";

            if(!String.IsNullOrEmpty(Template)) {
              view += "/" + Template;
            } else if(!String.IsNullOrEmpty(TemplateProperty)) {
              if(!device.ContainsKey(TemplateProperty)) {
                return new UnprocessableEntityObjectResult($"The device did not contain a value for the TemplateProperty '{TemplateProperty}'");
              }
              if(device[TemplateProperty] is ResultPropertyValueCollection) {
                view += "/" + ((ResultPropertyValueCollection)device[TemplateProperty])[0];
              } else {
                view += "/" + device[TemplateProperty];
              }
            }
            view += ".cshtml";
            Response.ContentType = "text/plain";
            return View(view, model);
          } catch (DeviceNotFoundException e) {
            return NotFound(e.Message);
          } catch (Exception e) {
            return View(new ErrorViewModel { ErrorMessage = e.Message });
          }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { });
        }
    }
}
